

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfServerSetting : WcfSetting
    {
        [DataMember]
        public WcfServerCoreSetting WcfCoreSetting { get; set; }
    }
}
