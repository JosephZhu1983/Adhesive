
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.Mongodb.Server.Imp
{
    [ConfigEntity(FriendlyName = "Mongodb数据服务服务端配置")]
    public class MongodbServerConfigurationEntity
    {
        [ConfigItem(FriendlyName = "维护数据库时间间隔毫秒")]
        public int MaintainceIntervalMilliSeconds { get; set; }

        [ConfigItem(FriendlyName = "Memcached集群名")]
        public string MemcachedClusterName { get; set; }

        [ConfigItem(FriendlyName = "允许缓存")]
        public bool EnableCache { get; set; }

        [ConfigItem(FriendlyName = "服务端数据项配置列表")]
        public Dictionary<string, MongodbServerConfigurationItem> MongodbServerConfigurationItems { get; set; }

        [ConfigItem(FriendlyName = "管理员权限配置列表")]
        public Dictionary<string, MongodbAdminConfigurationItem> MongodbAdminConfigurationItems { get; set; }

        [ConfigItem(FriendlyName = "Mongodb服务器列表")]
        public Dictionary<string, MongodbServerUrl> MongodbServerUrls { get; set; }
    }
}
