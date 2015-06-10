

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfSecuritySetting
    {
        [DataMember]
        public PasswordCheck PasswordCheck { get; set; }
    }
}
