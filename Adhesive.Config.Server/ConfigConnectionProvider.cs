using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using Adhesive.Common;

namespace Adhesive.Config.Server
{
    public class ConfigConnectionProvider
    {
        private static readonly StorageContextConfigurationEntity _defaultConfig;
        private static readonly StorageContextConfigurationEntity _localStorageContextConfig = null;
        private static StorageContextConfigurationItem _storageContextConfigurationItem;
        private static readonly Dictionary<string, string> _connectionStringCache = new Dictionary<string, string>();
        private static readonly object _locker = new object();
        static ConfigConnectionProvider()
        {
            _defaultConfig = new StorageContextConfigurationEntity
            {
                StorageContextConfigurationItemList = new List<StorageContextConfigurationItem>
                                         {
                                          new  StorageContextConfigurationItem
                                               {
                                                   Name = "DefaultContext",
                                                   ProviderName = "System.Data.SqlClient",
                                                   ConnectionString = "Server=.;Database=Adhesive;User ID=sa;Password=DLdNa9R+IFkkHxvWszyLHw==;Trusted_Connection=False;Persist Security Info=True",
                                               }
                                         }
            };
            _localStorageContextConfig = LocalConfigService.GetConfig("StorageConfig.config", _defaultConfig);
        }
        public static DbConnection GetConnection()
        {
            _storageContextConfigurationItem = _localStorageContextConfig.StorageContextConfigurationItemList[0];
            string providerInvariantName = _storageContextConfigurationItem.ProviderName;
            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(providerInvariantName);
            if (providerFactory == null)
                throw new InvalidOperationException(String.Format("The '{0}' provider is not registered on the local machine.", providerInvariantName));

            DbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = GetConnectionString(_storageContextConfigurationItem.ConnectionString);
            return connection;
        }
        public static string GetConnectionString(string encryptedConnectionString)
        {
            string connectionString;
            if (_connectionStringCache.TryGetValue(encryptedConnectionString, out connectionString))
                return connectionString;
            lock(_locker)
            {
                if (_connectionStringCache.TryGetValue(encryptedConnectionString, out connectionString))
                    return connectionString;
                connectionString = GetDescryptedConnectionString(encryptedConnectionString);
                if (connectionString != null)
                    _connectionStringCache.Add(encryptedConnectionString, connectionString);
            }
            return connectionString;
        }
        private static string GetDescryptedConnectionString(string connectionString)
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
    internal sealed class CryptoUtil
    {
        private static string _iv = "#kRz4rK^Z#kLMgQ^!ZHsY0u6";
        private static string _key = "S()%s@z^";
        /// <summary>          
        /// DES加密偏移量，必须是>=8位长的字符串          
        /// </summary>          
        public string IV
        {
            get { return _iv; }
            set { _iv = value; }
        }
        /// <summary>          
        /// DES加密的私钥，必须是8位长的字符串          
        /// </summary>          
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        /// <summary>          
        /// 对字符串进行DES加密          
        /// </summary>          
        /// <param name="sourceString">待加密的字符串</param>          
        /// <returns>加密后的BASE64编码的字符串</returns>          
        public static string Encrypt(string sourceString)
        {
            byte[] btKey = Encoding.Default.GetBytes(_key);
            byte[] btIV = Encoding.Default.GetBytes(_iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Encoding.Default.GetBytes(sourceString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>          
        /// 对DES加密后的字符串进行解密          
        /// </summary>          
        /// <param name="encryptedString">待解密的字符串</param>          
        /// <returns>解密后的字符串</returns>          
        public static string Decrypt(string encryptedString)
        {
            byte[] btKey = Encoding.Default.GetBytes(_key);
            byte[] btIV = Encoding.Default.GetBytes(_iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.Default.GetString(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }
    }
    [XmlRoot("StorageConfig")]
    public class StorageContextConfigurationEntity
    {
        [XmlArray("StorageContexts")]
        public List<StorageContextConfigurationItem> StorageContextConfigurationItemList { get; set; }
    }
    [XmlType("StorageContext")]
    public class StorageContextConfigurationItem
    {
        [XmlAttribute]
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
    }
}
