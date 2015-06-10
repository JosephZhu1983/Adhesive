using Adhesive.Common;
using Adhesive.Persistence.Test.Classes;
using NUnit.Framework;

namespace Adhesive.Persistence.Test
{
    [TestFixture]
    public class RepositoryTest
    {
        [Test]
        public void TestRepository()
        {
            AdhesiveFramework.Start();
            Customer c2 = null;
            IDbContextFactory dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
            IRepositoryFactory repositoryFactory = LocalServiceLocator.GetService<IRepositoryFactory>();
            using (IAdhesiveUnitOfwork adhesiveUnitOfWork = dbContextFactory.CreateContext<AdhesiveUnitOfwork>())
            {
                ICustomerRepository customerRepository = repositoryFactory.CreateRepository<Customer, CustomerRepository>(adhesiveUnitOfWork);
                var c1 = new Customer { FirstName = "James" ,LastName = "Chan"};
                customerRepository.Add(c1);
                customerRepository.UnitOfWork.Commit();
                c2 = customerRepository.Get(c1.Id);
                Assert.IsNotNull(c2);
            }
            using (IAdhesiveUnitOfwork adhesiveUnitOfWork = dbContextFactory.CreateContext<AdhesiveUnitOfwork>())
            {
                ICustomerRepository customerRepository = repositoryFactory.CreateRepository<Customer, CustomerRepository>(adhesiveUnitOfWork);
                customerRepository.TrackItem(c2);
                c2.FirstName = "James";
                customerRepository.UnitOfWork.Commit();
            }

            AdhesiveFramework.End();
        }
    }
}
