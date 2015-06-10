
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adhesive.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Adhesive.Mongodb.Server.Imp
{
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        #region Statistics

        public List<Statistics> GetStatisticsData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, TimeSpan span, Dictionary<string, object> filters)
        {
            try
            {
                var data = new ConcurrentBag<Statistics>();

                var databaseInfos = MongodbServerMaintainceCenter.GetDatabaseInfos(databasePrefix, beginTime, endTime);
                if (databaseInfos.Count == 0) return null;
                var typeFullName = MongodbServerMaintainceCenter.GetTypeFullName(databasePrefix);
                var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);

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

                var server = CreateSlaveMongoServer(typeFullName);
                if (server == null) return null;
                var serverUrl = GetMongodbServerUrl(typeFullName);
                var delay = TimeSpan.FromSeconds(0);
                if (serverUrl != null) delay = serverUrl.SyncDelay;

                Parallel.ForEach(tableNames, tableName =>
                {
                    var item = new Statistics();
                    item.StatisticsItems = new List<StatisticsItem>();
                    item.TableName = tableName;

                    var begin = beginTime;
                    var end = endTime;
                    while (begin < endTime)
                    {
                        DateTime tmpEndTime = begin.Add(span);
                        if (tmpEndTime > endTime)
                            tmpEndTime = endTime;

                        var statItem = new StatisticsItem
                        {
                            BeginTime = begin,
                            EndTime = tmpEndTime,
                            Value = 0,
                        };
                        if (begin.ToString("yyyyMM") == tmpEndTime.AddSeconds(-1).ToString("yyyyMM"))
                        {
                            var databaseInfo = databaseInfos.FirstOrDefault(d => d.DatabaseDate.ToLocalTime().ToString("yyyyMM") == tmpEndTime.ToString("yyyyMM"));
                            if (databaseInfo != null)
                            {
                                var databaseName = databaseInfo.DatabaseName;
                                statItem.Value = InternalGetDataCount(delay, typeFullName, databaseName, tableName,
                                    statTimeColumn.ColumnName, begin, tmpEndTime, filterquery);
                            }
                        }
                        else
                        {
                            var spanBegin = begin;
                            var spanEnd = tmpEndTime;
                            while (spanBegin < spanEnd)
                            {
                                DateTime tmpSpanEndTime = spanBegin.AddDays(1);
                                if (tmpSpanEndTime > spanEnd)
                                    tmpSpanEndTime = spanEnd;

                                var databaseInfo = databaseInfos.FirstOrDefault(d => d.DatabaseDate.ToLocalTime().ToString("yyyyMM") == spanBegin.ToString("yyyyMM"));
                                if (databaseInfo == null) continue;
                                var databaseName = databaseInfo.DatabaseName;
                                var query = Query.And(Query.LT(statTimeColumn.ColumnName, spanEnd).GTE(spanBegin), filterquery);
                                statItem.Value += InternalGetDataCount(delay, typeFullName, databaseName, tableName,
                                    statTimeColumn.ColumnName, spanBegin, tmpSpanEndTime, filterquery);
                                spanBegin = tmpSpanEndTime;
                            }
                        }
                        item.StatisticsItems.Add(statItem);
                        begin = tmpEndTime;
                    }
                    data.Add(item);
                });
                return data.ToList();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Statistics", "GetStatisticsData", ex.Message);
                throw;
            }
        }

        private int InternalGetDataCount(TimeSpan delay, string typeFullName, string databaseName, string tableName, string columnName, DateTime begin, DateTime end, IMongoQuery filterquery)
        {
            try
            {
                var count = MongodbServerMaintainceCenter.MemcachedClient.GetAndSetWhen<int?>(GetMemcachedKey(typeFullName, databaseName, tableName, columnName,
                    begin.ToString("yyyyMMddHHmmss"), end.ToString("yyyyMMddHHmmss"), filterquery), () =>
                    {
                        var query = Query.And(Query.LT(columnName, end).GTE(begin), filterquery);
                        var server = CreateSlaveMongoServer(typeFullName);
                        var database = server.GetDatabase(databaseName);
                        var collection = database.GetCollection(tableName);
                        var c = Convert.ToInt32(collection.Count(query));
                        //LocalLoggingService.Debug(string.Format("类型 {0} 数据库 {1} 表 {2} 列 {3} 开始 {4} 结束 {5} 条件 {6}  值 {7}", typeFullName, databaseName, tableName, columnName,
                        //   begin.ToString("yyyyMMddHHmmss"), end.ToString("yyyyMMddHHmmss"), filterquery == null ? "" : filterquery.ToString(), c));
                        return c;
                    }, TimeSpan.FromDays(20), () => MongodbServerConfiguration.GetConfig().EnableCache && DateTime.Now > end + delay);
                return count.Value;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Statistics", "InternalGetDataCount", ex.Message);
                throw;
            }
        }

        #endregion
    }
}
