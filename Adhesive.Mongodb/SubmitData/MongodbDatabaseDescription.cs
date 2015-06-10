
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class MongodbDatabaseDescription
    {
        [DataMember]
        public bool SentToServer { get; set; }

        [DataMember]
        public string TypeFullName { get; set; }

        [DataMember]
        public string DatabasePrefix { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public int ExpireDays { get; set; }

        [DataMember]
        public List<MongodbColumnDescription> MongodbColumnDescriptionList { get; set; }

        [DataMember]
        public List<MongodbEnumColumnDescription> MongodbEnumColumnDescriptionList { get; set; }
    }
}
