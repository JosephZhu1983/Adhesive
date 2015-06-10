
using System;
using System.Linq;
using Adhesive.Common;

namespace Adhesive.Mongodb.Server.Imp
{
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        public MongodbAdminConfigurationItem GetAdminConfiguration(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || password == null) return null;

            try
            {
                var config = MongodbServerConfiguration.GetConfig().MongodbAdminConfigurationItems;
                var configItem = config.Values.FirstOrDefault(c => c.UserName == username && c.Password == password);
                if (configItem.MongodbAdminDatabaseConfigurationItems == null)
                    LocalLoggingService.Error("MongodbAdminDatabaseConfigurationItems为空！！！ {0} {1}", username, password);
                return configItem;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Admin", "GetAdminConfiguration", ex.ToString());
                throw;
            }
        }

        public MongodbAdminConfigurationItem GetAdminConfigurationInternal(string username)
        {
            try
            {
                var config = MongodbServerConfiguration.GetConfig().MongodbAdminConfigurationItems;
                var configItem = config.Values.FirstOrDefault(c => c.UserName == username);
                return configItem;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Admin", "GetAdminConfigurationInternal", ex.ToString());
                throw;
            }
        }
    }
}
