
using System;
using System.Collections.Generic;
using System.Threading;
using Adhesive.Common;

namespace Adhesive.MemoryQueue.Imp
{
    public class MemoryQueueService : AbstractService, IMemoryQueueService
    {
        internal static readonly string ModuleName = "内存队列服务模块";

        private Queue<object> memoryQueue = new Queue<object>();
        private List<Thread> consumeThreads = new List<Thread>();
        private MemoryQueueServiceConfiguration configuration;
        private Dictionary<int, int> errorItemRetryItem = new Dictionary<int, int>();
        private string lastConsumeErrorMessage = string.Empty;
        private DateTime lastConsumeErrorOccurTime = DateTime.MinValue;
        private DateTime lastReachMaxItemCountOccurTime = DateTime.MinValue;
        private DateTime lastReachMaxItemCountExceptionTime = DateTime.MinValue;
        private long totalConsumeItemCount = 0;
        private long totalConsumeErrorItemCount = 0;

        public MemoryQueueServiceState GetState()
        {
            var state = new MemoryQueueServiceState
            {
                MemoryQueueName = configuration.MemoryQueueName,
                TotalConsumeErrorItemCount = totalConsumeErrorItemCount,
                TotalConsumeItemCount = totalConsumeItemCount,
                Configuration = configuration,
                CurrentErrorRetryItemCount = errorItemRetryItem.Count,
                CurrentItemCount = memoryQueue.Count,
                LastConsumeErrorMessage = lastConsumeErrorMessage,
                LastConsumeErrorOccurTime = lastConsumeErrorOccurTime,
                LastReachMaxItemCountOccurTime = lastReachMaxItemCountOccurTime
            };
            return state;
        }

        public void Init(MemoryQueueServiceConfiguration configuration)
        {
            this.configuration = configuration;
            InternalInit();
            LocalLoggingService.Debug("AdhesiveFramework.MemoryQueueService 成功初始化内存队列 '{0}'!", configuration.MemoryQueueName);
        }

