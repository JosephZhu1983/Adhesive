
namespace Adhesive.MemoryQueue
{
    public enum MemoryQueueServiceNotReachBatchCountConsumeAction
    {
        WaitForMoreItem = 1, //等待更多数据
        ConsumeAllItems = 2, //直接消费当前所有数据
    }
}
