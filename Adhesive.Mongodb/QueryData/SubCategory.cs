

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [Serializable]
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class SubCategory
    {
        [DataMember]
        public string TypeFullName { get; set; }

        [DataMember]
        public string DatabasePrefix { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public List<string> TableNames { get; set; }
    }
}
