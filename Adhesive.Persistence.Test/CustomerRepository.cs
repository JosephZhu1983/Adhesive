using Adhesive.Persistence.Imp;
using Adhesive.Persistence.Test.Classes;

namespace Adhesive.Persistence.Test
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IAdhesiveUnitOfwork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
