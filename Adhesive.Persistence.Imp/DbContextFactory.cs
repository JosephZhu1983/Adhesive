
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
namespace Adhesive.Persistence.Imp
{
    public class DbContextFactory : IDbContextFactory
    {
        public DbContextFactory()
        {
        }
        public TContext CreateContext<TContext>() where TContext : StorageContext
        {
            Type contextType = typeof(TContext);
            return (TContext) CreateContext(contextType);
        }
        public StorageContext CreateContext(Type storageContextType)
        {
            Type contextType = storageContextType;
            string contextName = null;
            ContextAttribute contextAttribute = contextType.GetCustomAttributes(false).OfType<ContextAttribute>().SingleOrDefault();
            if (contextAttribute == null)
                contextAttribute = new ContextAttribute(null);
            contextName = contextAttribute.ContextName;
            if (string.IsNullOrEmpty(contextName) || contextName.Trim() == string.Empty)
                contextName = contextType.Name;
            StorageContext context = (StorageContext)Activator.CreateInstance(contextType, contextName);
            return context;
        }
    }
}
