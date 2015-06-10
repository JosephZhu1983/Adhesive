

using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class CollectionInfo
    {
        [DataMember]
        public string CollectionName { get; set; }

        [DataMember]
        public CollectionStatus CollectionStatus { get; set; }

        [DataMember]
        public List<TextboxFilterColumnInfo> TextboxFilterColumns { get; set; }

        [DataMember]
        public List<ListFilterColumnInfo> ListFilterColumns { get; set; }

        [DataMember]
        public List<CascadeFilterColumnInfo> CascadeFilterColumns { get; set; }
    }
}
