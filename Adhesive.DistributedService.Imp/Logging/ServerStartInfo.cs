

using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "服务端启动日志", Name = "ServerStart")]
    public class ServerStartInfo : StartInfo, IWcfServerInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "SN")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "服务名", ShowInTableView = true)]
        public string ServiceName { get; set; }

        [MongodbPersistenceItem(ColumnName = "WSC")]
        [MongodbPresentationItem(DisplayName = "Wcf服务端端点信息")]
        public WcfServiceConfig WcfService { get; set; }
    }
}
