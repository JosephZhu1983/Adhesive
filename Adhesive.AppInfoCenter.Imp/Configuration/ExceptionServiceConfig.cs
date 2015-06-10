
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "异常服务配置")]
    public class ExceptionServiceConfig
    {
        [ConfigItem(FriendlyName = "是否开启")]
        public bool Enabled { get; set; }

        [ConfigItem(FriendlyName = "未处理异常消息")]
        public string UnhandledExceptionMessage { get; set; }

        [ConfigItem(FriendlyName = "异常策略列表")]
        public Dictionary<string, ExceptionStrategy> StrategyList { get; set; }
        [ConfigItem(FriendlyName = "未处理异常过滤配置")]
        public UnhandledExceptionFilterConfig UnhandledExceptionFilterConfig { get; set; }
    }
}