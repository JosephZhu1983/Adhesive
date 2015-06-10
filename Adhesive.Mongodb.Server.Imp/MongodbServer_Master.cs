
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web.Script.Serialization;
using Adhesive.Common;
using Adhesive.MemoryQueue;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Adhesive.Mongodb.Server.Imp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = false,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        Namespace = "Adhesive.Mongodb")]
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        private static JavaScriptSerializer s = new JavaScriptSerializer();
        private static readonly Dictionary<string, IMemoryQueueService> submitDataMemoryQueueServices = new Dictionary<string, IMemoryQueueService>();
        private static readonly Dictionary<string, MongoServer> masterServerCache = new Dictionary<string, MongoServer>();

        internal static void ConfigUpdateCallbackForMaster()
        {
            LocalLoggingService.Info("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "ConfigUpdateCallbackForMaster", "配置改变，重新初始化");

            lock (submitDataMemoryQueueServices)
            {
                //释放老的线程！
                foreach (var submitDataMemoryQueueService in submitDataMemoryQueueServices)
                {
                    submitDataMemoryQueueService.Value.Dispose();
                }
                submitDataMemoryQueueServices.Clear();
            }
            lock (masterServerCache)
            {
                masterServerCache.Clear();
            }
        }

        //internal static List<MongodbServerStateInfo> GetState()
        //{
        //    lock (submitDataMemoryQueueServices)
        //    {
        //        var state = new List<MongodbServerStateInfo>() 
        //        {
        //            new MongodbServerStateInfo
        //            {
        //                MemoryQueueServiceStates = submitDataMemoryQueueServices.ToDictionary(s=>s.Key, s=>s.Value.GetState()),
        //            }
        //        };
        //        return state;
        //    }
        //}

        private static MongoServer CreateMasterMongoServer(string typeName)
        {
            if (!masterServerCache.ContainsKey(typeName))
            {
                lock (masterServerCache)
                {
                    if (!masterServerCache.ContainsKey(typeName))
                    {
                        var config = MongodbServerConfiguration.GetMongodbServerConfigurationItem(typeName);
                        if (config == null)
                        {
                            LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "CreateMasterMongoServer",
                                string.Format("没取到服务器配置项，参数：{0}", typeName));
                            return null;
                        }
                        var server = MongodbServerConfiguration.GetMongodbServerUrl(config.MongodbServerUrlName);
                        if (server == null)
                        {
                            LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "CreateMasterMongoServer",
                                string.Format("没取到服务器地址，参数：{0}", config.MongodbServerUrlName));
                            return null;
                        }
                        masterServerCache.Add(typeName, MongoServer.Create(server.Master));
                    }
                }
            }
            return masterServerCache[typeName];
        }

        protected override void InternalDispose()
        {
            base.InternalDispose();
            foreach (var item in submitDataMemoryQueueServices)
                item.Value.Dispose();
        }

        public void SubmitData(IList<MongodbData> dataList, MongodbDatabaseDescription databaseDescription)
        {
            if (dataList == null || dataList.Count == 0)
                return;

            var typeFullName = dataList.First().TypeFullName;

            try
            {
                var config = MongodbServerConfiguration.GetMongodbServerConfigurationItem(typeFullName);
                if (config == null)
                {
                    LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "SubmitData",
                        string.Format("没取到服务器配置项，参数：{0}", typeFullName));
                    return;
                }

                if (!submitDataMemoryQueueServices.ContainsKey(typeFullName))
                {
                    lock (submitDataMemoryQueueServices)
                    {
                        if (!submitDataMemoryQueueServices.ContainsKey(typeFullName))
                        {
                            var memoryQueueService = LocalServiceLocator.GetService<IMemoryQueueService>();
                            memoryQueueService.Init(new MemoryQueueServiceConfiguration(string.Format("{0}_{1}", ServiceName, typeFullName), InternalSubmitData)
                            {
                                ConsumeErrorAction = config.ConsumeErrorAction,
                                ConsumeIntervalMilliseconds = config.ConsumeIntervalMilliseconds,
                                ConsumeIntervalWhenErrorMilliseconds = config.ConsumeIntervalWhenErrorMilliseconds,
                                ConsumeItemCountInOneBatch = config.ConsumeItemCountInOneBatch,
                                ConsumeThreadCount = config.ConsumeThreadCount,
                                MaxItemCount = config.MaxItemCount,
                                NotReachBatchCountConsumeAction = config.NotReachBatchCountConsumeAction,
                                ReachMaxItemCountAction = config.ReachMaxItemCountAction,
                            });
                            submitDataMemoryQueueServices.Add(typeFullName, memoryQueueService);
                        }
                    }
                }

                if (databaseDescription != null)
                {
                    InitMongodbDatabaseDescription(typeFullName, databaseDescription);
                }

                if (config.SubmitToDatabase)
                {
                    submitDataMemoryQueueServices[typeFullName].EnqueueBatch(dataList);
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "SubmitData", ex.Message);
                throw;
            }
        }

        private void InitMongodbDatabaseDescription(string typeFullName, MongodbDatabaseDescription databaseDescription)
        {
            try
            {
                var server = CreateMasterMongoServer(typeFullName);
                if (server != null)
                {
                    var database = server.GetDatabase(MongodbServerConfiguration.MetaDataDbName);
                    var collection = database.GetCollection(databaseDescription.DatabasePrefix);
                    collection.Save(databaseDescription);
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "InitMongodbDatabaseDescription", string.Format("更新元数据到数据库出现错误，参数：{0} DatabaseName:{1}", typeFullName), ex.Message);
            }
        }

        private void HandleDistinctData(BsonDocument data, string databaseName, string collectionName)
        {
            try
            {
                var databaseInfo = MongodbServerMaintainceCenter.GetDatabaseInfo(databaseName);
                if (databaseInfo != null)
                {
                    var collectionInfo = databaseInfo.Collections.SingleOrDefault(c => c.CollectionName == collectionName);
                    if (collectionInfo != null)
                    {
                        lock (collectionInfo.CascadeFilterColumns)
                        {
                            foreach (var item in collectionInfo.CascadeFilterColumns)
                            {
                                BsonElement element;
                                data.TryGetElement(item.ColumnName, out element);
                                if (element != null)
                                {
                                    var value = element.Value.RawValue.ToString();
                                    if (item.DistinctValues.Count(v => string.Equals(v, value, StringComparison.InvariantCultureIgnoreCase)) == 0)
                                    {
                                        item.DistinctValues.Add(value);
                                    }
                                }
                            }
                        }

                        lock (collectionInfo.ListFilterColumns)
                        {
                            foreach (var item in collectionInfo.ListFilterColumns)
                            {
                                var levels = item.ColumnName.Split('.');
                                BsonDocument doc = data;
                                BsonElement element;
                                string name = "";
                                if (levels.Length > 1)
                                {
                                    name = levels[levels.Length - 1];
                                    for (int i = 0; i < levels.Length - 1; i++)
                                    {
                                        doc.TryGetElement(levels[i], out element);
                                        if (element != null && element.Value != null)
                                            doc = element.Value.AsBsonDocument;
                                    }
                                }
                                else
                                {
                                    name = item.ColumnName;
                                }

                                if (doc != null)
                                {
                                    doc.TryGetElement(name, out element);
                                    if (element != null)
                                    {
                                        var value = element.Value.RawValue.ToString();
                                        var enumColumnDescription = MongodbServerMaintainceCenter.GetMongodbEnumColumnDescription(databaseInfo.DatabasePrefix, item.ColumnName);
                                        if (enumColumnDescription != null)
                                        {
                                            if (item.DistinctValues.Count(v => Convert.ToInt16(v.Value) == Convert.ToInt16(value)) == 0)
                                            {
                                                item.DistinctValues.Add(new ItemPair
                                                {
                                                    Name = enumColumnDescription.EnumItems.FirstOrDefault(enumItem => value == enumItem.Key).Value,
                                                    Value = Convert.ToInt16(value),
                                                });
                                            }
                                        }
                                        else
                                        {
                                            if (item.DistinctValues.Count(v => string.Equals(v.Value.ToString(), value, StringComparison.InvariantCultureIgnoreCase)) == 0)
                                            {
                                                item.DistinctValues.Add(new ItemPair
                                                {
                                                    Name = value.ToString(),
                                                    Value = value.ToString(),
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "HandleDistinctData", "处理索引数据出现错误", ex.Message);
            }
        }

        private void InternalSubmitData(IList<object> items)
        {
            var mongodbDataList = items.Cast<MongodbData>().ToList();
            var typeFullName = mongodbDataList.First().TypeFullName;

            var sw = Stopwatch.StartNew();

            var server = CreateMasterMongoServer(typeFullName);
            if (server != null)
            {
                mongodbDataList.ForEach(item =>
                {
                    try
                    {
                        var database = server.GetDatabase(item.DatabaseName);
                        var collection = database.GetCollection(item.TableName);
                        var documentList = new List<BsonDocument>();

                        var dic = s.DeserializeObject(item.Data) as IDictionary;
                        var document = new BsonDocument().Add(dic);
                        HandleDistinctData(document, item.DatabaseName, item.TableName);
                        collection.Insert(document);
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServer_Master", "InternalSubmitData", string.Format("写入数据出现错误，参数：{0}", typeFullName), ex.Message);
                    }
                });

                //LocalLoggingService.Debug("Mongodb服务端成功服务提交 {0} 条数据到数据库，类型是 '{1}'，耗时 {2} 毫秒", mongodbDataList.Count.ToString(), typeFullName, sw.ElapsedMilliseconds.ToString());
            }

        }
    }
}
