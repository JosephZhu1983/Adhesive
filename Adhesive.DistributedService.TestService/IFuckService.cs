using System.ServiceModel;

namespace Adhesive.DistributedService.TestService
{
    [ServiceContract(Namespace = "Adhesive.DistributedService.TestService")]
    public interface IFuckService
    {
        [OperationContract]
        string YouWannaFuckWho(string name, int time);
    }
}
