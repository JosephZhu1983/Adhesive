
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Aic", DisplayName = "Mvc未处理异常", Name = "MvcException")]
    public class MvcUnhandledExceptionInfo : WebSiteUnhandledExceptionInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "CTN")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "控制器类型名", ShowInTableView = true)]
        public string ControllerTypeName { get; set; }
    }
}
