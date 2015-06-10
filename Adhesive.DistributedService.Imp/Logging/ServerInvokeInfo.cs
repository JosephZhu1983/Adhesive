

using System.Collections.Generic;
using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "服务端调用日志", Name = "ServerInvoke")]
    public class ServerInvokeInfo : InvokeInfo, IWcfServerInfo
    {
        [MongodbPersistenceItem(ColumnName = "SN")]
        [MongodbPresentationItem(DisplayName = "服务名")]
        public string ServiceName { get; set; }

        [MongodbPersistenceItem(ColumnName = "P")]
        [MongodbPresentationItem(DisplayName = "参数")]
        public List<string> Parameters { get; set; }

        [MongodbPersistenceItem(ColumnName = "R")]
        [MongodbPresentationItem(DisplayName = "返回值")]
        public List<string> Results { get; set; }

        [MongodbPersistenceItem(ColumnName = "ISI")]
        [MongodbPresentationItem(DisplayName = "是否是同步调用")]
        public bool IsSyncInvoke { get; set; }
    }
}
