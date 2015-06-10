
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public abstract class ExceptionInfo : AbstractInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "ETN")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter, DisplayName = "异常类型名", ShowInTableView = true)]
        public string ExceptionTypeName { get; set; }

        [MongodbPresentationItem(DisplayName = "异常描述", ShowInTableView = true)]
        [MongodbPersistenceItem(ColumnName = "D")]
        public string Description { get; set; }

        [MongodbPresentationItem(DisplayName = "异常")]
        [MongodbPersistenceItem(ColumnName = "E")]
        public AppException Exception { get; set; }

        [MongodbPresentationItem(DisplayName = "异常消息", ShowInTableView = true)]
        [MongodbPersistenceItem(ColumnName = "EM")]
        public string ExceptionMessage { get; set; }
    }
}
