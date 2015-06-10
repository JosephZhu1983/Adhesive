
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class DatabaseInfo
    {
        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public DateTime DatabaseDate { get; set; }

        [DataMember]
        public string DatabasePrefix { get; set; }

        [DataMember]
        public DatabaseStatus DatabaseStatus { get; set; }

        [DataMember]
        public List<CollectionInfo> Collections { get; set; }
    }
}
