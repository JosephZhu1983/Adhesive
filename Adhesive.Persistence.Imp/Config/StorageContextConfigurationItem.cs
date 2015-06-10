

using Adhesive.Config;

namespace Adhesive.Persistence.Imp.Config
{
    public class StorageContextConfigurationItem
    {
        [ConfigItem(FriendlyName = "存储上下文名称")]
        public string Name { get; set; }
        [ConfigItem(FriendlyName="数据提供程序名称")]
        public string ProviderName { get; set; }
        [ConfigItem(FriendlyName = "数据库连接字符串")]
        public string ConnectionString { get; set; }
    }
}
