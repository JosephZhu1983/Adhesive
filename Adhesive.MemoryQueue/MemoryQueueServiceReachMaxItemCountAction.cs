
using System;

namespace Adhesive.MemoryQueue
{
    [Flags]
    public enum MemoryQueueServiceReachMaxItemCountAction
    {
        AbandonOldItems = 0x1, //抛弃老数据
        DoubleMaxItemCountOnce = 0x2, //扩大一次最大队列项数
        ChangeConsumeErrorActionToAbandonAndLogException = 0x4, //把遇到错误的处理策略修改为抛弃数据并且记录异常
        DecreaseConsumeIntervalOnce = 0x8, //减少一次提交数据的间隔时间
        DecreaseConsumeIntervalWhenErrorOnce = 0x10, //减少一次遇到错误时提交数据的间隔时间
        LogExceptionEveryOneSecond = 0x20, //每秒记录一次异常
    }
}
