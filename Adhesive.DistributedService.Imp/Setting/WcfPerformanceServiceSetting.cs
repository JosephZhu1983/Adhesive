
namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;
    using Adhesive.Config;
    using System.Collections.Generic;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfPerformanceServiceSetting
    {
        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        [ConfigItem(FriendlyName = "汇报状态的间隔毫秒")]
        public int ReportStateIntervalMilliSeconds { get; set; }

        [ConfigItem(FriendlyName = "允许的方法", Description = "只要包含一个*则为允许所有")]
        public List<string> AllowMethods { get; set; }

        [ConfigItem(FriendlyName = "拒绝的方法", Description = "拒绝比允许优先级高")]
        public List<string> DenyMethods { get; set; }
    }
}