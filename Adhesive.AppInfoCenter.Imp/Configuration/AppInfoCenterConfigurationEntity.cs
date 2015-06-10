
using System.Collections.Generic;
using Adhesive.Config;
namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "信息中心配置")]
    public class AppInfoCenterConfigurationEntity
    {
        [ConfigItem(FriendlyName = "包含信息策略列表")]
        public Dictionary<string, IncludeInfoStrategy> IncludeInfoStrategys { get; set; }

        [ConfigItem(FriendlyName = "包含信息的策略配置")]
        public Dictionary<string, IncludeInfoStrategyConfigurationItem> IncludeInfoStrategyConfigurations { get; set; }

        [ConfigItem(FriendlyName = "公共配置")]
        public CommonConfig CommonConfig { get; set; }

        [ConfigItem(FriendlyName = "日志服务配置")]
        public LoggingServiceConfig LoggingServiceConfig { get; set; }

        [ConfigItem(FriendlyName = "性能服务配置")]
        public PerformanceServiceConfig PerformanceServiceConfig { get; set; }

        [ConfigItem(FriendlyName = "状态服务配置")]
        public StateServiceConfig StateServiceConfig { get; set; }

        [ConfigItem(FriendlyName = "异常服务配置")]
        public ExceptionServiceConfig ExceptionServiceConfig { get; set; }
    }
}