        public void Enqueue<T>(T item)
        {
            try
            {
                if (AdhesiveFramework.Status != AdhesiveFrameworkStatus.Started) return;

                lock (memoryQueue)
                {
                    if (memoryQueue.Count < configuration.MaxItemCount)
                    {
                        memoryQueue.Enqueue(item);
                        return;
                    }

                    if (configuration.ReachMaxItemCountAction.Has(MemoryQueueServiceReachMaxItemCountAction.LogExceptionEveryOneSecond))
                    {
                        if (lastReachMaxItemCountExceptionTime.AddSeconds(1) < DateTime.Now)
                        {
                            LocalLoggingService.Error(ModuleName, "Adhesive.MemoryQueue", configuration.MemoryQueueName,
                                string.Format("内存队列服务 '{0}' 达到最大项限制!", configuration.MemoryQueueName));
                            lastReachMaxItemCountExceptionTime = DateTime.Now;
                        }
                    }

                    if (configuration.ReachMaxItemCountAction.Has(MemoryQueueServiceReachMaxItemCountAction.AbandonOldItems))
                    {
                        while (memoryQueue.Count >= configuration.MaxItemCount)
                            memoryQueue.Dequeue();
                        memoryQueue.Enqueue(item);
                    }

                    if (configuration.ReachMaxItemCountAction.Has(MemoryQueueServiceReachMaxItemCountAction.ChangeConsumeErrorActionToAbandonAndLogException))
                    {
                        configuration.ConsumeErrorAction = MemoryQueueServiceConsumeErrorAction.AbandonAndLogException;
                        configuration.ReachMaxItemCountAction = configuration.ReachMaxItemCountAction.Remove(MemoryQueueServiceReachMaxItemCountAction.ChangeConsumeErrorActionToAbandonAndLogException);
                        LocalLoggingService.Warning("{0} {1} {2} {3}",ModuleName, "Adhesive.MemoryQueue", configuration.MemoryQueueName,
                            string.Format("内存队列服务 '{0}' 调节出错策略!", configuration.MemoryQueueName));
                    }

                    if (configuration.ReachMaxItemCountAction.Has(MemoryQueueServiceReachMaxItemCountAction.DecreaseConsumeIntervalOnce))
                    {
                        if (configuration.ConsumeIntervalMilliseconds > 2)
                            configuration.ConsumeIntervalMilliseconds = configuration.ConsumeIntervalMilliseconds / 2;
                        configuration.ReachMaxItemCountAction = configuration.ReachMaxItemCountAction.Remove(MemoryQueueServiceReachMaxItemCountAction.DecreaseConsumeIntervalOnce);
                        LocalLoggingService.Warning("{0} {1} {2} {3}", ModuleName, "Adhesive.MemoryQueue", configuration.MemoryQueueName,
                            string.Format("内存队列服务 '{0}' 调节消费时间间隔!", configuration.MemoryQueueName));
                    }

                    if (configuration.ReachMaxItemCountAction.Has(MemoryQueueServiceReachMaxItemCountAction.DecreaseConsumeIntervalWhenErrorOnce))
                    {
                        if (configuration.ConsumeIntervalWhenErrorMilliseconds > 2)
                            configuration.ConsumeIntervalWhenErrorMilliseconds = configuration.ConsumeIntervalWhenErrorMilliseconds / 2;
                        configuration.ReachMaxItemCountAction = configuration.ReachMaxItemCountAction.Remove(MemoryQueueServiceReachMaxItemCountAction.DecreaseConsumeIntervalWhenErrorOnce);
                        LocalLoggingService.Warning("{0} {1} {2} {3}", ModuleName, "Adhesive.MemoryQueue", configuration.MemoryQueueName,
                            string.Format("内存队列服务 '{0}' 调节出错消费时间间隔!"));
                    }

                    if (configuration.ReachMaxItemCountAction.Has(MemoryQueueServiceReachMaxItemCountAction.DoubleMaxItemCountOnce))
                    {
                        if (configuration.MaxItemCount < int.MaxValue / 2)
                            configuration.MaxItemCount *= 2;
                        configuration.ReachMaxItemCountAction = configuration.ReachMaxItemCountAction.Remove(MemoryQueueServiceReachMaxItemCountAction.DoubleMaxItemCountOnce);
                        LocalLoggingService.Warning("{0} {1} {2} {3}", ModuleName, "Adhesive.MemoryQueue", configuration.MemoryQueueName,
                            string.Format("内存队列服务 '{0}' 调节最大项!", configuration.MemoryQueueName));
                    }
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error(ex.ToString());
            }
        }

        public void EnqueueBatch<T>(IList<T> item)
        {
            item.Each(Enqueue);
        }

        private void InternalInit()
        {
            lock (memoryQueue)
            {
                if (consumeThreads.Count == 0)
                {
                    for (int i = 0; i < configuration.ConsumeThreadCount; i++)
                    {
                        var thread = new Thread(Consume)
                        {
                            Name = string.Format("{0}_{1}_{2}", "Adhesive.MemoryQueue", configuration.MemoryQueueName, i),
                            IsBackground = true,
                        };
                        thread.Start();
                        consumeThreads.Add(thread);
                    }

                }
            }
        }

        private void Consume()
        {
            while (Enabled)
            {
                int sleepTime = configuration.ConsumeIntervalMilliseconds;
                List<object> items = new List<object>();

                try
                {
                    lock (memoryQueue)
                    {
                        var count = memoryQueue.Count;
                        if (count >= configuration.ConsumeItemCountInOneBatch)
                        {
                            for (int i = 0; i < configuration.ConsumeItemCountInOneBatch; i++)
                            {
                                items.Add(memoryQueue.Dequeue());
                            }
                        }
                        else if (count > 0 && configuration.NotReachBatchCountConsumeAction == MemoryQueueServiceNotReachBatchCountConsumeAction.ConsumeAllItems)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                items.Add(memoryQueue.Dequeue());
                            }
                        }
                    }

                    if (items.Count > 0)
                    {
                        try
                        {
                            configuration.ConsumeAction(items);
                            Interlocked.Add(ref totalConsumeItemCount, items.Count);
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Add(ref totalConsumeErrorItemCount, items.Count);

                            lastConsumeErrorMessage = ex.ToString();
                            lastConsumeErrorOccurTime = DateTime.Now;

                            sleepTime = configuration.ConsumeIntervalWhenErrorMilliseconds;

                            if (configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.AbandonAndLogException ||
                                configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.EnqueueForeverAndLogException ||
                                configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.EnqueueTwiceAndLogException)
                            {
                                LocalLoggingService.Error("{0} {1} {2} {3}", ModuleName, "Adhesive.MemoryQueue", configuration.MemoryQueueName,
                                    string.Format("内存队列服务 '{0}' 消费出错!", configuration.MemoryQueueName),ex.Message);
                            }

                            if (configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.Abandon || configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.AbandonAndLogException)
                            {

                            }

                            if (configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.EnqueueForever || configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.EnqueueForeverAndLogException)
                            {
                                items.ForEach(memoryQueue.Enqueue);
                            }

                            if (configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.EnqueueTwiceAndLogException || configuration.ConsumeErrorAction == MemoryQueueServiceConsumeErrorAction.EnqueueTwiceAndLogException)
                            {
                                items.ForEach(item =>
                                {
                                    var itemId = item.GetHashCode();
                                    lock (errorItemRetryItem)
                                    {
                                        if (!errorItemRetryItem.ContainsKey(itemId))
                                        {
                                            Enqueue(item);
                                            errorItemRetryItem[itemId] = 1;
                                        }
                                        else
                                        {
                                            var retryCount = errorItemRetryItem[itemId];
                                            if (retryCount < 2)
                                            {
                                                Enqueue(item);
                                                errorItemRetryItem[itemId]++;
                                            }
                                            else
                                            {
                                                errorItemRetryItem.Remove(itemId);
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error(ex.ToString());
                }
                finally
                {
                    Thread.Sleep(sleepTime);
                }
            }
        }
    }
}
