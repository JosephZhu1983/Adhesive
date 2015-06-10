
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class ServerInfo
    {
        [DataMember]
        public MongodbServerUrl Url { get; set; }

        [DataMember]
        public List<DatabaseInfo> Databases { get; set; }

        [DataMember]
        public List<MongodbDatabaseDescription> Descriptions { get; set; }
    }
}
