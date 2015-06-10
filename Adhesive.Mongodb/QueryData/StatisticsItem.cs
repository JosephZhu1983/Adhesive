
using System;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class StatisticsItem
    {
        [DataMember]
        public int Value { get; set; }

        [DataMember]
        public DateTime BeginTime { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }
    }
}
