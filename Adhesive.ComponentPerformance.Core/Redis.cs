using System.Collections.Generic;
using ServiceStack.Redis;

namespace Adhesive.ComponentPerformance.Core
{
    internal class Redis
    {
        private static Dictionary<string, ComponentItem> items = new Dictionary<string, ComponentItem>
        { 
            { "redis_version", new ComponentItem("其它", "版本号", ItemValueType.TextValue) },

            { "used_cpu_sys", new ComponentItem("CPU", "CPU系统时间", ItemValueType.StateValue) },
            { "used_cpu_user", new ComponentItem("CPU", "CPU用户时间", ItemValueType.StateValue) },
            
            { "connected_clients", new ComponentItem("连接", "连接的客户端", ItemValueType.StateValue) },
            { "blocked_clients", new ComponentItem("连接", "阻塞的客户端", ItemValueType.StateValue) },
            { "connected_slaves", new ComponentItem("连接", "连接的从机", ItemValueType.StateValue) },
            { "total_connections_received", new ComponentItem("连接", "收到的连接", ItemValueType.TotalValue) },

            { "used_memory", new ComponentItem("内存", "Redis分配的内存", ItemValueType.StateValue) },
            { "used_memory_rss", new ComponentItem("内存", "使用的系统内存", ItemValueType.StateValue) },
            { "mem_fragmentation_ratio", new ComponentItem("内存", "内存碎片率", ItemValueType.StateValue) },

            { "vm_stats_used_pages", new ComponentItem("VM", "VM使用的页数", ItemValueType.StateValue) },
            { "vm_stats_swapped_objects", new ComponentItem("VM", "VM切换的页数", ItemValueType.StateValue) },
            { "vm_stats_swappin_count", new ComponentItem("VM", "VM换入的页数", ItemValueType.TotalValue) },
            { "vm_stats_swappout_count", new ComponentItem("VM", "VM换出的页数", ItemValueType.TotalValue) },

            { "expired_keys", new ComponentItem("数据", "过期的键", ItemValueType.TotalValue) },
            { "evicted_keys", new ComponentItem("数据", "逐出的键", ItemValueType.TotalValue) },
            { "pubsub_channels", new ComponentItem("数据", "发布订阅通道数", ItemValueType.StateValue) },
            { "keyspace_hits", new ComponentItem("数据", "键空间命中", ItemValueType.TotalValue) },
            { "keyspace_misses", new ComponentItem("数据", "键空间丢失", ItemValueType.TotalValue) },

            { "total_commands_processed", new ComponentItem("操作数", "处理的命令", ItemValueType.TotalValue) },            
            { "bgsave_in_progress", new ComponentItem("操作数", "正在bgsave的命令", ItemValueType.StateValue) },
            { "changes_since_last_save", new ComponentItem("操作数", "上次保存后的修改数", ItemValueType.StateValue) },
            { "bgrewriteaof_in_progress", new ComponentItem("操作数", "正在bgrewriteaof的命令", ItemValueType.StateValue) },
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
            using (RedisClient s = new RedisClient(ip, port))
            {
                var raw = s.Info;
                
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
