

using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "客户端启动日志", Name = "ClientStart")]
    public class ClientStartInfo : StartInfo, IWcfClientInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "CN")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "契约名", ShowInTableView = true)]
        public string ContractName { get; set; }

        [MongodbPersistenceItem(ColumnName = "WCE")]
        [MongodbPresentationItem(DisplayName = "Wcf客户端端点信息")]
        public WcfClientEndpointConfig WcfEndpoint { get; set; }
    }
}
