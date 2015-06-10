
using System;
using Adhesive.Mongodb;

namespace Adhesive.MemoryQueue
{
    [MongodbPersistenceEntity("State", DisplayName = "内存队列服务状态", Name = "MemQueue")]
    public class MemoryQueueServiceState
    {
        /// <summary>
        /// 内存队列名
        /// </summary>
        [MongodbPresentationItem(DisplayName = "内存队列名")]
        [MongodbPersistenceItem(ColumnName = "MQN")]
        public string MemoryQueueName { get; set; }

        /// <summary>
        /// 内存队列的配置
        /// </summary>
        [MongodbPresentationItem(DisplayName = "内存队列的配置")]
        [MongodbPersistenceItem(ColumnName = "C")]
        public MemoryQueueServiceConfiguration Configuration { get; set; }

        /// <summary>
        /// 总消费的项数量
        /// </summary>
        [MongodbPresentationItem(DisplayName = "总消费的项数量")]
        [MongodbPersistenceItem(ColumnName = "TCI")]
        public long TotalConsumeItemCount { get; set; }

        /// <summary>
        /// 总消费出错的项数量
        /// </summary>
        [MongodbPresentationItem(DisplayName = "总消费出错的项数量")]
        [MongodbPersistenceItem(ColumnName = "TCE")]
        public long TotalConsumeErrorItemCount { get; set; }

        /// <summary>
        /// 当前队列剩余项数
        /// </summary>
        [MongodbPresentationItem(DisplayName = "当前队列剩余项数")]
        [MongodbPersistenceItem(ColumnName = "CIC")]
        public int CurrentItemCount { get; set; }

        /// <summary>
        /// 当前错误重试的项数
        /// </summary>
        [MongodbPresentationItem(DisplayName = "当前错误重试的项数")]
        [MongodbPersistenceItem(ColumnName = "CER")]
        public int CurrentErrorRetryItemCount { get; set; }

        /// <summary>
        /// 上次消费出错的时间
        /// </summary>
        [MongodbPresentationItem(DisplayName = "上次消费出错的时间")]
        [MongodbPersistenceItem(ColumnName = "LCEO")]
        public DateTime LastConsumeErrorOccurTime { get; set; }

        /// <summary>
        /// 上次达到最大项数的时间
        /// </summary>
        [MongodbPresentationItem(DisplayName = "上次达到最大项数的时间")]
        [MongodbPersistenceItem(ColumnName = "LRMI")]
        public DateTime LastReachMaxItemCountOccurTime { get; set; }

        /// <summary>
        /// 上次消费出错的异常信息
        /// </summary>
        [MongodbPresentationItem(DisplayName = "上次消费出错的异常信息")]
        [MongodbPersistenceItem(ColumnName = "LCEM")]
        public string LastConsumeErrorMessage { get; set; }
    }
}
