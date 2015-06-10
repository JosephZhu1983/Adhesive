using System.Threading;
using Adhesive.Common;
using Adhesive.Persistence.Test.Classes;
using NUnit.Framework;

namespace Adhesive.Persistence.Test
{
    [TestFixture]
    public class MySqlTest
    {
        [SetUp]
        public void SetUp()
        {
            AdhesiveFramework.Start();
        }
        [Test]
        public void TestStorageContext()
        {

            string customerId = null;
            using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
            {
                context.Customers.Each(e => context.Customers.Remove(e));
                context.SaveChanges();
            }
            using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
            {
                Customer c = new Customer();
                c.FirstName = "James";
                c.LastName = "Chan";
                context.Customers.Add(c);
                context.SaveChanges();
                customerId = c.Id;
            }
            using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
            {
                Customer c = context.Customers.Find(customerId);
                if (c != null)
                {
                    c.FirstName = "Alex";
                    context.SaveChanges();
                }
            }


        }
        [Test]
        public void TestConcurrency()
        {
            string customerId = null;
            using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
            {
                context.Customers.Each(e => context.Customers.Remove(e));
                context.SaveChanges();
            }
            using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
            {
                Customer c = new Customer();
                c.FirstName = "James";
                c.LastName = "Chan";
                context.Customers.Add(c);
                context.SaveChanges();
                customerId = c.Id;
            }
            var th1 = new Thread(() =>
            {
                int n = 0;
                do
                {
                    using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
                    {
                        Customer c = context.Customers.Find(customerId);
                        if (c != null)
                        {
                            c.FirstName = "Alex";
                            context.SaveChanges();
                        }
                    }
                    n++;
                }
                while (n < 2);
            });
            th1.Start();

            var th2 = new Thread(() =>
            {
                int n = 0;
                do
                {
                    using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
                    {
                        Customer c = context.Customers.Find(customerId);
                        if (c != null)
                        {
                            c.FirstName = "Jhon";
                            context.SaveChanges();
                        }
                    }
                    n++;
                }
                while (n < 2);
            });
            th2.Start();

            th1.Join();
            th2.Join();
        }
        [Test]
        public void TestLargeWrite()
        {
            using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
            {
                for (int i = 0; i < 10000000; i++)
                {
                    Customer c = new Customer { FirstName = "James",LastName="Chan" };
                    context.Customers.Add(c);
                    context.SaveChanges();
                }

            }
        }
        [Test]
        public void TestSoftDelete()
        {

            using (MySqlContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<MySqlContext>())
            {
                Customer c = context.Customers.Find("f33a724271284a64b371c23bb01fbbf7");
                context.Customers.Remove(c);
                context.SaveChanges();
            }


        }
        [TearDown]
        public void TearDown()
        {
            AdhesiveFramework.End();
        }
    }
}
