using System;
using Adhesive.Mongodb;

namespace Adhesive.Test.WebApp
{
    public enum Status
    {
        借出 = 1,
        归还 = 2,
        丢失 = 3,
    }

    [MongodbPersistenceEntity("Test", DisplayName = "书籍借阅信息", Name = "Book")]
    public class Book
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.AscendingAndUnique, IsPrimaryKey = true)]
        [MongodbPresentationItem(ShowInTableView = true, DisplayName = "主键")]
        public string ID { get; set; }

        [MongodbPersistenceItem(IsTableName = true)]
        [MongodbPresentationItem(DisplayName = "部门", ShowInTableView = true)]
        public string DeptName { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Descending, IsTimeColumn = true, ColumnName = "T")]
        [MongodbPresentationItem(MongodbSortOption = MongodbSortOption.Descending, DisplayName = "时间", ShowInTableView = true)]
        public DateTime ServerTime { get; set; }

        [MongodbPresentationItem(DisplayName = "书名", ShowInTableView = true)]
        public string Name { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "借书者",  ShowInTableView = true, MongodbFilterOption = MongodbFilterOption.TextBoxFilter)]
        public string UserName { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "状态变化", ShowInTableView = true, MongodbFilterOption = MongodbFilterOption.DropDownListFilter)]
        public Status Status { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending)]
        [MongodbPresentationItem(DisplayName = "书籍分类", ShowInTableView = true, MongodbFilterOption = MongodbFilterOption.CheckBoxListFilter)]
        public string Category { get; set; }

        [MongodbPresentationItem(DisplayName = "备注")]
        public string Memo { get; set; }
    }
}