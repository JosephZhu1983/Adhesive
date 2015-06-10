
using Adhesive.Common;

using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "状态服务配置项")]
    public class StateServiceConfigurationItem
    {
        [ConfigItem(FriendlyName = "类型完整名")]
        public string TypeFullName { get; set; }

        [ConfigItem(FriendlyName = "是否开启")]
        public bool Enabled { get; set; }

        [ConfigItem(FriendlyName = "汇报状态的时间间隔毫秒")]
        public int ReportStateIntervalMilliSeconds { get; set; }

        public StateServiceConfigurationItem()
        {
            TypeFullName = "";
            Enabled = false;
            ReportStateIntervalMilliSeconds = CommonConfiguration.GetConfig().StateServiceDefaultReportStateIntervalMilliSeconds;
        }
    }
}
