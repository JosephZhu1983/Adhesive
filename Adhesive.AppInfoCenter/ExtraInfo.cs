
using System.Collections.Generic;
using Adhesive.Common;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter
{
    public class ExtraInfo : DebugBase
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "DF1")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "单选过滤1")]
        public string DropDownListFilterItem1 { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "DF2")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "单选过滤2")]
        public string DropDownListFilterItem2 { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "CF1")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter, DisplayName = "多选过滤1")]
        public string CheckBoxListFilterItem1 { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "CF2")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter, DisplayName = "多选过滤2")]
        public string CheckBoxListFilterItem2 { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "TF1")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.TextBoxFilter, DisplayName = "文本搜索1")]
        public string TextBoxFilterItem1 { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "TF2")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.TextBoxFilter, DisplayName = "文本搜索2")]
        public string TextBoxFilterItem2 { get; set; }

        [MongodbPresentationItem(DisplayName = "其它数据")]
        [MongodbPersistenceItem(ColumnName = "DI")]
        public Dictionary<string, string> DisplayItems { get; set; }
    }
}
