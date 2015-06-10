
using Adhesive.Mongodb.Server;
namespace Adhesive.Mongodb.Imp
{
    public class MongodbServiceLocator
    {
        public static IMongodbServer GetService()
        {
            return (IMongodbServer)new MongodbServiceRealProxy().GetTransparentProxy();
        }
    }
}
