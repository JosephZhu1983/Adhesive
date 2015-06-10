
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Adhesive.Common;
using Adhesive.Config;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Adhesive.Mongodb.Server.Imp
{
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        private static readonly Dictionary<string, MongoServer> slaveServerCache = new Dictionary<string, MongoServer>();

        internal static void ConfigUpdateCallbackForSlave()
        {
            LocalLoggingService.Info("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "ConfigUpdateCallbackForSlave", "配置改变，重新初始化");
            lock (slaveServerCache)
            {
                slaveServerCache.Clear();
            }
        }

        private static MongoServer CreateSlaveMongoServer(string typeName)
        {
            try
            {
                if (!slaveServerCache.ContainsKey(typeName))
                {
                    lock (slaveServerCache)
                    {
                        if (!slaveServerCache.ContainsKey(typeName))
                        {
                            var config = MongodbServerConfiguration.GetMongodbServerConfigurationItem(typeName);
                            if (config == null)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "CreateSlaveMongoServer",
                                    string.Format("没取到服务器配置项，参数：{0}", typeName));
                                return null;
                            }
                            var server = MongodbServerConfiguration.GetMongodbServerUrl(config.MongodbServerUrlName);
                            if (server == null)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "CreateSlaveMongoServer",
                                    string.Format("没取到服务器地址，参数：{0}", config.MongodbServerUrlName));
                                return null;
                            }
                            slaveServerCache.Add(typeName, MongoServer.Create(server.Slave));
                        }
                    }
                }
                return slaveServerCache[typeName];
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "CreateSlaveMongoServer",
                    string.Format("创建服务器链接出错，参数：{0}", typeName), ex.Message);
                throw;
            }
        }

        private static MongodbServerUrl GetMongodbServerUrl(string typeName)
        {
            var config = MongodbServerConfiguration.GetMongodbServerConfigurationItem(typeName);
            if (config == null)
                return null;
            var server = MongodbServerConfiguration.GetMongodbServerUrl(config.MongodbServerUrlName);
            if (server == null)
                return null;
            return server;
        }

        private string GetMemcachedKey(params object[] parms)
        {
            var key = parms.Select(o => o == null ? "" : o.ToString()).Aggregate((a, b) => a + b) + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return ConfigHelper.ComputeHash(key);
        }

        public Dictionary<MongodbServerUrl, ServerInfo> GetServerInfo()
        {
            try
            {
                return MongodbServerMaintainceCenter.GetServerInfos();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "GetServerInfo", ex.Message);
                throw;
            }
        }

        #region Filter

        public FilterData GetFilterData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime)
        {
            try
            {
                var data = new FilterData()
                {
                    CascadeFilters = new List<CascadeFilter>(),
                    ListFilters = new List<ListFilter>(),
                    TextboxFilters = new List<TextboxFilter>(),
                };
                var databaseInfos = MongodbServerMaintainceCenter.GetDatabaseInfos(databasePrefix, beginTime, endTime);
                if (databaseInfos.Count == 0) return null;

                foreach (var databaseInfo in databaseInfos)
                {
                    foreach (var tableName in tableNames)
                    {
                        var collectionInfo = databaseInfo.Collections.FirstOrDefault(c => c.CollectionName == tableName);
                        if (collectionInfo != null)
                        {
                            foreach (var filterColumn in collectionInfo.TextboxFilterColumns)
                            {
                                var filter = data.TextboxFilters.FirstOrDefault(f => f.ColumnName == filterColumn.ColumnName);
                                if (filter == null)
                                {
                                    data.TextboxFilters.Add(new TextboxFilter
                                    {
                                        ColumnName = filterColumn.ColumnName,
                                    });
                                }
                            }

                            foreach (var filterColumn in collectionInfo.ListFilterColumns)
                            {
                                var filter = data.ListFilters.FirstOrDefault(f => f.ColumnName == filterColumn.ColumnName);
                                if (filter == null)
                                {
                                    data.ListFilters.Add(new ListFilter
                                    {
                                        ColumnName = filterColumn.ColumnName,
                                        Items = filterColumn.DistinctValues,
                                    });
                                }
                                else
                                {
                                    filter.Items.AddRange(filterColumn.DistinctValues);
                                    filter.Items = filter.Items.Distinct(new LambdaComparer<ItemPair>((a, b) => a.Value.ToString() == b.Value.ToString())).ToList();
                                }
                            }

                            foreach (var filterColumn in collectionInfo.CascadeFilterColumns)
                            {
                                var dic = new Dictionary<string, List<string>>();
                                var values = filterColumn.DistinctValues;
                                foreach (var val in values)
                                {
                                    if (!val.Contains("__"))
                                    {
                                        dic.Add(val, new List<string>());
                                        continue;
                                    }

                                    var parent = val.Substring(0, val.LastIndexOf("__"));
                                    var child = val.Substring(val.LastIndexOf("__") + 2);
                                    if (dic.ContainsKey(parent))
                                        dic[parent].Add(child);
                                    else
                                        dic.Add(parent, new List<string> { child });
                                }

                                var filter = data.CascadeFilters.FirstOrDefault(f => f.ColumnName == filterColumn.ColumnName);
                                if (filter == null)
                                {
                                    data.CascadeFilters.Add(new CascadeFilter
                                    {
                                        ColumnName = filterColumn.ColumnName,
                                        Items = dic,
                                    });
                                }
                                else
                                {
                                    foreach (var item in dic)
                                    {
                                        if (filter.Items.ContainsKey(item.Key))
                                        {
                                            filter.Items[item.Key].AddRange(item.Value);
                                            filter.Items[item.Key] = filter.Items[item.Key].Distinct().ToList();
                                        }
                                        else
                                        {
                                            filter.Items.Add(item.Key, item.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var filter in data.TextboxFilters)
                    {
                        var columnDescription = MongodbServerMaintainceCenter.GetMongodbColumnDescription(databasePrefix, filter.ColumnName);
                        if (columnDescription != null)
                        {
                            filter.Description = columnDescription.Description;
                            filter.DisplayName = columnDescription.DisplayName;
                        }
                    }

                    foreach (var filter in data.ListFilters)
                    {
                        var columnDescription = MongodbServerMaintainceCenter.GetMongodbColumnDescription(databasePrefix, filter.ColumnName);
                        if (columnDescription != null)
                        {
                            filter.Description = columnDescription.Description;
                            filter.DisplayName = columnDescription.DisplayName;
                            switch (columnDescription.MongodbFilterOption)
                            {
                                case MongodbFilterOption.CheckBoxListFilter:
                                    filter.ListFilterType = ListFilterType.MultipleSelect;
                                    break;
                                case MongodbFilterOption.DropDownListFilter:
                                    filter.ListFilterType = ListFilterType.Select;
                                    break;
                            }
                        }
                    }

                    foreach (var filter in data.CascadeFilters)
                    {
                        var columnDescription = MongodbServerMaintainceCenter.GetMongodbColumnDescription(databasePrefix, filter.ColumnName);
                        if (columnDescription != null)
                        {
                            filter.Description = columnDescription.Description;
                            filter.DisplayName = columnDescription.DisplayName;
                            switch (columnDescription.MongodbCascadeFilterOption)
                            {
                                case MongodbCascadeFilterOption.LevelOne:
                                    filter.CascadeFilterType = CascadeFilterType.LevelOne;
                                    break;
                                case MongodbCascadeFilterOption.LevelTwo:
                                    filter.CascadeFilterType = CascadeFilterType.LevelTwo;
                                    break;
                                case MongodbCascadeFilterOption.LevelThree:
                                    filter.CascadeFilterType = CascadeFilterType.LevelThree;
                                    break;
                            }
                        }
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "GetFilterData", ex.Message);
                throw;
            }
        }

        #endregion

        #region DataCount

        public int GetDataCount(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            try
            {
                var count = 0;
                var databaseInfos = MongodbServerMaintainceCenter.GetDatabaseInfos(databasePrefix, beginTime, endTime);
                if (databaseInfos.Count == 0) return count;
                var typeFullName = MongodbServerMaintainceCenter.GetTypeFullName(databasePrefix);
                var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);
                var statTimeColumn = columnDescriptions.FirstOrDefault(c => c.IsTimeColumn);
                if (statTimeColumn == null) return 0;
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
                foreach (var databaseInfo in databaseInfos)
                {
                    var server = CreateSlaveMongoServer(typeFullName);
                    var database = server.GetDatabase(databaseInfo.DatabaseName);
                    var collection = database.GetCollection(tableName);
                    count += Convert.ToInt32(collection.Count(query));
                }
                return count;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "GetDataCount", ex.Message);
                throw;
            }
        }

        #endregion

        #region Category

        private SubCategory GetSubCategory(MongodbDatabaseDescription description, ServerInfo serverInfo)
        {
            try
            {
                var subCategory = new SubCategory()
                {
                    DatabasePrefix = description.DatabasePrefix,
                    DisplayName = description.DisplayName,
                    Name = description.Name,
                    TableNames = new List<string>(),
                    TypeFullName = description.TypeFullName,
                };

                subCategory.TableNames = serverInfo.Databases
                    .Where(database => database.DatabaseName.StartsWith(description.DatabasePrefix))
                    .SelectMany(database => database.Collections).Select(collection => collection.CollectionName).Distinct().ToList();

                return subCategory;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "GetSubCategory", ex.Message);
                throw;
            }
        }

        public List<Category> GetCategoryData()
        {
            try
            {
                var data = new Dictionary<string, Category>();

                foreach (var serverInfo in MongodbServerMaintainceCenter.GetServerInfos().Select(item => item.Value).ToList())
                {
                    var descriptions = serverInfo.Descriptions;
                    foreach (var description in descriptions)
                    {
                        if (string.IsNullOrEmpty(description.CategoryName) || string.IsNullOrEmpty(description.Name)) continue;

                        if (!data.ContainsKey(description.CategoryName))
                        {
                            data.Add(description.CategoryName, new Category
                            {
                                Name = description.CategoryName,
                                SubCategoryList = new List<SubCategory>(),
                            });
                            data[description.CategoryName].SubCategoryList.Add(GetSubCategory(description, serverInfo));
                        }
                        else
                        {
                            if (!data[description.CategoryName].SubCategoryList.Exists(sub => sub.Name == description.Name))
                            {
                                data[description.CategoryName].SubCategoryList.Add(GetSubCategory(description, serverInfo));
                            }
                        }
                    }
                }

                return data.Select(item => item.Value).ToList();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Slave", "GetCategoryData", ex.Message);
                throw;
            }
        }

        #endregion
    }
}
