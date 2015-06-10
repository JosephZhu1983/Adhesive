using System.Collections.Generic;

namespace Adhesive.Mongodb.Test
{
    [MongodbPersistenceEntity("Test", DisplayName = "Test", Name = "Test2")]
    public class Test : TestBase
    {
        [MongodbPersistenceItem(ColumnName = "NormalColumn22")]
        [MongodbPresentationItem(DisplayName = "Test.NormalColumn", Description = "子类中普通的列")]
        public string NormalColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "EnumColumn22")]
        [MongodbPresentationItem(DisplayName = "Test.EnumColumn", Description = "子类中枚举的列")]
        public Enum1 EnumColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ShownInStateColumn22")]
        [MongodbPresentationItem(DisplayName = "Test.ShownInStateColumn", Description = "子类中显示在状态图中的列")]
        public int ShownInStateColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "CheckBoxFilterColumn22", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Test.CheckBoxFilterColumn", Description = "子类中多选过滤的列", MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter, ShowInTableView = true)]
        public Enum2 CheckBoxFilterColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DropDownListFilterColumn22", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Test.DropDownListFilterColumn", Description = "子类中单选过滤的列", MongodbFilterOption = MongodbFilterOption.DropDownListFilter, ShowInTableView = true)]
        public Enum2 DropDownListFilterColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "TextboxFilterColumn22", MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "Test.TextboxFilterColumn", Description = "子类中文本搜索的列", MongodbFilterOption = MongodbFilterOption.TextBoxFilter)]
        public string TextboxFilterColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "IgnoreColumn22", IsIgnore = true)]
        [MongodbPresentationItem(DisplayName = "Test.IgnoreColumn", Description = "子类中忽略的列")]
        public string IgnoreColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DictionaryColumn22")]
        [MongodbPresentationItem(DisplayName = "Test.DictionaryColumn", Description = "子类中的字典列")]
        public Dictionary<int, string> DictionaryColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ListColumn22")]
        [MongodbPresentationItem(DisplayName = "Test.ListColumn", Description = "子类中的列表列")]
        public List<int> ListColumn2 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ExtColumn22")]
        [MongodbPresentationItem(DisplayName = "Test.ExtColumn", Description = "子类中的扩展列")]
        public Ext2 ExtColumn2 { get; set; }
    }
}
