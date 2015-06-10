using System;
using System.Collections.Generic;

namespace Adhesive.Mongodb.Test
{
    public class Ext2
    {
        [MongodbPersistenceItem(ColumnName = "NormalColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext2.NormalColumn", Description = "扩展类2中普通的列")]
        public DateTime NormalColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "EnumColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext2.EnumColumn", Description = "扩展类2中枚举的列")]
        public Enum3 EnumColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ShownInStateColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext2.ShownInStateColumn", Description = "扩展类2中显示在状态图中的列")]
        public int ShownInStateColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "CheckBoxFilterColumn33", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Ext2.CheckBoxFilterColumn", Description = "扩展类2中多选过滤的列", MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter, ShowInTableView = true)]
        public string CheckBoxFilterColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DropDownListFilterColumn33", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Ext2.DropDownListFilterColumn", Description = "扩展类2中单选过滤的列", MongodbFilterOption = MongodbFilterOption.DropDownListFilter, ShowInTableView = true)]
        public string DropDownListFilterColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "TextboxFilterColumn33", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Ext2.TextboxFilterColumn", Description = "扩展类2中文本搜索的列", MongodbFilterOption = MongodbFilterOption.TextBoxFilter)]
        public string TextboxFilterColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "IgnoreColumn33", IsIgnore = true)]
        [MongodbPresentationItem(DisplayName = "Ext.IgnoreColumn", Description = "扩展类2中忽略的列")]
        public string IgnoreColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DictionaryColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext2.DictionaryColumn", Description = "扩展类2中的字典列")]
        public Dictionary<string, int> DictionaryColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ListColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext2.ListColumn", Description = "扩展类2中的列表列")]
        public List<int> ListColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtListColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext.ExtListColumn", Description = "扩展类2中的扩展列表列")]
        public List<ExtItem> ExtListColumn3 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtDictionaryColumn33")]
        [MongodbPresentationItem(DisplayName = "Ext2.ExtDictionaryColumn", Description = "扩展类2中的扩展字典列")]
        public Dictionary<string, ExtItem> ExtDictionaryColumn3 { get; set; }
    }
}
