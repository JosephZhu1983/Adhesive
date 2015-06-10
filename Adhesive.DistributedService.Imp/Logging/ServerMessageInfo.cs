

using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "服务端消息日志", Name = "ServerMsg")]
    public class ServerMessageInfo : WcfMessageInfo, IWcfServerInfo
    {
        [MongodbPersistenceItem(ColumnName = "SN")]
        [MongodbPresentationItem(DisplayName = "服务名")]
        public string ServiceName { get; set; }
    }
}
