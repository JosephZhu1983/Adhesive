
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Aic", DisplayName = "网站未处理异常", Name = "WebsiteException")]
    public class WebSiteUnhandledExceptionInfo : ExceptionInfo
    {
    }
}
