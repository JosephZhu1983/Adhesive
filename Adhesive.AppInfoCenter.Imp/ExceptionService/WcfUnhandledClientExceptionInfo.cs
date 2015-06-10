

using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "客户端异常日志", Name = "ClientException")]
    public class WcfUnhandledClientExceptionInfo : ExceptionInfo, IWcfClientInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName ="CN")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "契约名", ShowInTableView = true)]
        public string ContractName { get; set; }

        [MongodbPersistenceItem(ColumnName = "SE")]
        [MongodbPresentationItem(DisplayName = "服务端异常Id")]
        public string ServerExceptionID { get; set; }
    }
}
