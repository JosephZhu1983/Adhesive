
//using System;
//using Adhesive.Mongodb;

//namespace Adhesive.DistributedService.Imp
//{
//    public abstract class WcfStateItem
//    {
//        [MongodbPersistenceItem(ColumnName = "CRC")]
//        [MongodbPresentationItem(DisplayName = "当前请求数量")]
//        public long CurrentRequestCount { get; set; }

//        [MongodbPersistenceItem(ColumnName = "MRC")]
//        [MongodbPresentationItem(DisplayName = "最大请求数量")]
//        public long MaxRequestCount { get; set; }

//        [MongodbPersistenceItem(ColumnName = "MRCO")]
//        [MongodbPresentationItem(DisplayName = "最大请求数量发生在")]
//        public DateTime MaxRequestCountOccur { get; set; }

//        [MongodbPersistenceItem(ColumnName = "TRC")]
//        [MongodbPresentationItem(DisplayName = "总共请求数量")]
//        public long TotalRequestCount { get; set; }

//        [MongodbPersistenceItem(ColumnName = "TERC")]
//        [MongodbPresentationItem(DisplayName = "总共错误的请求数量")]
//        public long TotalErrorRequestCount { get; set; }

//        [MongodbPersistenceItem(ColumnName = "LERET")]
//        [MongodbPresentationItem(DisplayName = "上一次错误请求执行时间")]
//        public long LastErrorRequestExecutionTime { get; set; }

//        [MongodbPersistenceItem(ColumnName = "LERETO")]
//        [MongodbPresentationItem(DisplayName = "上一次错误请求执行发生在")]
//        public DateTime LastErrorRequestExecutionTimeOccur { get; set; }

//        [MongodbPersistenceItem(ColumnName = "TRET")]
//        [MongodbPresentationItem(DisplayName = "总共请求执行时间")]
//        public long TotalRequestExecutionTime { get; set; }

//        [MongodbPersistenceItem(ColumnName = "ARET")]
//        [MongodbPresentationItem(DisplayName = "平均请求执行时间")]
//        public long AverageRequestExecutionTime { get; set; }

//        [MongodbPersistenceItem(ColumnName = "MRET")]
//        [MongodbPresentationItem(DisplayName = "最大请求执行时间")]
//        public long MaxRequestExecutionTime { get; set; }

//        [MongodbPersistenceItem(ColumnName = "MRETO")]
//        [MongodbPresentationItem(DisplayName = "最大请求执行发生在")]
//        public DateTime MaxRequestExecutionTimeOccur { get; set; }

//        [MongodbPersistenceItem(ColumnName = "LRET")]
//        [MongodbPresentationItem(DisplayName = "上一次请求执行时间")]
//        public long LastRequestExecutionTime { get; set; }

//        [MongodbPersistenceItem(ColumnName = "LRETO")]
//        [MongodbPresentationItem(DisplayName = "上一次请求执行发生在")]
//        public DateTime LastRequestExecutionTimeOccur { get; set; }
//    }
//}
