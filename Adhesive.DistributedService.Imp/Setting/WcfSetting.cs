

using System.Runtime.Serialization;
namespace Adhesive.DistributedService.Imp
{
    [DataContract(Namespace = "Adhesive.DistributedService")]
    public abstract class WcfSetting
    {
        [DataMember]
        public WcfLogSetting WcfLogSetting { get; set; }

        [DataMember]
        public WcfPerformanceServiceSetting WcfPerformanceServiceSetting { get; set; }

        [DataMember]
        public WcfSecuritySetting WcfSecuritySetting { get; set; }
    }
}
