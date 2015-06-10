

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;
    using Adhesive.Mongodb;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfClientEndpointConfig : WcfEndpointConfig
    {
        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EA")]
        [MongodbPresentationItem(DisplayName = "端点地址")]
        public string EndpointAddress { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "SFN")]
        [MongodbPresentationItem(DisplayName = "服务场名")]
        public string ServerFarmName { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "ST")]
        [MongodbPresentationItem(DisplayName = "服务类型名")]
        public string ServiceType { get; set; }
    }
}
