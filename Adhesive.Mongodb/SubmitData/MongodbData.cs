
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class MongodbData
    {
        [DataMember]
        public string TypeFullName { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}
