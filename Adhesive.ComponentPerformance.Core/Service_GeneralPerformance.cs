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
    public partial class Service
    {
        private static MongoServer serverGeneralPerformance;

        [OperationContract]
        [WebGet]
        public List<string> GPGetAppNames(string catname, string subcatname)
        {
            var pre = string.Format("{0}__{1}__", catname, subcatname);
            var names = serverGeneralPerformance.GetDatabaseNames().Where(n => n.StartsWith(pre)).Select(
                name =>
                {
                    var a = name.Substring(pre.Length);
                    return a.Substring(0, a.IndexOf("__"));
                });
            return names.Distinct().ToList();
        }

        [OperationContract]
        [WebGet]
        public List<string> GPGetItemName(string catname, string subcatname, string groupname, string appname, string aggName)
        {
            
            var r = new List<string>();
            try
            {
                var db = serverGeneralPerformance.GetDatabase(string.Format("{0}__{1}__{2}__{3}", catname, subcatname, appname, groupname));
                var colNames = db.GetCollectionNames().Where(name => !name.Contains("system.") && !name.Contains("$"));
                if (string.IsNullOrEmpty(aggName))
                    colNames = colNames.Where(n => !n.Contains("___")).ToList();
                else
                    colNames = colNames.Where(n => n.Contains("___" + aggName)).ToList();
                foreach (var colName in colNames)
                {
                    var colKey = colName;
                    if (colKey.Contains("___"))
                        colKey = colKey.Substring(0, colKey.IndexOf("___"));
                    if (colKey.Contains("__"))
                        r.Add(colKey.Substring(0, colKey.IndexOf("__")));
                    else
                        r.Add(colKey);
                }
            }
            catch(Exception ex)
            {
                ex.Handle();
            }
            return r.Distinct().ToList();
        }

        [OperationContract]
        [WebGet]
        public Dictionary<string, List<List<long>>> GPGetData(string catname, string subcatname, string groupname, string appname, string aggName, string itemname, int pageindex, int pagesize)
        {
            var r = new Dictionary<string, List<List<long>>>();

            var db = serverGeneralPerformance.GetDatabase(string.Format("{0}__{1}__{2}__{3}", catname, subcatname, appname, groupname));
            var colNames = db.GetCollectionNames().Where(name => !name.Contains("system.") && !name.Contains("$"));
            if (string.IsNullOrEmpty(aggName))
                colNames = colNames.Where(n => !n.Contains("___"));
            else
                colNames = colNames.Where(n => n.Contains("___" + aggName));

            if (!string.IsNullOrEmpty(itemname))
                colNames = colNames.Where(n => n.Contains(itemname));
            else
                colNames = colNames.Take(20);

            Parallel.ForEach(colNames.ToList(), colName =>
            {
                try
                {
                    var colKey = colName;
                    if (colKey.Contains("___"))
                        colKey = colKey.Substring(0, colKey.IndexOf("___"));

                    var col = db.GetCollection(colName);
                    var data = col.FindAll().SetLimit(pagesize).SetSkip(pageindex * pagesize)
                        .SetSortOrder(SortBy.Descending("$natural")).ToList();
                    var v = data.Select(item =>
                        new List<long>
                {
                    Convert.ToInt64((item["_id"].AsDateTime.ToLocalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds),
                    Math.Max(Convert.ToInt64(item["V"].RawValue), 0),
                }).Reverse().ToList();

                    if (v.Count > 0)
                    {
                        lock (r)
                        {
                            r.Add(colKey, v);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Handle();
                }
            });

            return r.OrderBy(_ => _.Key).ToDictionary(_ => _.Key, _ => _.Value);
        }
    }
}
