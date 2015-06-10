
using System.Collections.Generic;
using System.Diagnostics;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Aic", DisplayName = "代码性能测量", Name = "Performance")]
    public class PerformanceInfo : AbstractInfo
    {
        [MongodbPersistenceItem(IsIgnore = true)]
        public Stopwatch sw { get; set; }

        [MongodbPersistenceItem(IsIgnore = true)]
        public long threadTime { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName ="N")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "性能测试名", ShowInTableView = true)]
        public string Name { get; set; }

        [MongodbPersistenceItem(ColumnName = "TCT")]
        [MongodbPresentationItem(DisplayName = "总CPU时间", ShowInTableView = true)]
        public long TotalCPUTime { get; set; }

        [MongodbPersistenceItem(ColumnName = "TTE")]
        [MongodbPresentationItem(DisplayName = "总消耗时间", ShowInTableView = true)]
        public long TotalTimeElapsed { get; set; }

        [MongodbPersistenceItem(ColumnName = "TPP")]
        [MongodbPresentationItem(DisplayName = "总测量点数", ShowInTableView = true)]
        public int TotalPerformancePointCount { get; set; }

        [MongodbPersistenceItem(ColumnName = "PP")]
        [MongodbPresentationItem(DisplayName = "所有测量点")]
        public Dictionary<string, PerformancePoint> PerformancePoints { get; set; }
    }
}
