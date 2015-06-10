using System;
using System.Collections.Generic;

namespace Adhesive.Mongodb.Test
{
    public class Ext
    {
        [MongodbPersistenceItem(ColumnName = "NormalColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.NormalColumn", Description = "扩展类中普通的列")]
        public DateTime NormalColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "EnumColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.EnumColumn", Description = "扩展类中枚举的列")]
        public Enum3 EnumColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ShownInStateColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.ShownInStateColumn", Description = "扩展类中显示在状态图中的列")]
        public int ShownInStateColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "CheckBoxFilterColumn33", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Ext.CheckBoxFilterColumn", Description = "扩展类中多选过滤的列", MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter, ShowInTableView = true)]
        public string CheckBoxFilterColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DropDownListFilterColumn33", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Ext.DropDownListFilterColumn", Description = "扩展类中单选过滤的列", MongodbFilterOption = MongodbFilterOption.DropDownListFilter, ShowInTableView = true)]
        public string DropDownListFilterColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "TextboxFilterColumn33", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Ext.TextboxFilterColumn", Description = "扩展类中文本搜索的列", MongodbFilterOption = MongodbFilterOption.TextBoxFilter)]
        public string TextboxFilterColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "IgnoreColumn33", IsIgnore = true)]
        [MongodbPresentationItem(DisplayName = "Ext.IgnoreColumn", Description = "扩展类中忽略的列")]
        public string IgnoreColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DictionaryColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.DictionaryColumn", Description = "扩展类中的字典列")]
        public Dictionary<string, string> DictionaryColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ListColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.ListColumn", Description = "扩展类中的列表列")]
        public List<int> ListColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtListColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.ExtListColumn", Description = "扩展类中的扩展列表列")]
        public List<ExtItem> ExtListColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtDictionaryColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.ExtDictionaryColumn", Description = "扩展类中的扩展字典列")]
        public Dictionary<string, ExtItem> ExtDictionaryColumn3 { get; set; }
    }
}
