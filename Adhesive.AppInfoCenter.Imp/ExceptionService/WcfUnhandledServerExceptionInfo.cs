

using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    [MongodbPersistenceEntity("Wcf", DisplayName = "服务端异常日志", Name = "ServerException")]
    public class WcfUnhandledServerExceptionInfo : ExceptionInfo, IWcfServerInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName ="SN")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "服务名", ShowInTableView = true)]
        public string ServiceName { get; set; }
    }
}
