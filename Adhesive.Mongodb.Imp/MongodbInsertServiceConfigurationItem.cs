
using Adhesive.Common;
using Adhesive.Config;
using Adhesive.MemoryQueue;

namespace Adhesive.Mongodb.Imp
{
    [ConfigEntity(FriendlyName = "Mongodb客户端针对每个数据类型的配置")]
    public class MongodbInsertServiceConfigurationItem
    {
        [ConfigItem(FriendlyName = "类型完整名")]
        public string TypeFullName { get; set; }

        [ConfigItem(FriendlyName = "是否提交到服务端")]
        public bool SubmitToServer { get; set; }

        [ConfigItem(FriendlyName = "队列最大项数")]
        public int MaxItemCount { get; set; }

        [ConfigItem(FriendlyName = "消费的线程总数")]
        public int ConsumeThreadCount { get; set; }

        [ConfigItem(FriendlyName = "消费数据的时间间隔毫秒")]
        public int ConsumeIntervalMilliseconds { get; set; }

        [ConfigItem(FriendlyName = "遇到错误时消费数据的时间间隔毫秒")]
        public int ConsumeIntervalWhenErrorMilliseconds { get; set; }

        [ConfigItem(FriendlyName = "消费数据的批量项数")]
        public int ConsumeItemCountInOneBatch { get; set; }

        [ConfigItem(FriendlyName = "达到最大项数后的策略")]
        public MemoryQueueServiceReachMaxItemCountAction ReachMaxItemCountAction { get; set; }

        [ConfigItem(FriendlyName = "消费数据时不足批次数的策略")]
        public MemoryQueueServiceNotReachBatchCountConsumeAction NotReachBatchCountConsumeAction { get; set; }

        [ConfigItem(FriendlyName = "消费数据遇到错误的策略")]
        public MemoryQueueServiceConsumeErrorAction ConsumeErrorAction { get; set; }

        public MongodbInsertServiceConfigurationItem()
        {
            TypeFullName = "";
            SubmitToServer = true;
            ReachMaxItemCountAction = MemoryQueueServiceReachMaxItemCountAction.AbandonOldItems
                .Add(MemoryQueueServiceReachMaxItemCountAction.LogExceptionEveryOneSecond);
            ConsumeErrorAction = MemoryQueueServiceConsumeErrorAction.AbandonAndLogException;
            ConsumeThreadCount = 2;
            ConsumeIntervalMilliseconds = 10;
            ConsumeIntervalWhenErrorMilliseconds = 1000;
            ConsumeItemCountInOneBatch = 20;
            NotReachBatchCountConsumeAction = MemoryQueueServiceNotReachBatchCountConsumeAction.ConsumeAllItems;
            MaxItemCount = 10000;
        }
    }
}
