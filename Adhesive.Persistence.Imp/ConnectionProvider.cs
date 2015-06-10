
using System;
using System.Data;
using System.Data.Common;
using Adhesive.Persistence.Imp.Config;

namespace Adhesive.Persistence.Imp
{
    public class ConnectionProvider : IConnectionProvider
    {
        private bool _isDisposed;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private StorageContextConfigurationItem _storageContextConfigurationItem;
        public ConnectionProvider(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }
        public void CloseConnection(DbConnection conn)
        {
            if (conn == null)
                return;
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DbConnection GetConnection(string contextName)
        {
            _storageContextConfigurationItem = StorageContextConfiguration.GetStorageContext(contextName);
            string providerInvariantName = _storageContextConfigurationItem.ProviderName;
            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(providerInvariantName);
            if (providerFactory == null)
                throw new InvalidOperationException(String.Format("The '{0}' provider is not registered on the local machine.", providerInvariantName));

            DbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = _connectionStringProvider.GetConnectionString(_storageContextConfigurationItem.Name);
            return connection;
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {

            }
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~ConnectionProvider()
        {
            Dispose(false);
        }
    }
}
