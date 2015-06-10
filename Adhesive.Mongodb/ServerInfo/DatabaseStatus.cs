
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class DatabaseStatus
    {
        [DataMember]
        public double AverageObjectSize { get; set; }

        [DataMember]
        public int CollectionCount { get; set; }

        [DataMember]
        public long DataSize { get; set; }

        [DataMember]
        public int ExtentCount { get; set; }

        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public int IndexCount { get; set; }

        [DataMember]
        public long IndexSize { get; set; }

        [DataMember]
        public long ObjectCount { get; set; }

        [DataMember]
        public long StorageSize { get; set; }
    }
}
