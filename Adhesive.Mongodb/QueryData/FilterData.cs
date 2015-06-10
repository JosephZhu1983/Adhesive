using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    [Serializable]
    public class FilterData
    {
        [DataMember]
        public List<TextboxFilter> TextboxFilters { get; set; }

        [DataMember]
        public List<ListFilter> ListFilters { get; set; }

        [DataMember]
        public List<CascadeFilter> CascadeFilters { get; set; }
    }
}
