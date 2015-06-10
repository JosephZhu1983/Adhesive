
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adhesive.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Adhesive.Mongodb.Server.Imp
{
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        #region Group

        public List<Group> GetGroupData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            try
            {
                var data = new ConcurrentBag<Group>();
                var typeFullName = MongodbServerMaintainceCenter.GetTypeFullName(databasePrefix);
                var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);
                var groupColumnDescriptions = columnDescriptions.Where(cd => cd.MongodbFilterOption == MongodbFilterOption.DropDownListFilter
                    || cd.MongodbFilterOption == MongodbFilterOption.CheckBoxListFilter || cd.MongodbCascadeFilterOption != MongodbCascadeFilterOption.None).ToList();

                var statTimeColumn = columnDescriptions.FirstOrDefault(c => c.IsTimeColumn);
                if (statTimeColumn == null) return data.ToList();
                var query = Query.LT(statTimeColumn.ColumnName, endTime).GTE(beginTime);


                Parallel.ForEach(tableNames, tableName =>
                {
                    var item = new Group();
                    item.GroupItems = new List<GroupItem>();
                    item.TableName = tableName;
                    groupColumnDescriptions.AsParallel().ForAll(groupColumnDescription =>
                    {
                        var groupItem = new GroupItem
                        {
                            Description = groupColumnDescription.Description,
                            DisplayName = groupColumnDescription.DisplayName,
                            Name = groupColumnDescription.ColumnName,
                            Values = new Dictionary<GroupItemValuePair, int>(),
                        };

                        groupItem.Values = InternalGetGroupData(typeFullName, databasePrefix, beginTime, endTime, tableName, query, groupColumnDescription.ColumnName, filters);
                        lock (item.GroupItems)
                            item.GroupItems.Add(groupItem);

                    });
                    data.Add(item);
                });
                return data.ToList();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Group", "GetGroupData", ex.Message);
                throw;
            }
        }

        private Dictionary<GroupItemValuePair, int> InternalGetGroupData(string typeFullName, string databasePrefix, DateTime beginTime, DateTime endTime, string tableName, IMongoQuery query, string columnName, Dictionary<string, object> filters)
        {
            try
            {
                var data = new Dictionary<GroupItemValuePair, int>();
                var server = CreateSlaveMongoServer(typeFullName);

                var databaseInfos = MongodbServerMaintainceCenter.GetDatabaseInfos(databasePrefix, beginTime, endTime);
                if (databaseInfos.Count == 0) return data;

                foreach (var databaseInfo in databaseInfos)
                {
                    var database = server.GetDatabase(databaseInfo.DatabaseName);
                    var collection = database.GetCollection(tableName);
                    var collectionInfo = databaseInfo.Collections.FirstOrDefault(c => c.CollectionName == tableName);
                    if (collectionInfo == null) continue;
                    var columnInfo = collectionInfo.ListFilterColumns.FirstOrDefault(c => c.ColumnName == columnName);
                    if (columnInfo != null)
                    {
                        columnInfo.DistinctValues.ForEach(value =>
                        {
                            var count = 0;
                            bool needQueryDb = true;
                            var q = Query.And(query, Query.EQ(columnInfo.ColumnName, BsonValue.Create(value.Value)));
                            if (filters != null)
                            {
                                foreach (var filter in filters)
                                {
                                    if (filter.Value != null)
                                    {
                                        if (filter.Value is string && filter.Value.ToString().Split(',').Length > 1)
                                        {
                                            var values = filter.Value.ToString().Split(',');
                                            if (filter.Key != columnName)
                                            {
                                                q = Query.And(q, Query.In(filter.Key, values.Select(val => BsonValue.Create(val)).ToArray()));
                                            }
                                            else
                                            {
                                                if (!values.Contains(value.Value.ToString()))
                                                {
                                                    needQueryDb = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (filter.Key != columnName)
                                            {
                                                q = Query.And(q, Query.EQ(filter.Key, BsonValue.Create(filter.Value)));
                                            }
                                            else
                                            {
                                                if (filter.Value.ToString() != value.Value.ToString())
                                                {
                                                    needQueryDb = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (needQueryDb)
                                count = Convert.ToInt32(collection.Count(q));

                            if (data.Keys.Count(aa => aa.Name == value.Value) > 0)
                            {
                                var item = data.Keys.FirstOrDefault(aa => aa.Name == value.Value);
                                if (item != null)
                                    data[item] += count;
                            }
                            else
                                data.Add(new GroupItemValuePair
                                {
                                    DisplayName = value.Name,
                                    Name = value.Value,
                                }, count);
                        });

                    }

                    var columnInfo2 = collectionInfo.CascadeFilterColumns.FirstOrDefault(c => c.ColumnName == columnName);
                    if (columnInfo2 != null)
                    {
                        columnInfo2.DistinctValues.ForEach(value =>
                        {
                            var count = 0;
                            bool needQueryDb = true;
                            var q = Query.And(query, Query.EQ(columnInfo2.ColumnName, BsonValue.Create(value)));
                            if (filters != null)
                            {
                                foreach (var filter in filters)
                                {
                                    if (filter.Value != null)
                                    {
                                        if (filter.Key != columnName)
                                        {
                                            q = Query.And(q, Query.EQ(filter.Key, BsonValue.Create(filter.Value)));
                                        }
                                        else
                                        {
                                            if (filter.Value.ToString() != value.ToString())
                                            {
                                                needQueryDb = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (needQueryDb)
                                count = Convert.ToInt32(collection.Count(q));

                            if (data.Keys.Count(aa => aa.DisplayName == value) > 0)
                            {
                                var item = data.Keys.FirstOrDefault(aa => aa.DisplayName == value);
                                if (item != null)
                                    data[item] += count;
                            }
                            else
                                data.Add(new GroupItemValuePair
                                {
                                    DisplayName = value,
                                    Name = value,
                                }, count);
                        });
                    }
                }

                return data.Where(d => d.Value > 0).ToDictionary(a => a.Key, a => a.Value);
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Group", "InternalGetGroupData", ex.Message);
                throw;
            }
        }

        #endregion
    }
}
