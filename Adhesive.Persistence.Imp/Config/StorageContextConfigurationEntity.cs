
using System.Collections.Generic;
using System.Xml.Serialization;
using Adhesive.Config;

namespace Adhesive.Persistence.Imp.Config
{
    [ConfigEntity(FriendlyName="�洢����������")]
    public class StorageContextConfigurationEntity
    {
        [ConfigItem(FriendlyName="�洢�����ļ���")]
        public Dictionary<string,StorageContextConfigurationItem> StorageContexts { get; set; }
    }
}
