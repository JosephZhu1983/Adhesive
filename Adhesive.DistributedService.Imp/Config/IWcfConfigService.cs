

namespace Adhesive.DistributedService.Imp
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "Adhesive.DistributedService")]
    public interface IWcfConfigService
    {
        [OperationContract]
        WcfServiceConfig GetWcfService(string serviceType, string serviceContractVersion, string machineIP);

        [OperationContract]
        WcfClientEndpointConfig GetWcfClientEndpoint(string serviceContractType, string serviceContractVersion, string machineIP);

        [OperationContract]
        WcfClientSetting GetClientSetting(string serviceContractType, string machineIP);

        [OperationContract]
        WcfServerSetting GetServerSetting(string serviceType, string machineIP);
    }
}
