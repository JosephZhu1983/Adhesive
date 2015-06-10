

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfLogSetting
    {
        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public InvokeInfoSetting InvokeInfoSetting { get; set; }

        [DataMember]
        public MessageInfoSetting MessageInfoSetting { get; set; }

        [DataMember]
        public StartInfoSetting StartInfoSetting { get; set; }

        [DataMember]
        public ExceptionInfoSetting ExceptionInfoSetting { get; set; }
    }
}
