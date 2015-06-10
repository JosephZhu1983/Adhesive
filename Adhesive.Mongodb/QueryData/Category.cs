using System;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [Serializable]
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class Category
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<SubCategory> SubCategoryList { get; set; }
    }
}
