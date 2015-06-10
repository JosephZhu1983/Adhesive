

using System.Runtime.Serialization;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfEndpointConfig
    {
        [DataMember]
        [MongodbPersistenceItem(ColumnName = "SCT")]
        [MongodbPresentationItem(DisplayName = "服务契约名")]
        public string ServiceContractType { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EBX")]
        [MongodbPresentationItem(DisplayName = "端点绑定Xml")]
        public string EndpointBindingXml { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EBT")]
        [MongodbPresentationItem(DisplayName = "端点绑定类型名")]
        public string EndpointBindingType { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EN")]
        [MongodbPresentationItem(DisplayName = "端点名")]
        public string EndpointName { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EPT")]
        [MongodbPresentationItem(DisplayName = "端点协议")]
        public string EndpointProtocol { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EP")]
        [MongodbPresentationItem(DisplayName = "端点端口")]
        public int EndpointPort { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EBHX")]
        [MongodbPresentationItem(DisplayName = "端点行为Xml")]
        public string EndpointBehaviorXml { get; set; }
    }
}
