

using System.Collections.Generic;
using System.Runtime.Serialization;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfServiceConfig
    {
        [DataMember]
        [MongodbPersistenceItem(ColumnName = "ST")]
        [MongodbPresentationItem(DisplayName = "服务类型名")]
        public string ServiceType { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "SBX")]
        [MongodbPresentationItem(DisplayName = "服务行为Xml")]
        public string ServiceBehaviorXml { get; set; }

        [DataMember]
        [MongodbPersistenceItem(ColumnName = "E")]
        [MongodbPresentationItem(DisplayName = "服务端点信息")]
        public List<WcfServiceEndpointConfig> Endpoints { get; set; }
    }
}
