
using Microsoft.Practices.Unity;

namespace Adhesive.Common
{
    public abstract class RegisterServiceBootstrapperTask : BootstrapperTask
    {
        public RegisterServiceBootstrapperTask(IUnityContainer container) : base(container) { }

        public override int Order
        {
            get
            {
                return 1;
            }
        }
    }
}
