

using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;
namespace Adhesive.DistributedService.Imp
{
    public abstract class WcfMessageInfo : AbstractInfo
    {
        [MongodbPersistenceItem(MongodbIndexOption = MongodbIndexOption.Ascending, ColumnName = "MD")]
        [MongodbPresentationItem(MongodbFilterOption = MongodbFilterOption.DropDownListFilter, DisplayName = "消息方向", ShowInTableView = true)]
        public MessageDirection MessageDirection { get; set; }

        [MongodbPersistenceItem(ColumnName = "M")]
        [MongodbPresentationItem(DisplayName = "完整的消息")]
        public string Message { get; set; }
    }
}
