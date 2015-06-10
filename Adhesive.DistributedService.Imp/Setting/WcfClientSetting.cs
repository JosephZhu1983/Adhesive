

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfClientSetting : WcfSetting
    {
        [DataMember]
        public WcfClientCoreSetting WcfCoreSetting { get; set; }
    }
}
