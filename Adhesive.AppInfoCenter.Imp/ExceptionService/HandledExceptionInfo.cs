
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Aic", DisplayName = "已处理异常", Name = "Exception")]
    public class HandledExceptionInfo : ExceptionInfo
    {
    }
}
