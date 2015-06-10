using System.Threading;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using System;
using Adhesive.Common;
using Adhesive.AppInfoCenter;
using MongoDB.Bson;

namespace Adhesive.GeneralPerformance.Core
{
    internal class GeneralPerformanceAggregator
    {
        private Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        private MongoServer server;
        private ItemConfigurationEntity config;
        internal GeneralPerformanceAggregator(ItemConfigurationEntity config)
        {
            this.config = config;
        }

        internal void Start()
        {
            server = MongoServer.Create(Configuration.GetConfig().DatabaseUrlForGeneralPerformanceMaster);
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        StartItem(config, "Success", "Sum");
                        StartItem(config, "Failed", "Sum");
                        StartItem(config, "Time", "Avg");
                    }
                    catch (Exception ex)
                    {
                        ex.Handle();                
                    }
                    finally
                    {
                        Thread.Sleep(1000 * 60);
                    }
                }
            }) { IsBackground = true }.Start();
        }

        private void StartItem(ItemConfigurationEntity item, string name, string type)
        {
            var dbNames = server.GetDatabaseNames().Where(_ => _.StartsWith(item.Prefix + "__" + name)).ToList();
            foreach (var dbName in dbNames)
            {
                var dbb = server.GetDatabase(dbName);
                var colNames = dbb.GetCollectionNames().Where(n => !n.Contains("system.")
                    && !n.Contains("$")
                    && !n.Contains("___")).ToList();

                foreach (var colName in colNames)
                {
                    foreach (var agg in item.AggregateSpans)
                    {
                        var aggcollectionname = string.Format("{0}___{1}", colName, agg.Key);
                        try
                        {
                            while (!dbb.CollectionExists(aggcollectionname))
                            {
                                var options = CollectionOptions
                                    .SetCapped(true)
                                    .SetAutoIndexId(false)
                                    .SetMaxSize(1024 * 1024 * 10)
                                    .SetMaxDocuments(100000);
                                dbb.CreateCollection(aggcollectionname, options);
                                Thread.Sleep(200);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Handle("聚合器创建表失败：" + aggcollectionname);
                            continue;
                        }

                        var key = string.Format("{0}.{1}.{2}", dbName, colName, agg.Key);

                        if (!timers.ContainsKey(key))
                        {
                            var timer = new Timer(state =>
                            {
                                var a = state as Tuple<string, string, string, TimeSpan>;
                                if (a != null)
                                {
                                    try
                                    {
                                        var db = server.GetDatabase(a.Item1);
                                        var collection = db.GetCollection(a.Item2);
                                        var aggcollection = db.GetCollection(a.Item3);
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
                                            var endTime = startTime.Value.Add(a.Item4);
                                            while (endTime < DateTime.Now)
                                            {
                                                var query = Query.LT("_id", endTime).GTE(startTime);
                                                var data = collection.Find(query).ToList();
                                                object v = 0;
                                                if (data.Count > 0)
                                                {
                                                    Func<BsonDocument, long> func = b =>
                                                    {
                                                        if (b["V"].IsInt64)
                                                            return b["V"].AsInt64;
                                                        if (b["V"].IsInt32)
                                                            return b["V"].AsInt32;
                                                        else return 0;
                                                    };
                                                    switch (type)
                                                    {
                                                        case "Min":
                                                            v = data.Select(func).Min();
                                                            break;
                                                        case "Max":
                                                            v = data.Select(func).Max();
                                                            break;
                                                        case "Avg":
                                                            v = Convert.ToInt32(data.Select(func).Average());
                                                            break;
                                                        case "Sum":
                                                            v = data.Select(func).Sum();
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                }
                                                var doc = new BsonDocument().Add("V", BsonValue.Create(v));
                                                doc.SetDocumentId(endTime.ToUniversalTime());
                                                aggcollection.Insert(doc);
                                                startTime = endTime;
                                                endTime += a.Item4;
                                            }
                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Handle(string.Format("{3} PerformanceAggregator出错：{0} {1} {2}", dbName, colName, agg.Key, config.Name));
                                    }
                                }

                            }, new Tuple<string, string, string, TimeSpan>(dbName, colName, aggcollectionname, agg.Value), TimeSpan.Zero, agg.Value);
                            timers.Add(key, timer);
                            LocalLoggingService.Info(string.Format("{3} PerformanceAggregator成功初始化：{0} {1} {2}", dbName, colName, agg.Key, config.Name));
                        }
                    }
                }
            }
        }
    }
}
