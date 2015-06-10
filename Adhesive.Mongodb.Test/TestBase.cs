using System;
using System.Collections.Generic;

namespace Adhesive.Mongodb.Test
{
    public class TestBase
    {
        [MongodbPersistenceItem(ColumnName = "PkColumn11", IsPrimaryKey = true, MongodbIndexOption = MongodbIndexOption.AscendingAndUnique)]
        [MongodbPresentationItem(DisplayName = "TestBase.PkColumn", Description = "主键列", ShowInTableView = true)]
        public string PkColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "NormalColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.NormalColumn", Description = "基类中普通的列")]
        public string NormalColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "EnumColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.EnumColumn", Description = "基类中枚举的列")]
        public Enum1 EnumColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "TableNameColumn11", IsTableName = true)]
        [MongodbPresentationItem(DisplayName = "TestBase.TableNameColumn", Description = "基类中作为表名的列", ShowInTableView = true)]
        public string TableNameColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ShownInStateColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.ShownInStateColumn", Description = "基类中显示在状态图中的列")]
        public long ShownInStateColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "CheckBoxFilterColumn11", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "TestBase.CheckBoxFilterColumn", Description = "基类中多选过滤的列", MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter, ShowInTableView = true)]
        public string CheckBoxFilterColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DropDownListFilterColumn11", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "TestBase.DropDownListFilterColumn", Description = "基类中单选过滤的列", MongodbFilterOption = MongodbFilterOption.DropDownListFilter, ShowInTableView = true)]
        public string DropDownListFilterColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "TextboxFilterColumn11", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "TestBase.TextboxFilterColumn", Description = "基类中文本搜索的列", MongodbFilterOption = MongodbFilterOption.TextBoxFilter)]
        public string TextboxFilterColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "StatTimeColumn2", IsTimeColumn = true, MongodbIndexOption = MongodbIndexOption.Descending)]
        [MongodbPresentationItem(DisplayName = "TestBase.StatTimeColumn", Description = "基类中作为统计时间的列", ShowInTableView = true, MongodbSortOption = MongodbSortOption.Descending)]
        public DateTime StatTimeColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "IgnoreColumn11", IsIgnore = true)]
        [MongodbPresentationItem(DisplayName = "TestBase.IgnoreColumn", Description = "基类中忽略的列")]
        public string IgnoreColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DictionaryColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.DictionaryColumn", Description = "基类中的字典列")]
        public Dictionary<string, string> DictionaryColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ListColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.ListColumn", Description = "基类中的列表列")]
        public List<string> ListColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "CascadeFilterColumnLevelOne11", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "TestBase.CascadeFilterColumnLevelOne", Description = "基类中第一级联过滤列", MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelOne, ShowInTableView = true)]
        public string CascadeFilterColumnLevelOne1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "CascadeFilterColumnLevelTwo11", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "TestBase.CascadeFilterColumnLevelTwo", Description = "基类中第二级联过滤列", MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelTwo, ShowInTableView = true)]
        public string CascadeFilterColumnLevelTwo1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "CascadeFilterColumnLevelThree11", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "TestBase.CascadeFilterColumnLevelThree", Description = "基类中第三级联过滤列", MongodbCascadeFilterOption = MongodbCascadeFilterOption.LevelThree, ShowInTableView = true)]
        public string CascadeFilterColumnLevelThree1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtListColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.ExtListColumn", Description = "基类中的扩展列表列")]
        public List<ExtItem> ExtListColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtDictionaryColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.ExtDictionaryColumn", Description = "基类中的扩展字典列")]
        public Dictionary<string, ExtItem> ExtDictionaryColumn1 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtColumn11")]
        [MongodbPresentationItem(DisplayName = "TestBase.ExtColumn", Description = "基类中的扩展列")]
        public Ext ExtColumn1 { get; set; }
    }
}
