
using System;
using System.Collections.Generic;
using Adhesive.Persistence.Imp.Config;

namespace Adhesive.Persistence.Imp
{
    internal class ConnectionStringProvider : IConnectionStringProvider
    {
        private static readonly Dictionary<string, string> _connectionStringCache = new Dictionary<string, string>();
        private static readonly object _locker = new object();
        public string GetConnectionString(string contextName)
        {
            string connectionString = null;
            if (contextName == null)
                throw new ArgumentNullException("contextName");
            if (_connectionStringCache.TryGetValue(contextName, out connectionString))
                return connectionString;
            lock (_locker)
            {
                if (_connectionStringCache.TryGetValue(contextName, out connectionString))
                    return connectionString;
                connectionString = InternalGetConnectionString(contextName);
                if (connectionString != null)
                    _connectionStringCache.Add(contextName, connectionString);
            }
            return connectionString;
        }
        private string InternalGetConnectionString(string contextName)
        {
            StorageContextConfigurationItem storageContextConfigurationItem = StorageContextConfiguration.GetStorageContext(contextName);
            if (storageContextConfigurationItem == null)
                return null;
            string connectionString = storageContextConfigurationItem.ConnectionString;
            return GetDescryptedConnectionString(connectionString);
        }
        private string GetDescryptedConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return null;
            string[] passwordOptionNames = { "Password", "Pwd" };
            foreach (string passwordOptionName in passwordOptionNames)
            {
                int passwordOptionNameIndex = -1;
                int equalsignIndex = -1;
                int semicolonIndex = -1;
                int passwordLength = 0;
                string encryptedPassword;
                string descryptedPassword;
                passwordOptionNameIndex = connectionString.IndexOf(passwordOptionName, StringComparison.CurrentCultureIgnoreCase);
                if (passwordOptionNameIndex != -1)
                {
                    equalsignIndex = connectionString.IndexOf('=', passwordOptionNameIndex + passwordOptionName.Length);
                    if (equalsignIndex == -1)
                        return connectionString;
                    semicolonIndex = connectionString.IndexOf(';', equalsignIndex + 1);
                    if (semicolonIndex == -1)
                        passwordLength = connectionString.Length - (equalsignIndex + 1);
                    else
                        passwordLength = semicolonIndex - (equalsignIndex + 1);
                    if (passwordLength <= 0)
                        return connectionString;
                    encryptedPassword = connectionString.Substring(equalsignIndex + 1, passwordLength);
                    if (string.IsNullOrEmpty(encryptedPassword))
                        return connectionString;
                    descryptedPassword = CryptoUtil.Decrypt(encryptedPassword);
                    connectionString = connectionString.Replace(encryptedPassword, descryptedPassword);
                    return connectionString;
                }
            }
            return connectionString;
        }
    }
}
