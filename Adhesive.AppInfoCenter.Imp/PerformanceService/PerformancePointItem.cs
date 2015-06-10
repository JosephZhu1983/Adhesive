
using System;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class PerformancePointItem
    {
        [MongodbPersistenceItem(ColumnName = "T")]
        [MongodbPresentationItem(DisplayName = "时间")]
        public DateTime Time { get; set; }

        [MongodbPersistenceItem(ColumnName = "C")]
        [MongodbPresentationItem(DisplayName = "一次迭代消耗CPU时间")]
        public long CPUTime { get; set; }

        [MongodbPersistenceItem(ColumnName = "TE")]
        [MongodbPresentationItem(DisplayName = "一次迭代消耗时间")]
        public long TimeElapsed { get; set; }
    }
}
