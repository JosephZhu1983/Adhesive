using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using Adhesive.Persistence;

namespace Adhesive.Alarm.Common
{
    [ContextAttribute("AlarmDbContext")]
    public class AlarmDbContext : StorageContext
    {
        public AlarmDbContext(string contextName)
            : base(contextName)
        {
        }
        public DbSet<AlarmItem> AlarmItems { get; set; }

        public DbSet<AlarmProcessItem> AlarmProcessItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Configuration.LazyLoadingEnabled = true;
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new AlarmItemConfiguration());
            modelBuilder.Configurations.Add(new AlarmProcessItemConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
