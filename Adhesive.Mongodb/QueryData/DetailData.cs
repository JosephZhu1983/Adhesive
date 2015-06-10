using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class DetailData
    {
        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public List<Detail> Data { get; set; }

        [DataMember]
        public string DatabasePrefix { get; set; }

        [DataMember]
        public string PkColumnName { get; set; }
    }
}
