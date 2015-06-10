
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Aic", DisplayName = "网站页面执行性能", Name = "WebsiteSlowExec")]
    public class WebsitePageExecutionInfo : AbstractInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName ="ET")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "页面执行时间分类")]
        public ExecutionTime ExecutionTime { get; set; }

        [MongodbPersistenceItem(ColumnName = "PET")]
        [MongodbPresentationItem(DisplayName = "页面执行毫秒", ShowInTableView = true)]
        public long PageExecutionTime { get; set; }
    }
}
