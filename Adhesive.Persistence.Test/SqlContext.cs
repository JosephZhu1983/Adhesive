using System.Data.Entity;
using Adhesive.Persistence.Test.Classes;

namespace Adhesive.Persistence.Test
{
    [ContextAttribute("SqlContext")]
    class SqlContext : StorageContext
    {
        public SqlContext(string contextName)
            : base(contextName)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        static SqlContext()
        {
            Database.SetInitializer<SqlContext>(
                new CreateDatabaseIfNotExists<SqlContext>());

        }
    }
}
