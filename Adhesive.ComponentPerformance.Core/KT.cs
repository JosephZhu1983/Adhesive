using System.Collections.Generic;
using System.Net;

namespace Adhesive.ComponentPerformance.Core
{
    internal class KT
    {
        private static Dictionary<string, ComponentItem> items = new Dictionary<string, ComponentItem>
        { 
            { "version", new ComponentItem("其它", "版本号", ItemValueType.TextValue) },

            { "curr_connections", new ComponentItem("连接", "当前连接数量", ItemValueType.StateValue) },

            { "cmd_get", new ComponentItem("操作数", "查询请求数", ItemValueType.TotalValue) },
            { "cmd_set", new ComponentItem("操作数", "存储请求数", ItemValueType.TotalValue) },
            { "cmd_delete", new ComponentItem("操作数", "删除请求数", ItemValueType.TotalValue) },
            { "cmd_flush", new ComponentItem("操作数", "清空请求数", ItemValueType.TotalValue) },

            { "curr_items", new ComponentItem("数据", "当前存储的内容数量", ItemValueType.StateValue) },
            { "bytes", new ComponentItem("数据", "存储的内容变化数量", ItemValueType.StateValue) },


            { "get_hits", new ComponentItem("命中", "查询成功获取数据的次数", ItemValueType.TotalValue) },
            { "get_misses", new ComponentItem("命中", "查询成功未获取到数据的次数", ItemValueType.TotalValue) },
            { "get_hits * 100 / (get_hits + get_misses)", new ComponentItem("命中", "查询成功未获取到数据的比例", ItemValueType.ExpressionValue) },
        };

        internal static Dictionary<string, ComponentItem> GetComponentItems()
        {
            return items;
        }

        internal static Dictionary<string, object> GetCurrentStatus(string url)
        {
            if (url.Split(':').Length != 2) return null;
            string ip = url.Split(':')[0];
            int port;
            if (string.IsNullOrWhiteSpace(ip) || !int.TryParse(url.Split(':')[1], out port))
                return null;

            var data = new Dictionary<string, object>();
            using (MemcachedSocket s = new MemcachedSocket(new IPEndPoint(IPAddress.Parse(ip), port)))
            {
                var raw = s.GetStats();
                foreach (var item in items)
                {
                    if (raw.ContainsKey(item.Key))
                    {
                        if (item.Value.ItemValueType == ItemValueType.TextValue)
                        {
                            data.Add(item.Key, raw[item.Key]);
                        }
                        else if (item.Value.ItemValueType == ItemValueType.StateValue || item.Value.ItemValueType == ItemValueType.TotalValue)
                        {
                            long val = 0;
                            if (long.TryParse(raw[item.Key], out val))
                            {
                                data.Add(item.Key, val);
                            }
                        }
                    }
                }
            }
            return data;
        }
    }
}