

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Adhesive.Config.Server
{
    public class ConfigDbContext : DbContext
    {
        public DbSet<ConfigItem> ConfigItems { get; set; }
        static ConfigDbContext()
        {
            Database.SetInitializer<ConfigDbContext>(
                new CreateDatabaseIfNotExists<ConfigDbContext>());
            //Database.SetInitializer<ConfigDbContext>(
            //   new DropCreateDatabaseIfModelChanges<ConfigDbContext>());
        }
        public ConfigDbContext()
            : base(ConfigConnectionProvider.GetConnection(),true)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Configuration.LazyLoadingEnabled = true;
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new ConfigItemConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
