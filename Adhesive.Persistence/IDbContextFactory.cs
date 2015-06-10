

using System;
using System.Data.Entity;

namespace Adhesive.Persistence
{
    public interface IDbContextFactory
    {
        TContext CreateContext<TContext>() where TContext : StorageContext;
        StorageContext CreateContext(Type storageContextType);
    }
}
