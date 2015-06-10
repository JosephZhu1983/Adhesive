
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    public class AppException
    {
        [MongodbPersistenceItem(ColumnName = "ETN")]
        [MongodbPresentationItem(DisplayName = "异常类型名")]
        public string ExceptionTypeName { get; set; }

        [MongodbPersistenceItem(ColumnName = "HL")]
        [MongodbPresentationItem(DisplayName = "帮助链接")]
        public string HelpLink { get; set; }

        [MongodbPersistenceItem(ColumnName = "IE")]
        [MongodbPresentationItem(DisplayName = "内部异常")]
        public AppException InnerException { get; set; }

        [MongodbPersistenceItem(ColumnName = "M")]
        [MongodbPresentationItem(DisplayName = "异常消息")]
        public string Message { get; set; }

        [MongodbPersistenceItem(ColumnName = "S")]
        [MongodbPresentationItem(DisplayName = "异常来源")]
        public string Source { get; set; }

        [MongodbPersistenceItem(ColumnName = "ST")]
        [MongodbPresentationItem(DisplayName = "异常堆栈")]
        public string StackTrace { get; set; }

        [MongodbPersistenceItem(ColumnName = "TS")]
        [MongodbPresentationItem(DisplayName = "出现异常的方法")]
        public string TargetSite { get; set; }
    }
}
