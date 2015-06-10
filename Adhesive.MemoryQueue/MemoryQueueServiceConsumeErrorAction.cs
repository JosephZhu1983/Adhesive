
namespace Adhesive.MemoryQueue
{
    public enum MemoryQueueServiceConsumeErrorAction
    {
        /// <summary>
        /// 抛弃数据
        /// </summary>
        Abandon = 1,
        /// <summary>
        /// 抛弃数据并且记录异常
        /// </summary>
        AbandonAndLogException = 2,
        /// <summary>
        /// 永远重新入列
        /// </summary>
        EnqueueForever = 3,
        /// <summary>
        /// 永远重新入列并且记录异常
        /// </summary>
        EnqueueForeverAndLogException = 4,
        /// <summary>
        /// 重新入列两次
        /// </summary>
        EnqueueTwice = 5,
        /// <summary>
        /// 重新入列两次并且记录异常
        /// </summary>
        EnqueueTwiceAndLogException = 6,
    }
}
