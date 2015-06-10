
using Microsoft.Practices.Unity;

namespace Adhesive.Common
{
    public abstract class BootstrapperTask : Disposable
    {
        protected IUnityContainer container;

        public BootstrapperTask(IUnityContainer container)
        {
            this.container = container;
        }

        public virtual int Order
        {
            get
            {
                return 0;
            }
        }

        public virtual string Description
        {
            get
            {
                return "";
            }
        }

        public abstract TaskContinuation Execute();
    }
}
