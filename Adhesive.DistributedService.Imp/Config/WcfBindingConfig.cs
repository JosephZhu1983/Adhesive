

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;
    using Adhesive.Mongodb;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfBindingConfig
    {
        [DataMember]
        [MongodbPersistenceItem(ColumnName = "BN")]
        [MongodbPresentationItem(DisplayName = "绑定名")]
        public string BindingName { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "BT")]
        [MongodbPresentationItem(DisplayName = "绑定类型")]
        public string BindingType { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "BP")]
        [MongodbPresentationItem(DisplayName = "绑定优先级")]
        public int BindingPriority { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "BX")]
        [MongodbPresentationItem(DisplayName = "绑定Xml")]
        public string BindingXml { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "BP")]
        [MongodbPresentationItem(DisplayName = "绑定协议")]
        public string BindingProtocol { get; set; }
    }
}
