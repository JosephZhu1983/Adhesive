
using System;
using Adhesive.Common;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter
{
    public abstract class BaseInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.AscendingAndUnique, IsPrimaryKey = true)]
        [MongodbPresentationItem(ShowInTableView = true, DisplayName = "主键")]
        public string ID { get; set; }

        [MongodbPersistenceItem(IsTableName = true, ColumnName = "AN")]
        [MongodbPresentationItem(DisplayName = "应用程序名", ShowInTableView = true)]
        public string AppName { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Descending, IsTimeColumn = true, ColumnName = "T")]
        [MongodbPresentationItem(MongodbSortOption = MongodbSortOption.Descending, DisplayName = "时间", ShowInTableView = true)]
        public DateTime ServerTime { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "IP")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "服务器IP", ShowInTableView = true)]
        public string MachineIP { get; set; }

        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "DN")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "域名", ShowInTableView = true)]
        public string DomainName { get; set; }

        public BaseInfo()
        {
            ID = Guid.NewGuid().ToString();
            AppName = CommonConfiguration.GetConfig().ApplicationName;
            MachineIP = CommonConfiguration.MachineIP;
            ServerTime = DateTime.Now;
        }
    }
}
