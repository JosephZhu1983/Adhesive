

using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "客户端消息日志", Name = "ClientMsg")]
    public class ClientMessageInfo : WcfMessageInfo, IWcfClientInfo
    {
        [MongodbPersistenceItem(ColumnName = "CN")]
        [MongodbPresentationItem(DisplayName = "契约名")]
        public string ContractName { get; set; }
    }
}
