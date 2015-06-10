
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "性能服务配置")]
    public class PerformanceServiceConfig
    {
        [ConfigItem(FriendlyName = "网站页面执行状态配置")]
        public WebsitePageExecutionStateConfig WebsitePageExecutionStateConfig { get; set; }

        [ConfigItem(FriendlyName = "代码性能测量配置")]
        public PerformanceMeasureConfig PerformanceMeasureConfig { get; set; }

        [ConfigItem(FriendlyName = "页面性能服务配置")]
        public PagePerformanceServiceConfig PagePerformanceServiceConfig { get; set; }
    }
}
