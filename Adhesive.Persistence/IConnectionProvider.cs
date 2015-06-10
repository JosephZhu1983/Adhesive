
using System;
using System.Data.Common;

namespace Adhesive.Persistence
{
    public interface IConnectionProvider : IDisposable
    {
        void CloseConnection(DbConnection conn);
        DbConnection GetConnection(string contextName);
    }
}
