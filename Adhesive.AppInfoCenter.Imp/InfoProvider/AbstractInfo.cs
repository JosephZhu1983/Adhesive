
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public abstract class AbstractInfo : BaseInfo
    {
        [MongodbPresentationItem(MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelOne, DisplayName = "模块名", ShowInTableView = true)]
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "MDN")]
        public string ModuleName { get; set; }

        [MongodbPresentationItem(MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelTwo, DisplayName = "大类", ShowInTableView = true)]
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "C")]
        public string CategoryName { get; set; }

        [MongodbPresentationItem(MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelThree, DisplayName = "小类", ShowInTableView = true)]
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "SC")]
        public string SubCategoryName { get; set; }

        [MongodbPresentationItem(DisplayName = "上下文标识", ShowInTableView = true)]
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, IsContextIdentityColumn = true, ColumnName = "CI")]
        public string ContextIdentity { get; set; }

        [MongodbPresentationItem(DisplayName = "环境信息")]
        [MongodbPersistenceItem(ColumnName = "ENI")]
        public EnvironmentInfo EnvironmentInfo { get; set; }

        [MongodbPresentationItem(DisplayName = "额外信息")]
        [MongodbPersistenceItem(ColumnName = "EI")]
        public ExtraInfo ExtraInfo { get; set; }

        [MongodbPresentationItem(DisplayName = "位置信息")]
        [MongodbPersistenceItem(ColumnName = "LI")]
        public LocationInfo LocationInfo { get; set; }

        [MongodbPresentationItem(DisplayName = "Http上下文信息")]
        [MongodbPersistenceItem(ColumnName = "HCI")]
        public HttpContextInfo HttpContextInfo { get; set; }

        [MongodbPresentationItem(DisplayName = "Mvc上下文信息")]
        [MongodbPersistenceItem(ColumnName = "MCU")]
        public MvcContextInfo MvcContextInfo { get; set; }
    }
}
