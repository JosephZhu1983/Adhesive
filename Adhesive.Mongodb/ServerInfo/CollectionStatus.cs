
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class CollectionStatus
    {
        [DataMember]
        public DateTime LastEnsureIndexTime { get; set; }

        [DataMember]
        public double AverageObjectSize { get; set; }

        [DataMember]
        public long DataSize { get; set; }

        [DataMember]
        public int ExtentCount { get; set; }

        [DataMember]
        public int Flags { get; set; }

        [DataMember]
        public int IndexCount { get; set; }

        [DataMember]
        public List<IndexStatus> IndexStatusList { get; set; }

        [DataMember]
        public long LastExtentSize { get; set; }

        [DataMember]
        public string Namespace { get; set; }

        [DataMember]
        public long ObjectCount { get; set; }

        [DataMember]
        public double PaddingFactor { get; set; }

        [DataMember]
        public long StorageSize { get; set; }

        [DataMember]
        public long TotalIndexSize { get; set; }
    }
}
