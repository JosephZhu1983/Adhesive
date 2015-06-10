using System.Threading;
using Adhesive.Common;
using Adhesive.Persistence.Test.Classes;
using NUnit.Framework;

namespace Adhesive.Persistence.Test
{
    [TestFixture]
    public class SqlContextTest
    {
        [SetUp]
        public void SetUp()
        {

        }
        [Test]
        public void TestStorageContext()
        {
            AdhesiveFramework.Start();
            IDbContextFactory dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
            //string customerId = null;
            //using (CustomerDbContext context = dbContextFactory.CreateContext<CustomerDbContext>())
            //{
            //    context.Customers.Each(e => context.Customers.Remove(e));
            //    context.SaveChanges();
            //}
            using (CustomerDbContext context = dbContextFactory.CreateContext<CustomerDbContext>())
            {
                Customer c = new Customer();
                c.FirstName = "James";
                c.LastName = "Chan";
                context.Customers.Add(c);
                context.SaveChanges();
                //customerId = c.Id;
            }
            //using (CustomerDbContext context = dbContextFactory.CreateContext<CustomerDbContext>())
            //{
            //    Customer c = context.Customers.Find(customerId);
            //    if (c != null)
            //    {
            //        c.FirstName = "Alex";
            //        context.SaveChanges();
            //    }
            //}
            AdhesiveFramework.End();

        }
        [Test]
        public void TestConcurrency()
        {
            AdhesiveFramework.Start();
            string customerId = null;
            using (CustomerDbContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<CustomerDbContext>())
            {
                context.Customers.Each(e => context.Customers.Remove(e));
                context.SaveChanges();
            }
            using (CustomerDbContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<CustomerDbContext>())
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
                    using (CustomerDbContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<CustomerDbContext>())
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
                    using (CustomerDbContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<CustomerDbContext>())
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
            AdhesiveFramework.End();
        }
        [Test]
        public void TestSoftDelete()
        {

            using (CustomerDbContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext<CustomerDbContext>())
            {
                Customer c = context.Customers.Find("0b006236034842118e69f3fe1619f5d4");
                context.Customers.Remove(c);
                context.SaveChanges();
            }


        }
        [TearDown]
        public void TearDown()
        {

        }
    }
}
