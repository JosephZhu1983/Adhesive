using System;
using System.Collections.Generic;
using System.Threading;
using Adhesive.AppInfoCenter;
using Adhesive.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Adhesive.ComponentPerformance.Core
{
    internal class Collector
    {
        private static List<Timer> timers = new List<Timer>();
        private static MongoServer server;
        private static Dictionary<string, Dictionary<string, long>> dataCache = new Dictionary<string, Dictionary<string, long>>();

        private static void HandleTotalValue(string key, ComponentConfiguration config, Dictionary<string, object> rawData)
        {
            if (!dataCache.ContainsKey(key))
            {
                var cache = new Dictionary<string, long>();
                foreach (var k in new List<string>(rawData.Keys))
                {
                    if (config.ComponentItems.ContainsKey(k))
                    {
                        var configItem = config.ComponentItems[k];
                        if (configItem.ItemValueType == ItemValueType.TotalValue)
                        {
                            var current = Convert.ToInt64(rawData[k]);
                            cache.Add(k, current);
                            rawData[k] = 0;
                        }
                    }
                }
                dataCache.Add(key, cache);
            }
            else
            {
                var cache = dataCache[key];
                foreach (var k in new List<string>(rawData.Keys))
                {   
                    if (config.ComponentItems.ContainsKey(k))
                    {
                        var configItem = config.ComponentItems[k];
                        if (configItem.ItemValueType == ItemValueType.TotalValue)
                        {
                            var current = Convert.ToInt64(rawData[k]);
                            if (cache.ContainsKey(k))
                            {
                                rawData[k] = Math.Max(current - cache[k], 0);
                                cache[k] = current;
                            }
                            else
                            {
                                cache.Add(k, current);
                                rawData[k] = 0;
                            }
                        }
                    }
                }
            }
        }

        private static void HandleValue(string key, ComponentConfiguration config, Dictionary<string, object> data)
        {
            HandleTotalValue(key, config, data);
            foreach (var dataItem in data)
            {
                var itemKey = dataItem.Key;
                if (config.ComponentItems.ContainsKey(itemKey))
                {
                    var configItem = config.ComponentItems[itemKey];
                    var db = server.GetDatabase(key);
                    var collection = db.GetCollection(itemKey);
                    var doc = new BsonDocument().Add("V", BsonValue.Create(dataItem.Value));
                    doc.SetDocumentId(DateTime.Now.ToUniversalTime());
                    if (configItem.ItemValueType == ItemValueType.TextValue)
                    {
                        collection.RemoveAll();
                        collection.Insert(doc);
                    }
                    else if (configItem.ItemValueType == ItemValueType.ExpressionValue)
                    {

                    }
                    else
                    {
                        collection.Insert(doc);
                    }
                }
            }
        }

        private static void StartItem(KeyValuePair<string, ComponentConfiguration> item)
        {
            var value = item.Value;
            var key = item.Key;
            if (value != null && !string.IsNullOrWhiteSpace(value.Url))
            {
                var db = server.GetDatabase(key);
                foreach (var citem in value.ComponentItems)
                {
                    while (!db.CollectionExists(citem.Key))
                    {
                        var options = CollectionOptions
                            .SetCapped(true)
                            .SetAutoIndexId(true)
                            .SetMaxSize(1024 * 1024 * 100)
                            .SetMaxDocuments(1000000);
                        db.CreateCollection(citem.Key, options);
                        Thread.Sleep(1000);
                    }
                }
                var timer = new Timer(state =>
                {
                    try
                    {
                        var a = state as Tuple<string,ComponentConfiguration>;
                        var config = a.Item2;
                        if (config != null)
                        {
                            var data = config.GetCurrentStatus();
                            if (data != null && data.Count >0)
                                HandleValue(a.Item1, config, data);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Handle("收集器" + key);
                    }
                }, new Tuple<string,ComponentConfiguration>(key, value) , TimeSpan.Zero, value.CollectSpan);
                LocalLoggingService.Info(string.Format("收集器成功初始化：{0}", key));
                timers.Add(timer);
            }
        }

        internal static void Start()
        {
            var rootconfig = Configuration.GetConfig();
            server = MongoServer.Create(rootconfig.DatabaseUrlForComponentPerformanceMaster);

            if (rootconfig.MongodbList != null)
            {
                foreach (var item in rootconfig.MongodbList)
                {
                    StartItem(item);
                }
            }

            if (rootconfig.MemcachedList != null)
            {
                foreach (var item in rootconfig.MemcachedList)
                {
                    StartItem(item);
                }
            }

            if (rootconfig.KTList != null)
            {
                foreach (var item in rootconfig.KTList)
                {
                    StartItem(item);
                }
            }

            if (rootconfig.RedisList != null)
            {
                foreach (var item in rootconfig.RedisList)
                {
                    StartItem(item);
                }
            }
        }

        internal static void Reset()
        {
            foreach (var timer in timers)
            {
                timer.Dispose();
            }
            timers.Clear();
            Start();
        }
    }
}
