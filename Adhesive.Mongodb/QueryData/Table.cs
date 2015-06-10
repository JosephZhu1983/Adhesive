
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class Table
    {
        //[DataMember]
        //public int TotalCount { get; set; }

        [DataMember]
        public string DatabasePrefix { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public List<Dictionary<string, string>> Data { get; set; }
    }
}
