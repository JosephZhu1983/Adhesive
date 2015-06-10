
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class IndexStatus
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool Unique { get; set; }

        [DataMember]
        public string Namespace { get; set; }

        [DataMember]
        public long Size { get; set; }

    }
}
