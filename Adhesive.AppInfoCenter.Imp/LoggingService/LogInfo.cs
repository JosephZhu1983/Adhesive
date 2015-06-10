

using Adhesive.Common;
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Aic", DisplayName = "日志", Name = "Log")]
    public class LogInfo : AbstractInfo
    {
        [MongodbPresentationItem(DisplayName = "日志消息", ShowInTableView = true)]
        [MongodbPersistenceItem(ColumnName = "M")]
        public string Message { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "L")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "日志级别", ShowInTableView = true)]
        public LogLevel LogLevel { get; set; }
    }
}
