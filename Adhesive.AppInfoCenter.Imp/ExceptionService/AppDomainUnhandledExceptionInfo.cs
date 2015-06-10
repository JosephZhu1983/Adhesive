
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Aic", DisplayName = "应用程序域未处理异常", Name = "AppDomainException")]
    public class AppDomainUnhandledExceptionInfo : ExceptionInfo
    {
        [MongodbPersistenceItem(ColumnName = "IT")]
        [MongodbPresentationItem(DisplayName = "是否终止程序")]
        public bool IsTerminating { get; set; }
    }
}
