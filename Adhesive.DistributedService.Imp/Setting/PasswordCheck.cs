

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class PasswordCheck
    {
        [DataMember]
        public bool Enable { get; set; }

        [DataMember]
        public OperationDirection Direction { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
