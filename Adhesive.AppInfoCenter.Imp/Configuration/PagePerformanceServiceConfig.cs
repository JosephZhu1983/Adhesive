
using System;
using Adhesive.Config;
using System.Collections.Generic;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "页面性能服务配置")]
    public class PagePerformanceServiceConfig
    {
        [ConfigItem(FriendlyName = "是否开启")]
        public bool Enabled { get; set; }

        [ConfigItem(FriendlyName = "汇报状态的间隔毫秒")]
        public int ReportStateIntervalMilliSeconds { get; set; }

        [ConfigItem(FriendlyName = "允许的Url", Description = "只要包含一个*则为允许所有")]
        public List<string> AllowUrls { get; set; }

        [ConfigItem(FriendlyName = "拒绝的Url", Description = "拒绝比允许优先级高")]
        public List<string> DenyUrls { get; set; }
    }
}
