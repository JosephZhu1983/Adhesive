

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    [Serializable]
    public class ListFilter : Filter
    {
        [DataMember]
        public ListFilterType ListFilterType { get; set; }

        [DataMember]
        public List<ItemPair> Items { get; set; }
    }
}
