using Adhesive.Common;

namespace Adhesive.DistributedService
{
    public class WcfService
    {
        public static IWcfServiceLocator WcfServiceLocator
        {
            get
            {
                return LocalServiceLocator.GetService<IWcfServiceLocator>();
            }
        }
    }
}
