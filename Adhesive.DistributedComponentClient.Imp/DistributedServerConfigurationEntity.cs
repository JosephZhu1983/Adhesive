

using Adhesive.Common;
using Adhesive.Config;
namespace Adhesive.DistributedComponentClient
{
    [ConfigEntity(FriendlyName = "分布式组件客户端配置")]
    public class DistributedServerConfigurationEntity
    {
        [ConfigItem(FriendlyName = "分布式组件客户端集群配置")]
        public SerializableDictionary<string, ClientClusterConfiguration> ClientClusterConfigurations { get; set; }
    }
}
