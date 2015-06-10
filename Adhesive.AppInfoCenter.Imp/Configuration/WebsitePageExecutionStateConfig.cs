
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "页面执行状态配置")]
    public class WebsitePageExecutionStateConfig
    {
        [ConfigItem(FriendlyName = "是否开启")]
        public bool Enabled { get; set; }

        [ConfigItem(FriendlyName = "记录慢页面的毫秒阀值")]
        public int LogSlowPageExecutionMilliSecondsThreshold { get; set; }
    }
}
