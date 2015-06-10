using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Adhesive.Common;
using Adhesive.DistributedComponentClient.Memcached;

namespace Adhesive.DistributedComponentClient.Test
{
    class fuck
    {
        public int times { get; set; }

        public string name { get; set; }

        public override string ToString()
        {
            return string.Format("fuck {0} {1} 次", name, times);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Adhesive.Common.AdhesiveFramework.Start();

            var client = MemcachedClient.GetClient("TestMemcachedCluster");

            //client.Flush();

            //var stat = client.Stat();


            var lockerkey = Guid.NewGuid().ToString();
            client.AcquireLock(lockerkey, TimeSpan.FromSeconds(1));
            Parallel.For(0, 10, i =>
            {
                try
                {
                    using (var clocker = client.AcquireLock(lockerkey, TimeSpan.FromSeconds(1)))
                    {
                        Console.WriteLine(i + "\t" + DateTime.Now.ToString("HH:mm:ss fff") + " 获得锁");
                        Thread.Sleep(1000);
                    }
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine(i + "\t" + DateTime.Now.ToString("HH:mm:ss fff") + " " + ex.Message);
                }
            });


            Console.ReadLine();
            //故障测试

            //Console.ReadLine();
            while (true)
            {
                Console.Clear();
                Parallel.For(0, 10, i =>
                {
                    LocalLoggingService.Debug("Get Key:" + "failtest" + i);
                    var v = client.Get("failtest" + i);
                    if (v == null)
                    {
                        LocalLoggingService.Debug("没有取到 Key:" + "failtest" + i);
                        bool b = client.Set("failtest" + i, "阿斯达撒大的" + i);
                        LocalLoggingService.Debug("Set Key:" + "failtest" + i + ":" + b.ToString());
                    }
                    else
                    {
                        LocalLoggingService.Debug("取到 Key:" + "failtest" + i + "  Value:" + v);
                    }
                });
                Thread.Sleep(1000);
            }

            //性能测试

            while (true)
            {
                var pcount = 100000;
                Stopwatch sw = Stopwatch.StartNew();
                var p = Guid.NewGuid().ToString().Substring(0, 10);
                Parallel.For(0, pcount, i =>
                {
                    if (!client.Set(p + i, new fuck
                    {
                        name = "你爷爷",
                        times = i,
                    }, TimeSpan.FromMinutes(5)))
                        LocalLoggingService.Warning("Set错误" + i);
                });
                LocalLoggingService.Info("写测试 次数：{0} 时间：{1} 每秒：{2}", pcount, sw.ElapsedMilliseconds, pcount * 1000 / sw.ElapsedMilliseconds);
                sw.Restart();
                Parallel.For(0, pcount, i =>
                {
                    client.FastSet(p + i, new fuck
                    {
                        name = "你爷爷",
                        times = i,
                    }, TimeSpan.FromMinutes(5));
                });
                LocalLoggingService.Info("快速写测试 次数：{0} 时间：{1} 每秒：{2}", pcount, sw.ElapsedMilliseconds, pcount * 1000 / sw.ElapsedMilliseconds);
                sw.Restart();
                Parallel.For(0, pcount, i =>
               {
                   if (client.Get<fuck>(p + i) == null)
                       LocalLoggingService.Warning("Get错误" + i);
               });
                LocalLoggingService.Info("读测试 次数：{0} 时间：{1} 每秒：{2}", pcount, sw.ElapsedMilliseconds, pcount * 1000 / sw.ElapsedMilliseconds);


            }
            Console.ReadKey();


            //测试List

            var listkey = Guid.NewGuid().ToString();
            int failed = 0;
            Parallel.For(0, 1000, i =>
            {
                var itemkey = Guid.NewGuid().ToString();
                if (!client.SetListItem(listkey, itemkey, new fuck
                {
                    name = "你爷爷",
                    times = i,
                }, TimeSpan.FromHours(1)))
                    Interlocked.Increment(ref failed);
            });

            //client.DeleteListItem("fucklist", "fuck5");

            List<fuck> listData = client.GetList<fuck>(listkey);
            Console.WriteLine(listData.Count);
            Console.WriteLine(failed);

            Console.ReadKey();

            // 为测试批量获取做准备
            var keys = new System.Collections.Concurrent.ConcurrentBag<string>();

            Parallel.For(0, 20, i =>
            {
                var t = i;
                var key = Guid.NewGuid().ToString();
                keys.Add(key);
                var fuck = new fuck
                {
                    name = "你妈",
                    times = t,
                };

                if (!client.Set(key, fuck, TimeSpan.FromSeconds(t + 1)))
                    throw new Exception("错误");
            });

            //为测试append做准备
            Console.WriteLine("append " + client.Set("append", "1"));
            Console.WriteLine("prepend " + client.Set("prepend", "1"));
            Console.WriteLine("replace " + client.Replace("replace", "test"));

            Console.WriteLine(client.Add("add", "test"));
            Console.WriteLine("add again " + client.Add("add", "test"));

            //测试cas
            var caskey = "castest";
            ulong version = 0;
            client.Set(caskey, caskey, version);
            client.Get(caskey, out version);
            Console.WriteLine(client.Set(caskey, caskey, version));
            Console.WriteLine(client.Set(caskey, caskey, version));

            Console.WriteLine("press key");
            Console.ReadKey();

            while (true)
            {
                Thread.Sleep(10);
                Console.Clear();
                var data = client.GetMultiple<fuck>(keys.ToList());
                foreach (var item in data)
                {
                    Console.WriteLine(item.Key + " " + item.Value);
                }

                Console.WriteLine("incr:" + client.IncrementWithInit("num1", 10, TimeSpan.FromSeconds(5), 5));
                Console.WriteLine("decr:" + client.DecrementWithInit("num2", 20, TimeSpan.FromSeconds(5), 5));

                client.Append("append", "2");
                Console.WriteLine("append:" + client.Get("append"));

                client.Prepend("prepend", "2");
                Console.WriteLine("prepend:" + client.Get("prepend"));

                var total = 1000;
                Parallel.For(0, total, new ParallelOptions { MaxDegreeOfParallelism = 4, }, i =>
                {
                    int t = i;
                    var key = "yzhu" + t;
                    var fuck = new fuck
                    {
                        name = new string('a', 1024),
                        times = t,
                    };

                    if (!client.Set(key, fuck))
                        throw new Exception("Set出错！" + key);
                });

                int count = 0;
                Parallel.For(0, total, new ParallelOptions { MaxDegreeOfParallelism = 4, }, i =>
                {
                    int t = i;
                    var key = "yzhu" + t;
                    if (client.Get<fuck>(key) != null)
                        Interlocked.Increment(ref count);
                    else
                        throw new Exception("Get出错！" + key);
                });

                if (count != total)
                    throw new Exception("没有取到所有数据" + count);
            }
        }
    }
}
