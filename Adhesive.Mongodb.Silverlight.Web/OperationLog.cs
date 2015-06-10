using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adhesive.Mongodb.Silverlight.Web
{
    [MongodbPersistenceEntity("OperationLog", DisplayName = "通用数据后台操作日志", Name = "MongodbWeb")]
    public class OperationLog
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.AscendingAndUnique, IsPrimaryKey = true)]
        [MongodbPresentationItem(ShowInTableView = true, DisplayName = "主键")]
        public string ID { get; set; }

        [MongodbPersistenceItem(IsTableName = true, ColumnName = "AN")]
        [MongodbPresentationItem(DisplayName = "账号")]
        public string AccountName { get; set; }

        [MongodbPersistenceItem(ColumnName = "ARL", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "姓名", ShowInTableView = true, MongodbFilterOption = MongodbFilterOption.TextBoxFilter)]
        public string AccountRealName { get; set; }

        [MongodbPersistenceItem(ColumnName = "A", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "行为", ShowInTableView = true, MongodbFilterOption = MongodbFilterOption.DropDownListFilter)]
        public string Action { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Descending, IsTimeColumn = true, ColumnName = "T")]
        [MongodbPresentationItem(MongodbSortOption = MongodbSortOption.Descending, DisplayName = "时间", ShowInTableView = true)]
        public DateTime ServerTime { get; set; }

        [MongodbPersistenceItem(ColumnName = "C", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "分类", ShowInTableView = true, MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelOne)]
        public string CategoryName { get; set; }

        [MongodbPersistenceItem(ColumnName = "D", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "数据库", ShowInTableView = true, MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelTwo)]
        public string DatabaseName { get; set; }

        [MongodbPersistenceItem(ColumnName = "TN", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "表", ShowInTableView = true, MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelThree)]
        public string TableName { get; set; }

        [MongodbPresentationItem(DisplayName = "备注")]
        [MongodbPersistenceItem(ColumnName = "M")]
        public string ActionMemo { get; set; }
    }
}