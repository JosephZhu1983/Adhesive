using System.Data.Entity;
using Adhesive.Persistence.Imp;
using Adhesive.Persistence.Test.Classes;

namespace Adhesive.Persistence.Test
{
    public class AdhesiveUnitOfwork : UnitOfWork, IAdhesiveUnitOfwork
    {
        public AdhesiveUnitOfwork(string contextName)
            : base(contextName)
        {
        }
        public DbSet<Customer> Customers { get; set; }
    }
}
