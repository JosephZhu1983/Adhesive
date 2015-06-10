

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Serialization;
    using Adhesive.Mongodb;

    [DataContract(Namespace = "Adhesive.DistributedService")]
    public class WcfServiceEndpointConfig : WcfEndpointConfig
    {
        [DataMember]
        [MongodbPersistenceItem(ColumnName = "EBN")]
        [MongodbPresentationItem(DisplayName = "端点绑定名")]
        public string EndpointBindingName { get; set; }
    }
}
