

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class StartInfoSetting
    {
        [DataMember]
        public bool Enabled { get; set; }
    }
}
