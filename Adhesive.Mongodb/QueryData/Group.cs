
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class Group
    {
        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public List<GroupItem> GroupItems { get; set; }
    }
}
