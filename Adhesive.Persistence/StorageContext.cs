
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Adhesive.Common;
using Adhesive.Domain;

namespace Adhesive.Persistence
{
    public abstract class StorageContext : DbContext
    {
        private static readonly IConnectionProvider ConnectionProvider = LocalServiceLocator.GetService<IConnectionProvider>();
        protected StorageContext(string contextName)
            : base(ConnectionProvider.GetConnection(contextName), true)
        {
        }
        public override int SaveChanges()
        {
            //自动审核
            var nowAuditDate = DateTime.Now;
            var changeSet = ChangeTracker.Entries<IAuditable>();
            if (changeSet != null)
                foreach (DbEntityEntry<IAuditable> dbEntityEntry in changeSet)
                {

                    switch (dbEntityEntry.State)
                    {
                        case EntityState.Added:
                            dbEntityEntry.Entity.CreatedOn = nowAuditDate;
                            dbEntityEntry.Entity.ModifiedOn = nowAuditDate;
                            break;
                        case EntityState.Modified:
                            dbEntityEntry.Entity.ModifiedOn = nowAuditDate;
                            break;
                    }
                }
            //乐观并发控制
            foreach (var dbEntityEntry in ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
            {
                IVersionable entity = dbEntityEntry.Entity as IVersionable;
                if (entity != null)
                {
                    entity.RowVersion = entity.RowVersion + 1;
                }
            }
            //Id自动生成
            foreach (var dbEntityEntry in ChangeTracker.Entries().Where(x => x.State == EntityState.Added))
            {
                IIdentifiable entity = dbEntityEntry.Entity as IIdentifiable;
                if (entity != null)
                {
                    if (entity.Id == Guid.Empty.ToString("N"))
                    {
                        IIdentityGenerator identityGenerator = LocalServiceLocator.GetService<IIdentityGenerator>();
                        if (identityGenerator != null)
                            entity.Id = (string)identityGenerator.NewId();
                    }
                }
            }
            //逻辑删除
            foreach (var dbEntityEntry in ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted))
            {
                ISoftDeletable entity = dbEntityEntry.Entity as ISoftDeletable;
                if (entity != null)
                {
                    if (!entity.IsDeleted)
                        entity.IsDeleted = true;
                    base.Entry(entity).State = EntityState.Modified;
                }
            }
            return base.SaveChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
