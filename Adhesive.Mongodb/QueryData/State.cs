
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class State
    {
        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<StateItem> StateItems { get; set; }
    }
}
