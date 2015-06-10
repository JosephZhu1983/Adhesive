using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Adhesive.AppInfoCenter;
using Adhesive.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Adhesive.ComponentPerformance.Core
{
    class Aggregator
    {
        private static List<Thread> timers = new List<Thread>();
        private static MongoServer server;

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

        private static void StartItem(KeyValuePair<string, ComponentConfiguration> item)
        {
            var value = item.Value;
            var key = item.Key;
            if (value != null && !string.IsNullOrWhiteSpace(value.Url))
            {
                var db = server.GetDatabase(key);
                foreach (var agg in value.AggregateSpans)
                {
                    foreach (var configItem in value.ComponentItems)
                    {
                        var aggcollectionname = string.Format("{0}__{1}", configItem.Key, agg.Key);
                        while (!db.CollectionExists(aggcollectionname))
                        {
                            var options = CollectionOptions
                                .SetCapped(true)
                                .SetAutoIndexId(false)
                                .SetMaxSize(1024 * 1024 * 10)
                                .SetMaxDocuments(100000);
                            db.CreateCollection(aggcollectionname, options);
                            Thread.Sleep(200);
                        }
                    }
                    var timer = new Thread(state =>
                    {
                        while (true)
                        {
                            var t = state as Tuple<KeyValuePair<string, TimeSpan>, ComponentConfiguration>;
                            if (t != null)
                            {
                                var aggInfo = t.Item1;
                                var config = t.Item2;

                                try
                                {
                                    foreach (var configItem in config.ComponentItems)
                                    {
                                        if (configItem.Value.ItemValueType == ItemValueType.TotalValue ||
                                            configItem.Value.ItemValueType == ItemValueType.StateValue)
                                        {
                                            var collection = db.GetCollection(configItem.Key);
                                            var aggcollectionname = string.Format("{0}__{1}", configItem.Key, aggInfo.Key);
                                            var aggcollection = db.GetCollection(aggcollectionname);
                                            var aggstart = aggcollection.FindAll().SetLimit(1).SetSortOrder(SortBy.Descending("$natural")).FirstOrDefault();
                                            DateTime? startTime = null;
                                            if (aggstart != null)
                                            {
                                                startTime = aggstart["_id"].AsDateTime.ToLocalTime();
                                            }
                                            else
                                            {
                                                var start = collection.FindAll().SetLimit(1).SetSortOrder(SortBy.Ascending("$natural")).FirstOrDefault();
                                                if (start != null)
                                                {
                                                    startTime = start["_id"].AsDateTime.ToLocalTime();
                                                }
                                            }

                                            if (startTime != null)
                                            {
                                                var endTime = startTime.Value.Add(aggInfo.Value);
                                                while (endTime < DateTime.Now)
                                                {
                                                    var query = Query.LT("_id", endTime).GTE(startTime);
                                                    var data = collection.Find(query).ToList();
                                                    object v = 0;
                                                    if (data.Count > 0)
                                                    {
                                                        Func<BsonDocument, long> func = a =>
                                                        {
                                                            if (a["V"].IsInt64)
                                                                return a["V"].AsInt64;
                                                            if (a["V"].IsInt32)
                                                                return a["V"].AsInt32;
                                                            else return 0;
                                                        };
                                                        if (configItem.Value.ItemValueType == ItemValueType.TotalValue)
                                                            v = data.Select(func).Sum();
                                                        if (configItem.Value.ItemValueType == ItemValueType.StateValue)
                                                            v = data.Select(func).Average();
                                                    }
                                                    var doc = new BsonDocument().Add("V", BsonValue.Create(v));
                                                    doc.SetDocumentId(endTime.ToUniversalTime());
                                                    aggcollection.Insert(doc);
                                                    startTime = endTime;
                                                    endTime += aggInfo.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.Handle("聚合器" + key);
                                }
                                finally
                                {
                                    Thread.Sleep(aggInfo.Value);
                                }
                            }
                            else
                                Thread.Sleep(100);
                        }
                    });
                    LocalLoggingService.Info(string.Format("聚合器成功初始化：{0} {1}", key, agg.Key));
                    timer.Start(new Tuple<KeyValuePair<string, TimeSpan>, ComponentConfiguration>(agg, value));
                    timers.Add(timer);
                }
            }
        }

        internal static void Reset()
        {
            foreach (var timer in timers)
            {
                try
                {
                    timer.Abort();
                }
                catch
                {
                }
            }
            timers.Clear();
            Start();
        }
    }
}
