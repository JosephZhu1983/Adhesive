

using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "客户端调用日志", Name = "ClientInvoke")]
    public class ClientInvokeInfo : InvokeInfo, IWcfClientInfo
    {
        [MongodbPersistenceItem(ColumnName = "CN")]
        [MongodbPresentationItem(DisplayName = "契约名")]
        public string ContractName { get; set; }
    }
}
