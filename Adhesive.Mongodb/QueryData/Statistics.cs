
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class Statistics
    {
        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public List<StatisticsItem> StatisticsItems { get; set; }
    }
}
