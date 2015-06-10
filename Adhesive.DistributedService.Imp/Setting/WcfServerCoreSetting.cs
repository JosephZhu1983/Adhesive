

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfServerCoreSetting : WcfCoreSetting
    {
        [DataMember]
        public bool EnableUnity { get; set; }

    }
}
