using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "未处理异常过滤配置")]
    public class UnhandledExceptionFilterConfig
    {
        [ConfigItem(FriendlyName = "是否开启")]
        public bool Enabled { get; set; }
        [ConfigItem(FriendlyName = "网络蜘蛛异常过滤配置")]
        public SpiderExceptionFilterConfig SpiderExceptionFilterConfig { get; set; }
    }
}
