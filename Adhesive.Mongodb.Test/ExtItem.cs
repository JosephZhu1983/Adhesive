using System.Collections.Generic;

namespace Adhesive.Mongodb.Test
{
    public class ExtItem
    {
        [MongodbPersistenceItem(ColumnName = "NormalColumn44")]
        [MongodbPresentationItem(DisplayName = "ExtItem.NormalColumn", Description = "项扩展类中普通的列")]
        public int NormalColumn4 { get; set; }

        [MongodbPersistenceItem(ColumnName = "EnumColumn44")]
        [MongodbPresentationItem(DisplayName = "ExtItem.EnumColumn", Description = "项扩展类中枚举的列")]
        public Enum4 EnumColumn4 { get; set; }

        [MongodbPersistenceItem(ColumnName = "IgnoreColumn44", IsIgnore = true)]
        [MongodbPresentationItem(DisplayName = "ExtItem.IgnoreColumn", Description = "项扩展类中忽略的列")]
        public string IgnoreColumn4 { get; set; }

        [MongodbPersistenceItem(ColumnName = "DictionaryColumn44")]
        [MongodbPresentationItem(DisplayName = "ExtItem.DictionaryColumn", Description = "项扩展类中的字典列")]
        public Dictionary<int, string> DictionaryColumn4 { get; set; }

        [MongodbPersistenceItem(ColumnName = "ListColumn44")]
        [MongodbPresentationItem(DisplayName = "ExtItem.ListColumn", Description = "项扩展类中的列表列")]
        public List<int> ListColumn4 { get; set; }
    }
}
