
using System.Collections.Generic;
using System.Xml.Serialization;
using Adhesive.Config;

namespace Adhesive.Persistence.Imp.Config
{
    [ConfigEntity(FriendlyName="存储上下文配置")]
    public class StorageContextConfigurationEntity
    {
        [ConfigItem(FriendlyName="存储上下文集合")]
        public Dictionary<string,StorageContextConfigurationItem> StorageContexts { get; set; }
    }
}
