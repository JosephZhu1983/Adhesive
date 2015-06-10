
using System.Collections.Generic;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class PerformancePoint
    {
        [MongodbPersistenceItem(ColumnName = "ACT")]
        [MongodbPresentationItem(DisplayName = "平均CPU时间")]
        public int AverageCPUTime { get; set; }

        [MongodbPersistenceItem(ColumnName = "ATE")]
        [MongodbPresentationItem(DisplayName = "平均消耗时间")]
        public int AverageTimeElapsed { get; set; }

        [MongodbPersistenceItem(ColumnName = "TPP")]
        [MongodbPresentationItem(DisplayName = "测量点迭代总数")]
        public int TotalPerformancePointItemCount { get; set; }

        [MongodbPersistenceItem(ColumnName = "MPP")]
        [MongodbPresentationItem(DisplayName = "测量点的每一次迭代")]
        public List<PerformancePointItem> PerformancePointItems { get; set; }
    }
}
