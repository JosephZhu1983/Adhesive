
using System;
using System.Collections.Generic;

namespace Adhesive.MemoryQueue
{
    public interface IMemoryQueueService : IDisposable
    {
        /// <summary>
        /// 初始化队列服务
        /// </summary>
        /// <param name="configuration"></param>
        void Init(MemoryQueueServiceConfiguration configuration);

        /// <summary>
        /// 入列一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Enqueue<T>(T item);

        /// <summary>
        /// 入列多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void EnqueueBatch<T>(IList<T> item);

        /// <summary>
        /// 获取队列状态
        /// </summary>
        /// <returns></returns>
        MemoryQueueServiceState GetState();
    }
}
