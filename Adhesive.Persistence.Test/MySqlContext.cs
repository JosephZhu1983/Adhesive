using System.Data.Entity;
using Adhesive.Persistence.Test.Classes;

namespace Adhesive.Persistence.Test
{
    public class MySqlContext : StorageContext
    {
        public MySqlContext(string contextName)
            : base(contextName)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        static MySqlContext()
        {
            Database.SetInitializer<MySqlContext>(
                new CreateDatabaseIfNotExists<MySqlContext>());

        }
    }
}
