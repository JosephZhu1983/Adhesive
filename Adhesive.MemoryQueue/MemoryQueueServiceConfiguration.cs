
using System;
using System.Collections.Generic;
using Adhesive.Common;
using Adhesive.Mongodb;

namespace Adhesive.MemoryQueue
{
    public class MemoryQueueServiceConfiguration
    {
        /// <summary>
        /// 内存队列名
        /// </summary>
        [MongodbPresentationItem(DisplayName = "内存队列名")]
        [MongodbPersistenceItem(ColumnName = "MQN")]
        public string MemoryQueueName { get; set; }

        /// <summary>
        /// 消费数据的委托
        /// </summary>
        [MongodbPersistenceItem(IsIgnore = true)]
        public Action<IList<object>> ConsumeAction { get; set; }

        /// <summary>
        /// 队列最大项数
        /// </summary>
        [MongodbPresentationItem(DisplayName = "队列最大项数")]
        [MongodbPersistenceItem(ColumnName = "MI")]
        public int MaxItemCount { get; set; }

        /// <summary>
        /// 消费数据的时间间隔毫秒
        /// </summary>
        [MongodbPresentationItem(DisplayName = "消费数据的时间间隔毫秒")]
        [MongodbPersistenceItem(ColumnName = "CI")]
        public int ConsumeIntervalMilliseconds { get; set; }

        /// <summary>
        /// 遇到错误时消费数据的时间间隔毫秒
        /// </summary>
        [MongodbPresentationItem(DisplayName = "遇到错误时消费数据的时间间隔毫秒")]
        [MongodbPersistenceItem(ColumnName = "CIE")]
        public int ConsumeIntervalWhenErrorMilliseconds { get; set; }

        /// <summary>
        /// 达到最大项数后的策略
        /// </summary>
        [MongodbPresentationItem(DisplayName = "达到最大项数后的策略")]
        [MongodbPersistenceItem(ColumnName = "RMI")]
        public MemoryQueueServiceReachMaxItemCountAction ReachMaxItemCountAction { get; set; }

        /// <summary>
        /// 消费数据时不足批次数的策略
        /// </summary>
        [MongodbPresentationItem(DisplayName = "消费数据时不足批次数的策略")]
        [MongodbPersistenceItem(ColumnName = "NRB")]
        public MemoryQueueServiceNotReachBatchCountConsumeAction NotReachBatchCountConsumeAction { get; set; }

        /// <summary>
        /// 消费数据遇到错误的策略
        /// </summary>
        [MongodbPresentationItem(DisplayName = "消费数据遇到错误的策略")]
        [MongodbPersistenceItem(ColumnName = "CRA")]
        public MemoryQueueServiceConsumeErrorAction ConsumeErrorAction { get; set; }

        private int consumeThreadCount;
        /// <summary>
        /// 消费的线程总数
        /// </summary>
        [MongodbPresentationItem(DisplayName = "消费的线程总数")]
        [MongodbPersistenceItem(ColumnName = "TC")]
        public int ConsumeThreadCount
        {
            get
            {
                return consumeThreadCount;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Invalid argument!", "ConsumeThreadCount");
                consumeThreadCount = value;
            }
        }

        private int consumeItemCountInOneBatch;
        /// <summary>
        /// 消费数据的批量项数
        /// </summary>
        [MongodbPresentationItem(DisplayName = "消费数据的批量项数")]
        [MongodbPersistenceItem(ColumnName = "B")]
        public int ConsumeItemCountInOneBatch
        {
            get
            {
                return consumeItemCountInOneBatch;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Invalid argument!", "ConsumeItemCountInOneBatch");
                consumeItemCountInOneBatch = value;
            }
        }

        public MemoryQueueServiceConfiguration(string queueName, Action<IList<object>> consumeAction)
        {
            MemoryQueueName = queueName;
            ConsumeAction = consumeAction;
            MaxItemCount = 10000;
            ReachMaxItemCountAction = MemoryQueueServiceReachMaxItemCountAction.AbandonOldItems
                .Add(MemoryQueueServiceReachMaxItemCountAction.LogExceptionEveryOneSecond)
                .Add(MemoryQueueServiceReachMaxItemCountAction.ChangeConsumeErrorActionToAbandonAndLogException)
                .Add(MemoryQueueServiceReachMaxItemCountAction.DecreaseConsumeIntervalOnce)
                .Add(MemoryQueueServiceReachMaxItemCountAction.DecreaseConsumeIntervalWhenErrorOnce);
            ConsumeErrorAction = MemoryQueueServiceConsumeErrorAction.AbandonAndLogException;
            ConsumeThreadCount = 1;
            ConsumeIntervalMilliseconds = 10;
            ConsumeIntervalWhenErrorMilliseconds = 1000;
            ConsumeItemCountInOneBatch = 10;
            NotReachBatchCountConsumeAction = MemoryQueueServiceNotReachBatchCountConsumeAction.ConsumeAllItems;
        }
    }
}
