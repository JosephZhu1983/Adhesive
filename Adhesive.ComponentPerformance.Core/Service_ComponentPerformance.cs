using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Adhesive.AppInfoCenter;
using Adhesive.Common;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Adhesive.ComponentPerformance.Core
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    [JavascriptCallbackBehavior(UrlParameterName = "callback")]
    public partial class Service
    {
        private static MongoServer server;
        private static ServiceHost host;

        public static void Stop()
        {
            AdhesiveFramework.End();
            host.Close();
        }

        public static void Start()
        {

            AdhesiveFramework.Start();
            LocalLoggingService.Info("框架启动成功");
            host = new ServiceHost(typeof(Service));
            host.Open();
            LocalLoggingService.Info("WCF服务启动成功");
            Collector.Start();
            LocalLoggingService.Info("收集器启动成功");
            Aggregator.Start();
            LocalLoggingService.Info("聚合器启动成功");
            InternalStart();
            LocalLoggingService.Info("服务启动成功");
        }

        private static void InternalStart()
        {
            var rootconfig = Configuration.GetConfig();
            server = MongoServer.Create(rootconfig.DatabaseUrlForComponentPerformanceSlave);
            serverGeneralPerformance = MongoServer.Create(rootconfig.DatabaseUrlForGeneralPerformanceSlave);
        }

        internal static void Reset()
        {
            Collector.Reset();
            LocalLoggingService.Info("收集器重启成功");
            Aggregator.Reset();
            LocalLoggingService.Info("聚合器重启成功");
        }

        [OperationContract]
        [WebGet]
        public List<List<long>> Hello()
        {
            var r = new Random(Environment.TickCount);
            return Enumerable.Range(0, 100).Select(i =>
            new List<long>
            {
                Convert.ToInt64((new DateTime(2011, 1, 1).AddMinutes(i) - new DateTime(1970, 1, 1)).TotalMilliseconds),
                r.Next(50, 100)
            }).ToList();
        }

        [OperationContract]
        [WebGet]
        public string ResetAgg(string password)
        {
            if (password != "//5173@#") return "";

            var rootconfig = Configuration.GetConfig();

            if (rootconfig.MemcachedList != null)
            {
                foreach (var item in rootconfig.MemcachedList)
                {
                    var db = server.GetDatabase(item.Key);
                    var colNames = db.GetCollectionNames().Where(name => name.Contains("__")).ToList();
                    foreach (var colName in colNames)
                    {
                        var col = db.GetCollection(colName);
                        col.Drop();
                    }
                }
            }

            if (rootconfig.KTList != null)
            {
                foreach (var item in rootconfig.KTList)
                {
                    var db = server.GetDatabase(item.Key);
                    var colNames = db.GetCollectionNames().Where(name => name.Contains("__")).ToList();
                    foreach (var colName in colNames)
                    {
                        var col = db.GetCollection(colName);
                        col.Drop();
                    }
                }
            }

            if (rootconfig.MongodbList != null)
            {
                foreach (var item in rootconfig.MongodbList)
                {
                    var db = server.GetDatabase(item.Key);
                    var colNames = db.GetCollectionNames().Where(name => name.Contains("__")).ToList();
                    foreach (var colName in colNames)
                    {
                        var col = db.GetCollection(colName);
                        col.Drop();
                    }
                }
            }

            if (rootconfig.RedisList != null)
            {
                foreach (var item in rootconfig.RedisList)
                {
                    var db = server.GetDatabase(item.Key);
                    var colNames = db.GetCollectionNames().Where(name => name.Contains("__")).ToList();
                    foreach (var colName in colNames)
                    {
                        var col = db.GetCollection(colName);
                        col.Drop();
                    }
                }
            }
            return "OK";
        }

        [OperationContract]
        [WebGet]
        public List<string> GetDbNames(string cat)
        {
            var rootconfig = Configuration.GetConfig();
            switch (cat)
            {
                case "mongodb":
                    return rootconfig.MongodbList.Keys.ToList();
                case "memcached":
                    return rootconfig.MemcachedList.Keys.ToList();
                case "kt":
                    return rootconfig.KTList.Keys.ToList();
                case "redis":
                    return rootconfig.RedisList.Keys.ToList();
            }
            return new List<string>();
        }

        [OperationContract]
        [WebGet]
        public Dictionary<string, string> GetTextData(string dbName)
        {
            var rootconfig = Configuration.GetConfig();
            var c = rootconfig.MemcachedList.Union(rootconfig.KTList).Union(rootconfig.MongodbList).Union(rootconfig.RedisList).SingleOrDefault(item => item.Key == dbName).Value;
            var r = new Dictionary<string, string>();
            try
            {
                if (c != null)
                {
                    var db = server.GetDatabase(dbName);
                    var colNames = c.ComponentItems.Where(a => a.Value.ItemValueType == ItemValueType.TextValue).Select(a => a.Key);
                    foreach (var colName in colNames)
                    {
                        if (c.ComponentItems.ContainsKey(colName))
                        {
                            var citem = c.ComponentItems[colName];
                            var col = db.GetCollection(colName);
                            var data = col.FindOne()["V"].RawValue == null ? "" : col.FindOne()["V"].RawValue.ToString();
                            r.Add(citem.Name, data);
                        }
                    }
                }

                return r;
            }
            catch(Exception ex)
            {
                ex.Handle();
                return r;
            }
        }

        [OperationContract]
        [WebGet]
        public List<string> GetGroupNames(string dbName)
        {
            var rootconfig = Configuration.GetConfig();
            var c = rootconfig.MemcachedList.Union(rootconfig.KTList).Union(rootconfig.MongodbList).Union(rootconfig.RedisList).SingleOrDefault(item => item.Key == dbName).Value;
            var r = new List<string>();
            if (c != null)
                return c.ComponentItems.Values.Select(a => a.GroupName).Distinct().ToList();
            return r;
        }

        [OperationContract]
        [WebGet]
        public Dictionary<string, List<List<long>>> GetData(string dbName, string aggName, int pageindex, int pagesize, string groupName)
        {
            var rootconfig = Configuration.GetConfig();
            var c = rootconfig.MemcachedList.Union(rootconfig.KTList).Union(rootconfig.MongodbList).Union(rootconfig.RedisList).SingleOrDefault(item => item.Key == dbName).Value;
            var r = new Dictionary<string, List<List<long>>>();
            if (c != null)
            {
                var db = server.GetDatabase(dbName);
                var colNames = db.GetCollectionNames().Where(name => !name.Contains("system.") && !name.Contains("$"));
                if (string.IsNullOrEmpty(aggName))
                    colNames = colNames.Where(n => !n.Contains("__")).ToList();
                else
                    colNames = colNames.Where(n => n.Contains("__" + aggName)).ToList();
                Parallel.ForEach(colNames, colName =>
                {
                    var colKey = colName;
                    if (colKey.Contains("__"))
                        colKey = colKey.Substring(0, colKey.IndexOf("__"));
                    if (c.ComponentItems.ContainsKey(colKey))
                    {
                        var citem = c.ComponentItems[colKey];
                        if (string.IsNullOrEmpty(groupName) || groupName == citem.GroupName)
                        {
                            if (citem.ItemValueType != ItemValueType.TextValue)
                            {
                                var col = db.GetCollection(colName);
                                var data = col.FindAll().SetLimit(pagesize).SetSkip(pageindex * pagesize)
                                    .SetSortOrder(SortBy.Descending("$natural")).ToList();
                                var v = data.Select(item =>
                                    new List<long>
                                    {
                                        Convert.ToInt64((item["_id"].AsDateTime.ToLocalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds),
                                        Math.Max(Convert.ToInt64(item["V"].RawValue), 0),
                                    }).Reverse().ToList();

                                if (v.Count > 0 && !r.ContainsKey(citem.Name))
                                {
                                    lock (r)
                                    {
                                        r.Add(citem.Name, v);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            return r.OrderBy(_ => _.Key).ToDictionary(_ => _.Key, _ => _.Value);
        }      
    }
}
