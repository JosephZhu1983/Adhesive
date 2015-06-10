
using Adhesive.Common;

namespace Adhesive.Mongodb
{
    public class MongodbService
    {
        public static IMongodbInsertService MongodbInsertService
        {
            get
            {
                return LocalServiceLocator.GetService<IMongodbInsertService>();
            }
        }

        public static IMongodbQueryService MongodbQueryService
        {
            get
            {
                return LocalServiceLocator.GetService<IMongodbQueryService>();
            }
        }
    }
}
