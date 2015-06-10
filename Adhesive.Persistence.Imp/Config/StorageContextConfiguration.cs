
using System.Collections.Generic;
using Adhesive.Common;
using Adhesive.Config;

namespace Adhesive.Persistence.Imp.Config
{
    public class StorageContextConfiguration
    {
        private static readonly IConfigService _configService = LocalServiceLocator.GetService<IConfigService>();
        private static readonly StorageContextConfigurationEntity _defaultConfig;
        private static readonly StorageContextConfigurationEntity _storageContextConfig;
        private static readonly Dictionary<string, StorageContextConfigurationItem> _storageContextsCache = new Dictionary<string, StorageContextConfigurationItem>();
        private static readonly object _locker = new object();
        static StorageContextConfiguration()
        {
            _defaultConfig = new StorageContextConfigurationEntity
                                    {
#if DEBUG
                                        StorageContexts = new Dictionary<string, StorageContextConfigurationItem>
                                         {
                                             {Constants.DefaultContextName,
                                          new  StorageContextConfigurationItem
                                               {
                                                   Name = Constants.DefaultContextName,
                                                   ProviderName = "System.Data.SqlClient",
                                                   ConnectionString = "Server=.;Database=Adhesive;User ID=sa;Password=DLdNa9R+IFkkHxvWszyLHw==;Trusted_Connection=False;Persist Security Info=True",
                                               }}
                                         }
#else
                                        StorageContexts = new Dictionary<string, StorageContextConfigurationItem>
                                         {
                                             {Constants.DefaultContextName,
                                          new  StorageContextConfigurationItem
                                               {
                                                   Name = Constants.DefaultContextName,
                                                   ProviderName = "System.Data.SqlClient",
                                                   ConnectionString = "Server=192.168.1.75;Database=Adhesive;User ID=Aic_User;Password=9djGOh3dP5AxjRixWicFUw==;Trusted_Connection=False;Persist Security Info=True",
                                               }},
                                               {
                                                   "AlarmDbContext",   
                                                   new  StorageContextConfigurationItem
                                               {
                                                   Name = "AlarmDbContext",
                                                   ProviderName = "System.Data.SqlClient",
                                                   ConnectionString = "Server=192.168.1.75;Database=Adhesive;User ID=Aic_User;Password=9djGOh3dP5AxjRixWicFUw==;Trusted_Connection=False;Persist Security Info=True",
                                               }},
                                               }

#endif
                                    };
            _storageContextConfig = _configService.GetConfigItemValue(true, "StorageConfig", _defaultConfig);
            foreach (var sc in _storageContextConfig.StorageContexts)
            {
                _storageContextsCache[sc.Key] = sc.Value;
            }
        }
        public static StorageContextConfigurationItem GetStorageContext(string contextName)
        {
            StorageContextConfigurationItem storageContext;
            if (_storageContextsCache.TryGetValue(contextName, out storageContext))
                return storageContext;
            return storageContext ?? (_storageContextsCache[Constants.DefaultContextName]);
        }
    }
}
