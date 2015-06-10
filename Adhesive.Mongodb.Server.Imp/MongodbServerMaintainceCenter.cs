
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Adhesive.DistributedComponentClient.Memcached;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Adhesive.Common;

namespace Adhesive.Mongodb.Server.Imp
{
    public class MongodbServerMaintainceCenter
    {
        private static Dictionary<MongodbServerUrl, ServerInfo> servers = new Dictionary<MongodbServerUrl, ServerInfo>();
        private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        internal static MemcachedClient MemcachedClient { get; private set; }
        private static bool fristLoaded = false;

        #region Internal

        internal static Dictionary<MongodbServerUrl, ServerInfo> GetServerInfos()
        {
            locker.EnterReadLock();
            var rtn = servers;
            locker.ExitReadLock();
            return rtn;
        }

        internal static List<DatabaseInfo> GetDatabaseInfos(string databasePrefix, DateTime beginTime, DateTime endTime)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = servers.Select(item => item.Value).Where(item => item.Databases != null)
                      .SelectMany(s => s.Databases).Where(database => database != null && database.DatabasePrefix == databasePrefix
                          && database.DatabaseDate.ToLocalTime() >= new DateTime(beginTime.Year, beginTime.Month, 1) && database.DatabaseDate.ToLocalTime() <= endTime).OrderByDescending(d => d.DatabaseDate).ToList();
                if (rtn == null || rtn.Count == 0)
                    LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetDatabaseInfos",
                        string.Format("没取到数据，参数：{0}，{1}，{2}", databasePrefix, beginTime, endTime));
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetDatabaseInfos", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static DatabaseInfo GetDatabaseInfo(string databaseName)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = servers.Select(item => item.Value).Where(item => item.Databases != null)
                      .SelectMany(s => s.Databases).Where(database => database != null && database.DatabaseName == databaseName).SingleOrDefault();

                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2}", "MongodbServerMaintainceCenter", "GetDatabaseInfo",ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static List<DatabaseInfo> GetDatabaseInfos(string databasePrefix)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = servers.Select(item => item.Value).Where(item => item.Databases != null)
                      .SelectMany(s => s.Databases).Where(database => database != null && database.DatabasePrefix == databasePrefix).OrderByDescending(d => d.DatabaseDate.ToLocalTime()).ToList();
                if (rtn == null || rtn.Count == 0)
                    LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetDatabaseInfos",
                        string.Format("没取到数据，参数：{0}", databasePrefix));
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetDatabaseInfos", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static List<DatabaseInfo> GetDatabaseInfos(DateTime beginTime, DateTime endTime)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = servers.Select(item => item.Value).Where(item => item.Databases != null)
                          .SelectMany(s => s.Databases).Where(database => database != null && database.DatabaseDate.ToLocalTime() >= beginTime && database.DatabaseDate.ToLocalTime() < endTime).OrderByDescending(d => d.DatabaseDate).ToList();
                if (rtn == null || rtn.Count == 0)
                    LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetDatabaseInfos",
                        string.Format("没取到数据，参数：{0}，{1}", beginTime, endTime));
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetDatabaseInfos", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static List<MongodbColumnDescription> GetMongodbColumnDescriptions(string databasePrefix)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = servers.Select(item => item.Value).Where(item => item.Descriptions != null)
                                      .SelectMany(s => s.Descriptions).Where(description => description != null && description.DatabasePrefix == databasePrefix)
                                      .SelectMany(desc => desc.MongodbColumnDescriptionList).ToList();
                if (rtn == null || rtn.Count == 0)
                    LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetMongodbColumnDescriptions",
                        string.Format("没取到数据，参数：{0}", databasePrefix));
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetMongodbColumnDescriptions", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static MongodbColumnDescription GetMongodbColumnDescription(string databasePrefix, string columnName)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = GetMongodbColumnDescriptions(databasePrefix).FirstOrDefault(c => c.ColumnName == columnName);
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetMongodbColumnDescription", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static List<MongodbEnumColumnDescription> GetMongodbEnumColumnDescriptions(string databasePrefix)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = servers.Select(item => item.Value).Where(item => item.Descriptions != null)
                                      .SelectMany(s => s.Descriptions).Where(description => description != null && description.DatabasePrefix == databasePrefix)
                                      .SelectMany(desc => desc.MongodbEnumColumnDescriptionList).ToList();
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetMongodbEnumColumnDescriptions", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static MongodbEnumColumnDescription GetMongodbEnumColumnDescription(string databasePrefix, string columnName)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = GetMongodbEnumColumnDescriptions(databasePrefix).FirstOrDefault(c => c.Name == columnName);
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetMongodbEnumColumnDescription", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal static string GetTypeFullName(string databasePrefix)
        {
            locker.EnterReadLock();
            try
            {
                var rtn = servers.Select(item => item.Value).Where(item => item.Descriptions != null)
                        .SelectMany(s => s.Descriptions).FirstOrDefault(description => description != null && description.DatabasePrefix == databasePrefix).TypeFullName;
                if (string.IsNullOrEmpty(rtn))
                    LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "GetTypeFullName",
                        string.Format("没取到数据，参数：{0}", databasePrefix));
                return rtn;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "", ex.Message);
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        #endregion

        private static void Maintainance(object o)
        {

            try
            {
                var url = o as MongodbServerUrl;

                LocalLoggingService.Info("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "服务器",
                    string.Format("对服务器 '{0}' 开始一次维护", url.Name));

                Stopwatch sw = Stopwatch.StartNew();

                ServerInfo oldServerInfo = null;
                locker.EnterReadLock();
                if (servers.ContainsKey(url))
                    oldServerInfo = servers[url];
                locker.ExitReadLock();

                var serverInfo = new ServerInfo();
                serverInfo.Url = url;
                serverInfo.Databases = new List<DatabaseInfo>();
                var server = MongoServer.Create(url.Master);

                //元数据
                var metaDatabase = server.GetDatabase(MongodbServerConfiguration.MetaDataDbName);
                var metaCollectionNames = metaDatabase.GetCollectionNames().Where(name => !name.Contains("system.index")
                    && !name.Contains("$")).ToList();
                var descriptions = new List<MongodbDatabaseDescription>();

                foreach (var metaCollectionName in metaCollectionNames)
                {
                    try
                    {
                        var metaCollection = metaDatabase.GetCollection(metaCollectionName);
                        descriptions.Add(metaCollection.FindOneAs<MongodbDatabaseDescription>());
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter",
                            "获取元数据出错",ex.Message);
                    }
                }
                serverInfo.Descriptions = descriptions;

                //获取所有的数据库
                var databaseNames = server.GetDatabaseNames().Where(name =>
                {
                    var categories = descriptions.Select(description => description.DatabasePrefix).Distinct();
                    foreach (var categoryName in categories)
                    {
                        if (name.StartsWith(categoryName))
                            return true;
                    }
                    return false;
                }).ToList();

                Parallel.ForEach(databaseNames, databaseName =>
                {
                    var swdb = Stopwatch.StartNew();

                    if (!databaseName.Contains("__"))
                    {
                        LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "数据库",
                            string.Format("数据库名 '{0}' 不包含__", databaseName));
                        return;
                    }

                    var datepart = databaseName.Substring(databaseName.LastIndexOf("__") + 2);
                    if (datepart.Length != 6)
                    {
                        LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "数据库",
                            string.Format("数据名 '{0}' 日期部分长度不为6", databaseName));
                        return;
                    }

                    try
                    {
                        if (fristLoaded && oldServerInfo != null && oldServerInfo.Databases.Exists(db => db.DatabaseName == databaseName) && datepart != DateTime.Now.ToString("yyyyMM"))
                        {
                            var oldDb = oldServerInfo.Databases.SingleOrDefault(db => db.DatabaseName == databaseName);
                            if (oldDb != null)
                            {
                                if (!serverInfo.Databases.Contains(oldDb))
                                {
                                    serverInfo.Databases.Add(oldDb);
                                }
                                LocalLoggingService.Debug("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "数据库",
                                    string.Format("对数据库 '{0}' 完成一次维护，直接使用现有数据.oldServerInfo", databaseName));
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string reason = "";
                        if (oldServerInfo.Databases == null)
                            reason = "oldServerInfo.Databases == null";
                        else if (oldServerInfo.Databases.Exists(d => d == null))
                            reason = "oldServerInfo.Databases.Exists(d => d == null)";
                        else
                            reason = "other";
                        LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter",
                            string.Format("获取已有数据库 {0} 出错：{1}", databaseName, reason), ex.ToString());
                    }

                    var database = server.GetDatabase(databaseName);

                    var databaseInfo = new DatabaseInfo();
                    databaseInfo.DatabaseName = databaseName;

                    try
                    {
                        var databaseStatusResult = database.GetStats();//获取数据库信息
                        if (databaseStatusResult != null)
                        {
                            var databaseStatus = new DatabaseStatus
                            {
                                AverageObjectSize = databaseStatusResult.AverageObjectSize,
                                CollectionCount = databaseStatusResult.CollectionCount,
                                DataSize = databaseStatusResult.DataSize,
                                FileSize = databaseStatusResult.FileSize,
                                IndexSize = databaseStatusResult.IndexSize,
                                IndexCount = databaseStatusResult.IndexCount,
                                ExtentCount = databaseStatusResult.ExtentCount,
                                ObjectCount = databaseStatusResult.ObjectCount,
                                StorageSize = databaseStatusResult.StorageSize,
                            };
                            databaseInfo.DatabaseStatus = databaseStatus;
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter",
                            string.Format("获取数据库 '{0}' 状态出错", databaseName), ex.ToString());
                    }

                    var prefixPart = databaseName.Substring(0, databaseName.LastIndexOf("__"));
                    databaseInfo.DatabasePrefix = prefixPart;
                    var date = DateTime.MinValue;
                    if (DateTime.TryParse(string.Format("{0}/{1}/{2}", datepart.Substring(0, 4), datepart.Substring(4, 2), "01"), out date))
                    {
                        databaseInfo.DatabaseDate = date;
                    }
                    else
                    {
                        LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter",
                            string.Format("数据库名 '{0}' 日期部分解析错误", databaseName));
                        return;
                    }

                    var description = descriptions.Where(d => d.DatabasePrefix == prefixPart).FirstOrDefault();
                    if (description == null)
                    {
                        LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "数据库",
                            string.Format("没有取到数据描述 '{0}'", prefixPart));
                        return;
                    }

                    databaseInfo.Collections = new List<CollectionInfo>();

                    var expireDays = description.ExpireDays;
                    if (expireDays > 0)
                    {
                        if (databaseInfo.DatabaseDate.AddMonths(1).AddDays(-1).AddDays(expireDays) < DateTime.Now && datepart != DateTime.Now.ToString("yyyyMM"))
                        {
                            database.Drop();
                            LocalLoggingService.Info("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "数据库",
                                string.Format("清除历史数据-删除库：{0}", databaseName));
                            return;
                        }
                    }

                    var collectionNames = database.GetCollectionNames().Where(name => !name.Contains("system.index")
                        && !name.Contains("$") && !name.Contains("__")).ToList();
                    foreach (var collectionName in collectionNames)
                    {
                        var swcoll = Stopwatch.StartNew();
                        try
                        {
                            var collection = database.GetCollection(collectionName);
                            //if (expireDays > 0)
                            //{
                            //    var statTimeColumn = description.MongodbColumnDescriptionList.FirstOrDefault(c => c.IsTimeColumn);
                            //    if (statTimeColumn != null)
                            //    {
                            //        var query = Query.LT(statTimeColumn.ColumnName, DateTime.Now.AddDays(-expireDays));
                            //        collection.Remove(query);
                            //        if (collection.Count() == 0)
                            //        {
                            //            collection.Drop();
                            //            LocalLoggingService.Info("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter 清除历史数据-无数据删除表", databaseName, collectionName);
                            //            continue;
                            //        }
                            //    }
                            //}

                            var collectionInfo = new CollectionInfo();
                            collectionInfo.CollectionName = collectionName;
                            collectionInfo.ListFilterColumns = new List<ListFilterColumnInfo>();
                            collectionInfo.CascadeFilterColumns = new List<CascadeFilterColumnInfo>();
                            collectionInfo.TextboxFilterColumns = new List<TextboxFilterColumnInfo>();

                            var collectionStatus = new CollectionStatus
                            {
                                IndexStatusList = new List<IndexStatus>(),
                            };
                            CollectionStatsResult collectionStatusResult = null;

                            try
                            {
                                if (collection.Count() > 0)
                                {
                                    collectionStatusResult = collection.GetStats();
                                    collectionStatus.AverageObjectSize = collectionStatusResult.AverageObjectSize;
                                    collectionStatus.DataSize = collectionStatusResult.DataSize;
                                    collectionStatus.StorageSize = collectionStatusResult.StorageSize;
                                    collectionStatus.LastExtentSize = collectionStatusResult.LastExtentSize;
                                    collectionStatus.Namespace = collectionStatusResult.Namespace;
                                    collectionStatus.ExtentCount = collectionStatusResult.ExtentCount;
                                    collectionStatus.Flags = collectionStatusResult.Flags;
                                    collectionStatus.IndexCount = collectionStatusResult.IndexCount;
                                    collectionStatus.ObjectCount = collectionStatusResult.ObjectCount;
                                    collectionStatus.PaddingFactor = collectionStatusResult.PaddingFactor;
                                    collectionStatus.TotalIndexSize = collectionStatusResult.TotalIndexSize;
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter 获取表状态出错",
                                    string.Format("库名:{0} 表名:{1}", databaseName, collectionName), ex.ToString());
                            }

                            var indexes = collection.GetIndexes();
                            collectionStatus.IndexStatusList = indexes.Select(i => new IndexStatus
                            {
                                Name = i.Name,
                                Namespace = i.Namespace,
                                Unique = i.IsUnique,
                            }).ToList();

                            collectionStatus.IndexStatusList.ForEach(i =>
                            {
                                if (collectionStatusResult != null && collectionStatusResult.IndexSizes.ContainsKey(i.Name))
                                    i.Size = collectionStatusResult.IndexSizes[i.Name];
                            });
                            collectionInfo.CollectionStatus = collectionStatus;

                            var indexColumnDescriptions = description.MongodbColumnDescriptionList.Where(c => c.MongodbIndexOption != MongodbIndexOption.None).ToList();
                            foreach (var indexColumnDescription in indexColumnDescriptions)
                            {
                                try
                                {
                                    switch (indexColumnDescription.MongodbIndexOption)
                                    {
                                        case MongodbIndexOption.Ascending:
                                            {
                                                collection.EnsureIndex(IndexKeys.Ascending(indexColumnDescription.ColumnName), IndexOptions.SetBackground(true));
                                                break;
                                            }
                                        case MongodbIndexOption.Descending:
                                            {
                                                collection.EnsureIndex(IndexKeys.Descending(indexColumnDescription.ColumnName), IndexOptions.SetBackground(true));
                                                break;
                                            }
                                        case MongodbIndexOption.AscendingAndUnique:
                                            {
                                                collection.EnsureIndex(IndexKeys.Ascending(indexColumnDescription.ColumnName), IndexOptions.SetBackground(true).SetUnique(true).SetDropDups(true));
                                                break;
                                            }
                                        case MongodbIndexOption.DescendingAndUnique:
                                            {
                                                collection.EnsureIndex(IndexKeys.Descending(indexColumnDescription.ColumnName), IndexOptions.SetBackground(true).SetUnique(true).SetDropDups(true));
                                                break;
                                            }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "创建索引出错", ex.Message);
                                }
                            }

                            collectionInfo.CollectionStatus.LastEnsureIndexTime = DateTime.Now;

                            //加载数据库中的元数据
                            var metaCollection = database.GetCollection("Metadata__" + collectionInfo.CollectionName);
                            var meta = metaCollection.FindOneAs<CollectionMetadata>();

                            if (meta == null)
                            {
                                meta = new CollectionMetadata
                                {
                                    CollectionName = collectionInfo.CollectionName,
                                    ListFilterColumns = new List<ListFilterColumnInfo>(),
                                    CascadeFilterColumns = new List<CascadeFilterColumnInfo>(),
                                    TextboxFilterColumns = new List<TextboxFilterColumnInfo>(),
                                };
                            }
                            var textboxColumnDescriptions = description.MongodbColumnDescriptionList.Where(c => c.MongodbFilterOption == MongodbFilterOption.TextBoxFilter).ToList();

                            foreach (var textboxColumnDescription in textboxColumnDescriptions)
                            {
                                if (!collectionInfo.CollectionStatus.IndexStatusList.Exists(index => index.Name.Contains(textboxColumnDescription.ColumnName)))
                                    continue;
                                var textboxColumnInfo = new TextboxFilterColumnInfo();
                                textboxColumnInfo.ColumnName = textboxColumnDescription.ColumnName;
                                collectionInfo.TextboxFilterColumns.Add(textboxColumnInfo);
                                if (meta.TextboxFilterColumns.Count(c => c.ColumnName == textboxColumnInfo.ColumnName) == 0)
                                    meta.TextboxFilterColumns.Add(textboxColumnInfo);
                            }

                            var filterColumnDescriptions = description.MongodbColumnDescriptionList.Where
                                (c => c.MongodbFilterOption == MongodbFilterOption.DropDownListFilter
                                || c.MongodbFilterOption == MongodbFilterOption.CheckBoxListFilter).ToList();

                            foreach (var filterColumnDescription in filterColumnDescriptions)
                            {
                                try
                                {
                                    if (!collectionInfo.CollectionStatus.IndexStatusList.Exists(index => index.Name.Contains(filterColumnDescription.ColumnName)))
                                        continue;

                                    var filterColumnInfo = new ListFilterColumnInfo();
                                    filterColumnInfo.ColumnName = filterColumnDescription.ColumnName;
                                    filterColumnInfo.DistinctValues = new List<ItemPair>();

                                    if (oldServerInfo != null && oldServerInfo.Databases != null)
                                    {
                                        var oldDatabase = oldServerInfo.Databases.FirstOrDefault(d => d != null && d.DatabaseName == databaseInfo.DatabaseName);
                                        if (oldDatabase != null && oldDatabase.Collections != null)
                                        {
                                            var oldCollection = oldDatabase.Collections.FirstOrDefault(d => d != null && d.CollectionName == collectionInfo.CollectionName);
                                            if (oldCollection != null && oldCollection.ListFilterColumns != null)
                                            {
                                                var oldColumn = oldCollection.ListFilterColumns.FirstOrDefault(d => d != null && d.ColumnName == filterColumnInfo.ColumnName);
                                                if (oldColumn != null)
                                                {
                                                    filterColumnInfo.DistinctValues = oldColumn.DistinctValues;
                                                }
                                            }
                                        }
                                    }

                                    var column = meta.ListFilterColumns.SingleOrDefault(c => c.ColumnName == filterColumnDescription.ColumnName);
                                    if (column != null)
                                    {
                                        foreach (var value in filterColumnInfo.DistinctValues)
                                        {
                                            if (column.DistinctValues.Count(v => string.Equals(v.Value.ToString(), value.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)) == 0)
                                            {
                                                LocalLoggingService.Debug("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "列",
                                                string.Format("添加新过滤索引项 {0} 到元数据 {1}.{2}.{3}", value.Value.ToString(), databaseName, collectionName, column.ColumnName));
                                                column.DistinctValues.Add(value);
                                            }
                                        }
                                        filterColumnInfo.DistinctValues = column.DistinctValues;
                                    }
                                    else
                                    {
                                        meta.ListFilterColumns.Add(filterColumnInfo);
                                    }
                                    collectionInfo.ListFilterColumns.Add(filterColumnInfo);
                                }
                                catch (Exception ex)
                                {
                                    LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "列",
                                        "创建过滤数据出错", ex.Message);
                                }
                            }

                            var cascadeFilterColumnDescriptions = description.MongodbColumnDescriptionList.Where(c => c.MongodbCascadeFilterOption != MongodbCascadeFilterOption.None).ToList();
                            foreach (var cascadeFilterColumnDescription in cascadeFilterColumnDescriptions)
                            {
                                try
                                {
                                    if (!collectionInfo.CollectionStatus.IndexStatusList.Exists(index => index.Name.Contains(cascadeFilterColumnDescription.ColumnName)))
                                        continue;

                                    var filterColumnInfo = new CascadeFilterColumnInfo();
                                    filterColumnInfo.ColumnName = cascadeFilterColumnDescription.ColumnName;
                                    filterColumnInfo.DistinctValues = new List<string>();
                                    if (oldServerInfo != null && oldServerInfo.Databases != null)
                                    {
                                        var oldDatabase = oldServerInfo.Databases.FirstOrDefault(d => d != null && d.DatabaseName == databaseInfo.DatabaseName);
                                        if (oldDatabase != null && oldDatabase.Collections != null)
                                        {
                                            var oldCollection = oldDatabase.Collections.FirstOrDefault(d => d != null && d.CollectionName == collectionInfo.CollectionName);
                                            if (oldCollection != null && oldCollection.CascadeFilterColumns != null)
                                            {
                                                var oldColumn = oldCollection.CascadeFilterColumns.FirstOrDefault(d => d != null && d.ColumnName == filterColumnInfo.ColumnName);
                                                if (oldColumn != null)
                                                {
                                                    filterColumnInfo.DistinctValues = oldColumn.DistinctValues;
                                                }
                                            }
                                        }
                                    }

                                    var column = meta.CascadeFilterColumns.SingleOrDefault(c => c.ColumnName == cascadeFilterColumnDescription.ColumnName);
                                    if (column != null)
                                    {
                                        foreach (var value in filterColumnInfo.DistinctValues)
                                        {
                                            if (column.DistinctValues.Count(v => string.Equals(v, value, StringComparison.InvariantCultureIgnoreCase)) == 0)
                                            {
                                                LocalLoggingService.Debug("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "列",
                                                string.Format("添加新级联过滤索引项 {0} 到元数据 {1}.{2}.{3}", value, databaseName, collectionName, column.ColumnName));
                                                column.DistinctValues.Add(value);
                                            }
                                        }
                                        filterColumnInfo.DistinctValues = column.DistinctValues;
                                    }
                                    else
                                    {
                                        meta.CascadeFilterColumns.Add(filterColumnInfo);
                                    }
                                    collectionInfo.CascadeFilterColumns.Add(filterColumnInfo);
                                }
                                catch (Exception ex)
                                {
                                    LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "列",
                                        "创建级联过滤数据出错", ex.Message);
                                }
                            }

                            try
                            {
                                metaCollection.Save(meta);
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "表",
                                   "保存表元数据出错", ex.Message);
                            }
                            databaseInfo.Collections.Add(collectionInfo);
                        }
                        catch (Exception ex)
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "表",
                                "维护其它出错", ex.Message);
                        }
                    }

                    if (databaseInfo.Collections.Count == 0)
                    {
                        //database.Drop();
                        //AppInfoCenterService.LoggingService.Debug(MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "数据库",
                        //    string.Format("清除历史数据-删除库：{0}", databaseName));
                    }
                    else
                    {
                        if (!serverInfo.Databases.Contains(databaseInfo))
                        {
                            serverInfo.Databases.Add(databaseInfo);
                        }
                    }

                    LocalLoggingService.Debug("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "数据库",
                        string.Format("对数据库 '{0}' 完成一次维护，耗时 {1} 毫秒", databaseName, swdb.ElapsedMilliseconds));
                });

                LocalLoggingService.Info("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", "服务器",
                    string.Format("对服务器 '{0}' 完成一次维护，耗时 {1} 毫秒", url.Name, sw.ElapsedMilliseconds));

                locker.EnterWriteLock();
                if (servers.ContainsKey(url))
                    servers[url] = serverInfo;
                else
                    servers.Add(url, serverInfo);
                locker.ExitWriteLock();

                fristLoaded = true;

            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2}", MongodbServerConfiguration.ModuleName, "MongodbServerMaintainceCenter", ex.Message);
            }
        }

        public static void Init()
        {
            if (servers.Count == 0)
            {
                MemcachedClient = MemcachedClient.GetClient(MongodbServerConfiguration.GetConfig().MemcachedClusterName);

                BsonClassMap.RegisterClassMap<MongodbDatabaseDescription>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    cm.SetIdMember(cm.GetMemberMap("TypeFullName"));
                });

                BsonClassMap.RegisterClassMap<MongodbColumnDescription>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });

                BsonClassMap.RegisterClassMap<CollectionMetadata>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    cm.SetIdMember(cm.GetMemberMap("CollectionName"));
                });

                foreach (var url in MongodbServerConfiguration.GetConfig().MongodbServerUrls)
                {
                    var server = url.Value;
                    Maintainance(server);
                    var thread = new Thread(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(Math.Max(MongodbServerConfiguration.GetConfig().MaintainceIntervalMilliSeconds + 1, 30 * 1000));
                            Maintainance(server);
                        }
                    })
                    {
                        IsBackground = true,
                        Name = string.Format("MongodbServerMaintainanceCenter_{0}", url.Key)
                    };
                    thread.Start();
                }
            }
        }
    }
}
