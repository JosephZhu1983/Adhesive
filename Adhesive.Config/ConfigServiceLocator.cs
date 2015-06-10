
namespace Adhesive.Config
{
    public class ConfigServiceLocator
    {
        public static IConfigServer GetService()
        {
            return (IConfigServer)new ConfigServiceRealProxy().GetTransparentProxy();
        }
    }
}
