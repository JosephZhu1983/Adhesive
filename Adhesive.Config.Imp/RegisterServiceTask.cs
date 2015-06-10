
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.Config.Imp
{
    public class RegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }
        public override TaskContinuation Execute()
        {
            container.RegisterTypeAsSingleton<IConfigService, ConfigService>();
            return TaskContinuation.Continue;
        }
    }
}
