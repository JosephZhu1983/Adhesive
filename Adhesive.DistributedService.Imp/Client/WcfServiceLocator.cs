

namespace Adhesive.DistributedService.Imp
{
    public class WcfServiceLocator : IWcfServiceLocator
    {
        public T GetService<T>() where T : class
        {
            return (T)(new ServiceRealProxy<T>().GetTransparentProxy());
        }

        public T GetSafeService<T>() where T : class
        {
            return (T)(new ServiceRealProxy<T>(true).GetTransparentProxy());
        }
    }
}
