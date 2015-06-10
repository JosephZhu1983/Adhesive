
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.MemoryQueue.Imp
{
    public class RegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }

        public override TaskContinuation Execute()
        {
            container.RegisterTypeAsTransient<IMemoryQueueService, MemoryQueueService>(); ;

            return TaskContinuation.Continue;
        }
    }
}
