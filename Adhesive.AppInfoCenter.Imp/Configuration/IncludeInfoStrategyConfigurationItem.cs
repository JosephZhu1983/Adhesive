
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "包含信息的策略配置项")]
    public class IncludeInfoStrategyConfigurationItem
    {
        [ConfigItem(FriendlyName = "类型完整名")]
        public string TypeFullName { get; set; }

        [ConfigItem(FriendlyName = "包含信息策略名")]
        public string IncludeInfoStrategyName { get; set; }

        [ConfigItem(FriendlyName = "条件")]
        public Dictionary<string, object> Conditions { get; set; }
    }
}
