using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Adhesive.DistributedComponentClient.Memcached;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Adhesive.DistributedComponentClient.UnitTest
{


    /// <summary>
    ///This is a test class for MemcachedClientTest and is intended
    ///to contain all MemcachedClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MemcachedClientTest
    {
        private static Random rnd = new Random();
        private TestContext testContextInstance;
        private static MemcachedClient client1 = MemcachedClient.GetClient("TestMemcachedCluster");
        private static List<MemcachedClient> clients = new List<MemcachedClient>();

        private static string stringValue = new string('操', 1000);
        private static readonly int Count = 1000;
        private static TimeSpan expire = TimeSpan.FromMinutes(1);

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Adhesive.Common.AdhesiveFramework.Start();
            clients.Add(client1);
            Parallel.ForEach(clients, client => client.Flush());
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion



        [TestMethod()]
        public void MultiGetTest()
        {
            Parallel.ForEach(clients, client =>
            {
                var keys = new System.Collections.Concurrent.ConcurrentBag<string>();
                Parallel.For(0, 1000, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, stringValue, TimeSpan.FromMinutes(10)));
                    keys.Add(key);
                });

                var value = client.GetMultiple(keys.ToList());
                Assert.AreEqual(value.Count, keys.Count);
                foreach (var item in value)
                {
                    Assert.AreEqual(item.Value, stringValue);
                }
            });
        }

        [TestMethod()]
        public void MultiGetObjectTest()
        {
            Parallel.ForEach(clients, client =>
            {
                var keys = new System.Collections.Concurrent.ConcurrentBag<string>();
                Parallel.For(0, 1000, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, new User(i), TimeSpan.FromMinutes(10)));
                    keys.Add(key);
                });

                var value = client.GetMultiple<User>(keys.ToList());
                Assert.AreEqual(value.Count, keys.Count);
                foreach (var item in value)
                {
                    Assert.AreEqual(item.Value.GetType(), typeof(User));
                }
            });
        }

        [TestMethod()]
        public void ListTest()
        {
            Parallel.ForEach(clients, client =>
            {
                var listKey = Guid.NewGuid().ToString();

                Parallel.For(0, 100, i =>
                {
                    var itemKey = Guid.NewGuid().ToString();
                    client.SetListItem(listKey, itemKey, stringValue, TimeSpan.FromMinutes(10));
                    Thread.Sleep(rnd.Next(2));
                });

                var value = client.GetList(listKey);
                Assert.AreEqual(value.Count, 100);
            });
        }

        [TestMethod()]
        public void ListPagingTest()
        {
            Parallel.ForEach(clients, client =>
            {
                var listKey = Guid.NewGuid().ToString();
                var pageIndex = 2;
                var pageSize = 10;

                Parallel.For(0, 100, i =>
                {
                    var itemKey = Guid.NewGuid().ToString();
                    client.SetListItem(listKey, itemKey, new User(i), TimeSpan.FromMinutes(10));
                    Thread.Sleep(rnd.Next(2));
                });
                var value = client.GetList<User>(listKey, pageSize, pageIndex);
                Assert.AreEqual(value.Count, pageSize);
                Assert.AreEqual(value.First().GetType(), typeof(User));
            });
        }

        [TestMethod()]
        public void BasicGetSetTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, stringValue, expire));
                    Assert.AreEqual(stringValue, client.Get(key));
                });
            });
        }

        [TestMethod()]
        public void CasGetSetTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    ulong version = 0;
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Add(key, stringValue, expire, version));
                    Assert.AreEqual(stringValue, client.Get(key, out version));
                    Assert.IsTrue(client.Replace(key, stringValue, version));
                    Assert.IsFalse(client.Replace(key, stringValue, version));
                });
            });
        }

        [TestMethod()]
        public void ObjectGetSetTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    var value = new User(i);
                    Assert.IsTrue(client.Set<User>(key, value, expire));
                    var value2 = client.Get<User>(key);
                    Assert.AreEqual(value, value2);
                });
            });
        }

        [TestMethod()]
        public void ExpireTest()
        {
            Parallel.ForEach(clients, client =>
            {
                var expireSpan = TimeSpan.FromSeconds(1);
                var keys = new System.Collections.Concurrent.ConcurrentBag<string>();
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, stringValue, expireSpan));
                    keys.Add(key);
                });
                Thread.Sleep(expireSpan);
                Parallel.ForEach(keys, key =>
                {
                    Assert.IsNull(client.Get(key));
                });
            });
        }

        [TestMethod()]
        public void VersionTest()
        {
            Parallel.ForEach(clients, client =>
            {
                var version = client.Version();
                foreach (var item in version)
                {
                    Assert.IsNotNull(item.Value);
                }
            });
        }

        [TestMethod()]
        public void FastSetTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    client.FastSet(key, stringValue, expire);
                    Thread.Sleep(10);
                    Assert.AreEqual(stringValue, client.Get(key));
                });
            });
        }

        [TestMethod()]
        public void GetAndSetTest()
        {
            Func<string> func = () => stringValue;
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.AreEqual(client.GetAndSet(key, func, expire), stringValue);
                    Assert.AreEqual(client.Get(key), stringValue);
                });
            });
        }

        [TestMethod()]
        public void ReplaceTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsFalse(client.Replace(key, stringValue));
                    Assert.IsTrue(client.Set(key, ""));
                    Assert.IsTrue(client.Replace(key, stringValue));
                    Assert.AreEqual(client.Get(key), stringValue);
                });
            });
        }

        [TestMethod()]
        public void AddTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, ""));
                    Assert.IsFalse(client.Add(key, stringValue, expire));
                    Assert.IsTrue(client.Delete(key));
                    Assert.IsTrue(client.Add(key, stringValue, expire));
                    Assert.AreEqual(client.Get(key), stringValue);
                });
            });

        }

        [TestMethod()]
        public void DeleteTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, ""));
                    Assert.AreEqual(client.Get(key), "");
                    Assert.IsTrue(client.Delete(key));
                    Assert.AreEqual(client.Get(key), null);
                });
            });
        }

        [TestMethod()]
        public void FastDeleteTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, ""));
                    Assert.AreEqual(client.Get(key), "");
                    client.FastDelete(key);
                    Thread.Sleep(10);
                    Assert.AreEqual(client.Get(key), null);
                });
            });
        }

        [TestMethod()]
        public void AppendTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, stringValue, expire));
                    Assert.IsTrue(client.Append(key, "你好"));
                    Assert.AreEqual(stringValue + "你好", client.Get(key));
                });
            });
        }

        [TestMethod()]
        public void PrependTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    Assert.IsTrue(client.Set(key, stringValue, expire));
                    Assert.IsTrue(client.Prepend(key, "你好"));
                    Assert.AreEqual("你好" + stringValue, client.Get(key));
                });
            });
        }

        [TestMethod()]
        public void IncrementTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    var result = client.Increment(key, 10);
                    Assert.IsFalse(result.HasValue);
                    Assert.IsTrue(client.Set(key, i));
                    result = client.Increment(key, 10);
                    Assert.IsTrue(result.HasValue);
                    Assert.AreEqual(result.Value, (ulong)i + 10);
                });
            });
        }

        [TestMethod()]
        public void IncrementWithInitTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    var result = client.IncrementWithInit(key, (ulong)i, expire, 10);
                    Assert.IsTrue(result.HasValue);
                    Assert.AreEqual(result.Value, (ulong)i);
                    result = client.IncrementWithInit(key, (ulong)i, expire, 10);
                    Assert.IsTrue(result.HasValue);
                    Assert.AreEqual(result.Value, (ulong)i + 10);
                });
            });
        }

        [TestMethod()]
        public void DecrementTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    var result = client.Decrement(key, 10);
                    Assert.IsFalse(result.HasValue);
                    Assert.IsTrue(client.Set(key, i + 100));
                    result = client.Decrement(key, 10);
                    Assert.IsTrue(result.HasValue);
                    Assert.AreEqual(result.Value, (ulong)i + 90);
                });
            });
        }

        [TestMethod()]
        public void DecrementWithInitTest()
        {
            Parallel.ForEach(clients, client =>
            {
                Parallel.For(0, Count, i =>
                {
                    var key = Guid.NewGuid().ToString();
                    var result = client.DecrementWithInit(key, (ulong)i + 100, expire, 10);
                    Assert.IsTrue(result.HasValue);
                    Assert.AreEqual(result.Value, (ulong)i + 100);
                    result = client.DecrementWithInit(key, (ulong)i + 100, expire, 10);
                    Assert.IsTrue(result.HasValue);
                    Assert.AreEqual(result.Value, (ulong)i + 90);
                });
            });
        }

        [TestMethod()]
        public void LockerTest()
        {
            Parallel.ForEach(clients, client =>
            {
                var timeoutCount = 0;
                var key = Guid.NewGuid().ToString();
                Parallel.For(0, 10, i =>
                {
                    try
                    {
                        using (var locker = client.AcquireLock(key, TimeSpan.FromMilliseconds(100)))
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    catch (TimeoutException)
                    {
                        Interlocked.Increment(ref timeoutCount);
                    }
                });

                Assert.AreEqual(timeoutCount, 9);
            });
        }
    }
}
