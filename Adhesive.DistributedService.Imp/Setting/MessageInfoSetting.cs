

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class MessageInfoSetting
    {
        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public MessageDirection MessageDirection { get; set; }
    }
}
