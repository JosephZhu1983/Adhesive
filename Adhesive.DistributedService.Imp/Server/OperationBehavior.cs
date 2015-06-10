

namespace Adhesive.DistributedService.Imp
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    internal class OperationBehavior : IOperationBehavior
    {
        protected OperationInvoker CreateInvoker(IOperationInvoker oldInvoker)
        {
            return new OperationInvoker(oldInvoker);
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        { }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            IOperationInvoker oldInvoker = dispatchOperation.Invoker;
            dispatchOperation.Invoker = CreateInvoker(oldInvoker);
        }

        public void Validate(OperationDescription operationDescription)
        { }
    }
}
