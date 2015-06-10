
using System;
using System.Threading;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient.Memcached
{
    public class MemcachedLocker : IDisposable
    {
        private readonly string key;
        private readonly TimeSpan timeOut;
        private readonly MemcachedClient client;

        public MemcachedLocker(MemcachedClient client, string key, TimeSpan timeOut)
        {
            this.client = client;
            this.key = key;
            this.timeOut = timeOut;

            int sleep = 10;
            DateTime now = DateTime.Now;
            while (DateTime.Now - now < timeOut)
            {
                if (client.Add<DateTime?>(key, DateTime.Now.Add(timeOut)))
                    return;

                //需要排除锁未释放的可能，如果检测到超过超时时间2倍的话，尝试获得锁
                ulong version;
                var time = client.Get<DateTime?>(key, out version);
                if (time == null || (time.HasValue && time.Value.ToLocalTime().Add(timeOut + timeOut) < DateTime.Now))
                {
                    LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedLocker", "MemcachedLocker",
                                        string.Format("发现一个超时的分布式锁，超时时间：{0} Key : {1}",
                                        time, key));
                    if (client.Add<DateTime?>(key, DateTime.Now.Add(timeOut), version))
                        return;
                }

                if (sleep < 1000)
                    sleep = sleep * 110 / 100;
                else
                    sleep = 1000;

                Thread.Sleep(sleep);
            }

            throw new TimeoutException(string.Format("获得锁的时间超过了设定的最大时间 {0}", timeOut.ToString()));
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.Delete(key);
            }
        }
    }
}
