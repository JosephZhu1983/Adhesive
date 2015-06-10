

using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class MongodbEnumColumnDescription
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Dictionary<string, string> EnumItems { get; set; }

    }
}
