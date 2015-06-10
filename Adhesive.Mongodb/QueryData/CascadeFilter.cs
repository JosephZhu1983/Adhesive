

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    [Serializable]
    public class CascadeFilter : Filter
    {
        [DataMember]
        public CascadeFilterType CascadeFilterType { get; set; }

        [DataMember]
        public Dictionary<string, List<string>> Items { get; set; }
    }
}
