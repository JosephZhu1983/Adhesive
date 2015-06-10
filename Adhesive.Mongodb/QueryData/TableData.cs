using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class TableData
    {
        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public List<Table> Tables { get; set; }

        [DataMember]
        public string PkColumnName { get; set; }

        [DataMember]
        public string PkColumnDisplayName { get; set; }

    }
}
