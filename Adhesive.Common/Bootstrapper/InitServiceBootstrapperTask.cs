
using Microsoft.Practices.Unity;

namespace Adhesive.Common
{
    public abstract class InitServiceBootstrapperTask : BootstrapperTask
    {
        public InitServiceBootstrapperTask(IUnityContainer container) : base(container) { }

        public override int Order
        {
            get
            {
                return 4;
            }
        }
    }
}
