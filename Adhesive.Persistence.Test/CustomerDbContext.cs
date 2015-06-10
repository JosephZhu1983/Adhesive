using System.Data.Entity;
using Adhesive.Persistence.Test.Classes;

namespace Adhesive.Persistence.Test
{
    [ContextAttribute("CustomerDbContext")]
    class CustomerDbContext : StorageContext
    {
        public CustomerDbContext(string contextName)
            : base(contextName)
        {
        }
        public DbSet<Customer> Customers { get; set; }
    }
}
