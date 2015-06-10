
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.Mongodb.Imp
{
    [ConfigEntity(FriendlyName = "Mongodb数据服务客户端配置")]
    public class MongodbServiceConfigurationEntity
    {
        [ConfigItem(FriendlyName = "插入服务配置项列表")]
        public Dictionary<string, MongodbInsertServiceConfigurationItem> MongodbInsertServiceConfigurationItems { get; set; }
    }
}
