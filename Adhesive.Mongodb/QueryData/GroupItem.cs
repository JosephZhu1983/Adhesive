
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class GroupItem
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Dictionary<GroupItemValuePair, int> Values { get; set; }
    }

    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class GroupItemValuePair
    {
        [DataMember]
        public object Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }
    }
}
