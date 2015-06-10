
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "状态服务配置")]
    public class StateServiceConfig
    {
        [ConfigItem(FriendlyName = "针对不同类型的状态服务配置")]
        public Dictionary<string, StateServiceConfigurationItem> StateServiceConfigurationItems { get; set; }
    }
}
