using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Adhesive.ComponentPerformance.Core
{
    internal class Mongodb
    {
        private static Dictionary<string, MongoServer> servers = new Dictionary<string, MongoServer>();
        private static Dictionary<string, ComponentItem> items = new Dictionary<string, ComponentItem>
        { 
            { "version", new ComponentItem("其它", "版本号", ItemValueType.TextValue) },
       
            { "mem.resident", new ComponentItem("内存", "使用的物理内存大小", ItemValueType.StateValue) },
            { "mem.virtual", new ComponentItem("内存", "使用的虚拟内存大小", ItemValueType.StateValue) },
            { "mem.mapped", new ComponentItem("内存", "映射的内存大小", ItemValueType.StateValue) },
            { "mem.mappedWithJournal", new ComponentItem("内存", "具有日志的映射的内存大小", ItemValueType.StateValue) },
            { "extra_info.page_faults", new ComponentItem("内存", "加载磁盘内容时发生页错误的次数", ItemValueType.TotalValue) },

            { "connections.current", new ComponentItem("连接", "当前连接数", ItemValueType.StateValue) },
            { "connections.available", new ComponentItem("连接", "可用连接数", ItemValueType.StateValue) },

            { "network.bytesIn", new ComponentItem("网络", "网络读取字节数", ItemValueType.TotalValue) },
            { "network.bytesOut", new ComponentItem("网络", "网络发送字节数", ItemValueType.TotalValue) },
            { "network.numRequests", new ComponentItem("网络", "网络请求数", ItemValueType.TotalValue) },

            { "opcounters.insert", new ComponentItem("操作数", "insert数", ItemValueType.TotalValue) },
            { "opcounters.query", new ComponentItem("操作数", "query数", ItemValueType.TotalValue) },
            { "opcounters.update", new ComponentItem("操作数", "update数", ItemValueType.TotalValue) },
            { "opcounters.delete", new ComponentItem("操作数", "delete数", ItemValueType.TotalValue) },
            { "opcounters.getmore", new ComponentItem("操作数", "游标getmore数", ItemValueType.TotalValue) },
            { "opcounters.command", new ComponentItem("操作数", "其它操作数", ItemValueType.TotalValue) },

            { "indexCounters.btree.accesses", new ComponentItem("索引", "访问索引次数", ItemValueType.TotalValue) },
            { "indexCounters.btree.hits", new ComponentItem("索引", "内存命中索引次数", ItemValueType.TotalValue) },
            { "indexCounters.btree.misses", new ComponentItem("索引", "内存不命中索引次数", ItemValueType.TotalValue) },
            { "indexCounters.btree.resets", new ComponentItem("索引", "索引计数器重置次数", ItemValueType.TotalValue) },
            { "indexCounters.btree.hits * 100 / indexCounters.btree.accesses", new ComponentItem("索引", "hitsratio ", ItemValueType.ExpressionValue) },
        };

        private static BsonElement GetNestedBson(BsonDocument doc, string name)
        {
            BsonElement element = null;
            var levels = name.Split('.');
            if (levels.Length > 1)
            {
                for (int i = 0; i < levels.Length - 1; i++)
                {
                    name = levels[levels.Length - 1];
                    doc.TryGetElement(levels[i], out element);
                    if (element != null && element.Value.IsBsonDocument)
                        doc = element.Value.AsBsonDocument;
                }
            }
            doc.TryGetElement(name, out element);
            return element;
        }

        internal static Dictionary<string, ComponentItem> GetComponentItems()
        {
            return items;
        }

        internal static Dictionary<string, object> GetCurrentStatus(string url)
        {
            if (!servers.ContainsKey(url))
            {
                lock (servers)
                {
                    if (!servers.ContainsKey(url))
                    {
                        servers.Add(url, MongoServer.Create(url));
                    }
                }
            }

            MongoServer server = null;
            Dictionary<string, object> data = null;

            if (servers.TryGetValue(url, out server))
            {
                data = new Dictionary<string, object>();

                var raw = server.GetDatabase("admin").RunCommand("serverStatus");
                if (raw.Ok)
                {
                    var doc = raw.Response;
                    foreach (var item in items)
                    {
                        if (item.Value.ItemValueType != ItemValueType.ExpressionValue)
                        {
                            var element = GetNestedBson(doc, item.Key);
                            if (element != null && element.Value != null)
                            {
                                if (item.Value.ItemValueType == ItemValueType.TextValue && element.Value.IsString)
                                    data.Add(item.Key, element.Value.ToString());
                                else if ((item.Value.ItemValueType == ItemValueType.StateValue || item.Value.ItemValueType == ItemValueType.TotalValue)
                                    && element.Value.IsNumeric)
                                    data.Add(item.Key, element.Value.RawValue);
                            }
                        }
                    }
                }
            }
            return data;
        }
    }
}
