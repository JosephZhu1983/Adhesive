

using Adhesive.Config;

namespace Adhesive.Persistence.Imp.Config
{
    public class StorageContextConfigurationItem
    {
        [ConfigItem(FriendlyName = "�洢����������")]
        public string Name { get; set; }
        [ConfigItem(FriendlyName="�����ṩ��������")]
        public string ProviderName { get; set; }
        [ConfigItem(FriendlyName = "���ݿ������ַ���")]
        public string ConnectionString { get; set; }
    }
}
