
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "日志服务配置")]
    public class LoggingServiceConfig
    {
        [ConfigItem(FriendlyName = "是否开启")]
        public bool Enabled { get; set; }

        [ConfigItem(FriendlyName = "日志策略列表")]
        public Dictionary<string, LogStrategy> StrategyList { get; set; }
    }
}
