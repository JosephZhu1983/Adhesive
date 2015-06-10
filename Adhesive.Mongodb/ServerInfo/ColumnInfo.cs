
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class TextboxFilterColumnInfo
    {
        [DataMember]
        public string ColumnName { get; set; }
    }

    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class ListFilterColumnInfo
    {
        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public List<ItemPair> DistinctValues { get; set; }
    }

    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class CascadeFilterColumnInfo
    {
        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public List<string> DistinctValues { get; set; }
    }
}
