
using Adhesive.AppInfoCenter.Imp;
using Adhesive.Mongodb;

namespace Adhesive.DistributedService.Imp
{
    public abstract class InvokeInfo : AbstractInfo
    {
        [MongodbPersistenceItem(ColumnName = "ET")]
        [MongodbPresentationItem(DisplayName = "执行时间", ShowInTableView = true)]
        public long ExecutionTime { get; set; }

        [MongodbPersistenceItem(ColumnName = "IS")]
        [MongodbPresentationItem(DisplayName = "是否成功", ShowInTableView = true)]
        public bool IsSuccessuful { get; set; }

        [MongodbPersistenceItem(ColumnName = "MN")]
        [MongodbPresentationItem(DisplayName = "调用方法名", ShowInTableView = true)]
        public string MethodName { get; set; }

        [MongodbPersistenceItem(ColumnName = "AC")]
        [MongodbPresentationItem(DisplayName = "上下文信息")]
        public ApplicationContext ApplicationContext { get; set; }
    }
}
