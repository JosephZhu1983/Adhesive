
using System;
using Adhesive.Config;

namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "代码性能测试配置")]
    public class PerformanceMeasureConfig
    {
        [ConfigItem(FriendlyName = "是否开启")]
        public bool Enabled { get; set; }

        [ConfigItem(FriendlyName = "只对执行慢的页面进行记录")]
        public int PageExecutionMilliSecondsThreshold { get; set; }

        [ConfigItem(FriendlyName = "开始时间")]
        public DateTime BeginTime { get; set; }

        [ConfigItem(FriendlyName = "结束时间")]
        public DateTime EndTime { get; set; }
    }
}
