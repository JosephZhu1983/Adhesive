using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Adhesive.AppInfoCenter;
using Adhesive.GeneralPerformance.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Adhesive.GeneralPerformance.Core
{
    internal class GreneralPerformanceCollector
    {
        private Thread thread;
        private MongoServer server;

        private readonly string GroupByMachineIP = "M";
        private readonly string GroupByPageName = "P";
        private Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> successRequestCountCache = new Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>>();
        private Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> failedRequestCountCache = new Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>>();
        private Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> averageRequestExecutionTimeCache = new Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>>();

        private ItemConfigurationEntity config;
        internal GreneralPerformanceCollector(ItemConfigurationEntity config)
        {
            this.config = config;
        }

        internal void Start()
        {
            server = MongoServer.Create(Configuration.GetConfig().DatabaseUrlForGeneralPerformanceMaster);
            thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        HandleData(successRequestCountCache, "Success", "Sum");
                        HandleData(failedRequestCountCache, "Failed", "Sum");
                        HandleData(averageRequestExecutionTimeCache, "Time", "Avg");

                    }
                    catch (Exception ex)
                    {
                        ex.Handle();
                    }
                    finally
                    {
                        Thread.Sleep(config.CollectSpan);
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void HandleData(Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> data, string name, string type)
        {
            foreach (var appKey in new List<string>(data.Keys))
            {
                var app = data[appKey];
                foreach (var itemKey in new List<string>(app.Keys))
                {
                    var item = app[itemKey];
                    var db = server.GetDatabase(GetDatabaseName(name, appKey, itemKey));

                    foreach (var subItemKey in new List<string>(item.Keys))
                    {
                        var subItem = item[subItemKey];

                        Int32 value = 0;
                        if (subItem != null && subItem.Count > 0)
                        {
                            lock (subItem)
                            {
                                switch (type)
                                {
                                    case "Min":
                                        value = subItem.Min();
                                        break;
                                    case "Max":
                                        value = subItem.Max();
                                        break;
                                    case "Avg":
                                        value = Convert.ToInt32(subItem.Average());
                                        break;
                                    case "Sum":
                                        value = subItem.Sum();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        var colName = subItemKey;
                        var col = db.GetCollection(colName);
                        var options = CollectionOptions
                              .SetCapped(true)
                              .SetAutoIndexId(true)
                              .SetMaxSize(1024 * 1024 * 100)
                              .SetMaxDocuments(1000000);
                        
                        try
                        {
                            while (!db.CollectionExists(colName))
                            {
                                db.CreateCollection(colName, options);
                                Thread.Sleep(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Handle("收集器创建表失败：" + colName);
                            continue;
                        }
                        //AppInfoCenterService.LoggingService.Info(string.Format("收集器成功初始化表：{0} 参数：{1}", col.FullName, options.ToString()));

                        try
                        {
                            var doc = new BsonDocument().Add("V", BsonValue.Create(value));
                            doc.SetDocumentId(DateTime.Now.ToUniversalTime());
                            col.Insert(doc);
                        }
                        catch (Exception ex)
                        {
                            ex.Handle();
                        }
                        finally
                        {
                            lock (subItem)
                                subItem.Clear();
                        }
                    }
                }
            }
        }

        internal void AddData(GeneralPerformanceInfo info)
        {
            InternalAddData(successRequestCountCache, info.AppName, info.MachineIP, info.PageName, info.SuccessRequestCount);
            InternalAddData(failedRequestCountCache, info.AppName, info.MachineIP, info.PageName, info.FailedRequestCount);
            InternalAddData(averageRequestExecutionTimeCache, info.AppName, info.MachineIP, info.PageName, info.AverageRequestExecutionTime);
        }

        private void InternalAddData(Dictionary<string, Dictionary<string, Dictionary<string, List<int>>>> cache, string appName, string machineIP, string pageName, int value)
        {
            machineIP = machineIP.Trim().Replace(" ", "").Replace("/", "_").Replace(".", "_").TrimStart('_');
            pageName = pageName.Trim().Replace(" ", "").Replace("/", "_").Replace(".", "_").TrimStart('_');

            if (!cache.ContainsKey(appName))
            {
                lock (cache)
                {
                    if (!cache.ContainsKey(appName))
                    {
                        cache.Add(appName, new Dictionary<string, Dictionary<string, List<int>>>());
                        cache[appName].Add(GroupByMachineIP, new Dictionary<string, List<int>>());
                        cache[appName].Add(GroupByPageName, new Dictionary<string, List<int>>());
                    }
                }
            }

            var cacheForApp = cache[appName];
            if (!string.IsNullOrWhiteSpace(machineIP) && cacheForApp.ContainsKey(GroupByMachineIP))
            {
                var cacheForMachine = cacheForApp[GroupByMachineIP];
                lock (cacheForMachine)
                {
                    if (!cacheForMachine.ContainsKey(machineIP))
                    {
                        cacheForMachine.Add(machineIP, new List<int>());
                    }
                }

                lock (cacheForMachine[machineIP])
                    cacheForMachine[machineIP].Add(value);
            }
            if (!string.IsNullOrWhiteSpace(pageName) && cacheForApp.ContainsKey(GroupByPageName))
            {
                var cacheForPage = cacheForApp[GroupByPageName];
                lock (cacheForPage)
                {
                    if (!cacheForPage.ContainsKey(pageName))
                    {
                        cacheForPage.Add(pageName, new List<int>());
                    }
                }

                lock (cacheForPage[pageName])
                    cacheForPage[pageName].Add(value);
            }

        }

        private string GetDatabaseName(string category, string appName, string groupName)
        {
            return string.Format("{0}__{1}__{2}__{3}", config.Prefix, category, appName, groupName).Replace(".", "");
        }
    }
}
