
//using Adhesive.Mongodb;

//namespace Adhesive.AppInfoCenter.Imp
//{
//    [MongodbPersistenceEntity("State", DisplayName = "应用程序状态", Name = "Application", ExpireDays = 10)]
//    public class ApplicationStateInfo : BaseInfo
//    {
//        [MongodbPersistenceItem(ColumnName = "PN")]
//        [MongodbPresentationItem(DisplayName = "进程名")]
//        public string ProcessName { get; set; }

//        [MongodbPersistenceItem(ColumnName = "WS")]
//        [MongodbPresentationItem(DisplayName = "工作集内存")]
//        public long WorkingSet64 { get; set; }

//        [MongodbPersistenceItem(ColumnName = "NSM")]
//        [MongodbPresentationItem(DisplayName = "非分页系统内存")]
//        public long NonpagedSystemMemorySize64 { get; set; }

//        [MongodbPersistenceItem(ColumnName = "MS")]
//        [MongodbPresentationItem(DisplayName = "分页内存")]
//        public long PagedMemorySize64 { get; set; }

//        [MongodbPersistenceItem(ColumnName = "PS")]
//        [MongodbPresentationItem(DisplayName = "分页的系统内存")]
//        public long PagedSystemMemorySize64 { get; set; }

//        [MongodbPersistenceItem(ColumnName = "PM")]
//        [MongodbPresentationItem(DisplayName = "私有内存")]
//        public long PrivateMemorySize64 { get; set; }

//        [MongodbPersistenceItem(ColumnName = "MV")]
//        [MongodbPresentationItem(DisplayName = "虚拟内存")]
//        public long VirtualMemorySize64 { get; set; }

//        [MongodbPersistenceItem(ColumnName = "CW")]
//        [MongodbPresentationItem(DisplayName = "工作线程")]
//        public int CurrentWorkThreadCount { get; set; }

//        [MongodbPersistenceItem(ColumnName = "CC")]
//        [MongodbPresentationItem(DisplayName = "完成端口线程")]
//        public int CurrentCompletionPortThreadCount { get; set; }
//    }
//}
