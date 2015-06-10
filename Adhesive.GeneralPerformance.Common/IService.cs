using System.ServiceModel;

namespace Adhesive.GeneralPerformance.Common
{
    [ServiceContract(Namespace = "Adhesive.GeneralPerformance")]
    public interface IService
    {
        [OperationContract]
        void SubmitPagePerformanceInfo(GeneralPerformanceInfo info);

        [OperationContract]
        void SubmitWcfClientPerformanceInfo(GeneralPerformanceInfo info);

        [OperationContract]
        void SubmitWcfServerPerformanceInfo(GeneralPerformanceInfo info);
    }
}
