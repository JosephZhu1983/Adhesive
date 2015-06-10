
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adhesive.Common;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Adhesive.Mongodb.Server.Imp
{
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        #region Table

        private void InternalGetTableData(Detail detail, Dictionary<string, string> row)
        {
            try
            {
                if (detail.SubDetails != null)
                {
                    foreach (var sub in detail.SubDetails)
                    {
                        var subdic = new Dictionary<string, string>();
                        InternalGetTableData(sub, subdic);
                        foreach (var item in subdic)
                        {
                            if (!row.ContainsKey(item.Key))
                                row.Add(item.Key, item.Value);
                        }
                    }
                }
                else
                {
                    if (!row.ContainsKey(detail.DisplayName.Replace('.', '_')))
                        row.Add(detail.DisplayName.Replace('.', '_'), detail.Value == null ? "" : detail.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Table", "InternalGetTableData", ex.Message);
                throw;
            }
        }

        public List<TableData> GetTableDataByContextId(string contextId)
        {
            try
            {
                var beginTime = new DateTime(DateTime.Now.Year, 1, 1);
                var endTime = DateTime.Now;
                var data = new ConcurrentBag<TableData>();
                var databaseInfos = MongodbServerMaintainceCenter.GetDatabaseInfos(beginTime, endTime);
                if (databaseInfos.Count == 0) return null;
                foreach(var databaseInfo in databaseInfos)
                {
                    var databasePrefix = databaseInfo.DatabasePrefix;
                    var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);
                    var contextColumn = columnDescriptions.FirstOrDefault(desc => desc.IsContextIdentityColumn);
                    if (contextColumn != null)
                    {
                        var collectionInfos = databaseInfo.Collections;
                        var tableData = GetTableData(databasePrefix, collectionInfos.Select(c => c.CollectionName).ToList(), databaseInfo.DatabaseDate.ToLocalTime(), databaseInfo.DatabaseDate.ToLocalTime().AddMonths(1), 0, 100, new Dictionary<string, object>
                        {
                            { contextColumn.ColumnName, contextId }
                        });
                        tableData.ForEach(item =>
                        {
                            foreach (var t in item.Tables)
                            {
                                if (t.Data.Count > 0)
                                {
                                    var d = new TableData
                                    {
                                        PkColumnDisplayName = item.PkColumnDisplayName,
                                        PkColumnName = item.PkColumnName,
                                        TableName = item.TableName,
                                        Tables = new List<Table>
                                        {
                                            t,
                                        }
                                    };
                                    data.Add(d);
                                }
                            }
                        });
                    }
                };
                return data.ToList();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Table", "GetTableDataByContextId", ex.Message);
                throw;
            }
        }

        public List<TableData> GetTableData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, int pageIndex, int pageSize, Dictionary<string, object> filters)
        {
            try
            {
                var data = new ConcurrentBag<TableData>();

                var databaseInfos = MongodbServerMaintainceCenter.GetDatabaseInfos(databasePrefix, beginTime, endTime);
                if (databaseInfos.Count == 0) return null;

                var typeFullName = MongodbServerMaintainceCenter.GetTypeFullName(databasePrefix);
                var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);
                var enumColumnDescriptions = MongodbServerMaintainceCenter.GetMongodbEnumColumnDescriptions(databasePrefix);
                var statTimeColumn = columnDescriptions.FirstOrDefault(c => c.IsTimeColumn);
                if (statTimeColumn == null) return null;

                var filterquery = Query.Null;
                if (filters != null)
                {
                    foreach (var item in filters)
                    {
                        if (item.Value != null)
                        {
                            if (item.Value is string && item.Value.ToString().Split(',').Length > 1)
                            {
                                var values = item.Value.ToString().Split(',');
                                filterquery = Query.And(filterquery, Query.In(item.Key, values.Select(val => BsonValue.Create(val)).ToArray()));
                            }
                            else
                            {
                                filterquery = Query.And(filterquery, Query.EQ(item.Key, BsonValue.Create(item.Value)));
                            }
                        }
                    }
                }
                var query = Query.And(Query.LT(statTimeColumn.ColumnName, endTime).GTE(beginTime), filterquery);

                var sortColumn = columnDescriptions.Where(desc => desc.MongodbSortOption != MongodbSortOption.None).FirstOrDefault();
                var sort = SortBy.Null;
                if (sortColumn != null)
                {
                    if (sortColumn.MongodbSortOption == MongodbSortOption.Descending)
                        sort = SortBy.Descending(sortColumn.ColumnName);
                    else
                        sort = SortBy.Ascending(sortColumn.ColumnName);
                }

                var showColumns = columnDescriptions.Where(desc => desc.ShowInTableView).ToList();
                var pkColumn = columnDescriptions.FirstOrDefault(desc => desc.IsPrimaryKey);
                if (pkColumn == null) return null;
                var fields = showColumns.Select(c => c.ColumnName).ToList();

                if (fields.Contains(statTimeColumn.ColumnName))
                {
                    fields.Remove(statTimeColumn.ColumnName);
                }
                fields.Insert(0, statTimeColumn.ColumnName);

                if (fields.Contains(pkColumn.ColumnName))
                {
                    fields.Remove(pkColumn.ColumnName);
                }
                fields.Insert(0, pkColumn.ColumnName);

                var server = CreateSlaveMongoServer(typeFullName);
                if (server == null) return null;

                Parallel.ForEach(tableNames, tableName =>
                {
                    var tables = new List<Table>();
                    var remainRows = pageSize;
                    //var totalCount = 0;
                    foreach (var databaseInfo in databaseInfos)
                    {
                        var table = new List<Dictionary<string, string>>();
                        var databaseName = databaseInfo.DatabaseName;
                        var database = server.GetDatabase(databaseName);
                        var collection = database.GetCollection(tableName);
                        //var count = Convert.ToInt32(collection.Count(query));
                        //totalCount += count;
                        if (remainRows > 0)
                        {
                            var q = collection.Find(query).SetFields(fields.ToArray())
                                .SetLimit(remainRows).SetSkip(pageIndex * pageSize);
                            if (sort != SortBy.Null)
                                q = q.SetSortOrder(sort);
                            var result = q.ToList();
                            foreach (var item in result)
                            {
                                var row = new Dictionary<string, string>();
                                foreach (var element in item)
                                {
                                    var details = new List<Detail>();
                                    InternalGetDetailData(string.Empty, details, element, columnDescriptions, enumColumnDescriptions, pkColumn.ColumnName);
                                    foreach (var detail in details)
                                    {
                                        InternalGetTableData(detail, row);
                                    }
                                }
                                table.Add(row);
                            }
                            var t = new Table
                            {
                                Data = table,
                                DatabaseName = databaseName,
                                DatabasePrefix = databasePrefix,
                                //TotalCount = 0,
                            };
                            tables.Add(t);
                            remainRows -= table.Count;
                        }
                    }

                    //tables.ForEach(t => t.TotalCount = totalCount);
                    data.Add(new TableData
                    {
                        TableName = tableName,
                        Tables = tables,
                        PkColumnName = pkColumn.ColumnName,
                        PkColumnDisplayName = pkColumn.DisplayName,
                    });
                });

                return data.ToList();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Table", "GetTableData", ex.Message);
                throw;
            }
        }

        #endregion
    }
}
