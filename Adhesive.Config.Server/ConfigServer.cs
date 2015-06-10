

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using Adhesive.Common;

namespace Adhesive.Config.Server
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true,
       ConcurrencyMode = ConcurrencyMode.Multiple,
       InstanceContextMode = InstanceContextMode.Single,
       Namespace = "Adhesive.Config.Server.ConfigServer")]
    public class ConfigServer : IConfigServer
    {
        public ConfigServer()
        {
        }

        public ConfigItem GetConfigItem(string appName, string cateName)
        {
            return GetConfigItem(appName, new[] { cateName });
        }

        public ConfigItem GetConfigItem(string appName, string cateName, string itemName)
        {
            return GetConfigItem(appName, new[] { cateName, itemName });
        }

        public ConfigItem GetConfigItem(string appName, string cateName, string subcateName, string itemName)
        {
            return GetConfigItem(appName, new[] { cateName, subcateName, itemName });
        }

        public object GetConfigItemValue(string appName, string cateName, Type valType, object defVal)
        {
            return GetConfigItemValue(appName, new[] { cateName }, valType, defVal);
        }

        public object GetConfigItemValue(string appName, string cateName, string itemName, Type valType, object defVal)
        {
            return GetConfigItemValue(appName, new[] { cateName, itemName }, valType, defVal);
        }

        public object GetConfigItemValue(string appName, string cateName, string subcateName, string itemName, Type valType, object defVal)
        {
            return GetConfigItemValue(appName, new[] { cateName, subcateName, itemName }, valType, defVal);
        }

        public T GetConfigItemValue<T>(string appName, string cateName, T defVal)
        {
            return (T)GetConfigItemValue(appName, cateName, typeof(T), defVal);
        }

        public T GetConfigItemValue<T>(string appName, string cateName, string itemName, T defVal)
        {
            return (T)GetConfigItemValue(appName, cateName, itemName, typeof(T), defVal);
        }

        public T GetConfigItemValue<T>(string appName, string cateName, string subcateName, string itemName, T defVal)
        {
            return (T)GetConfigItemValue(appName, cateName, subcateName, itemName, typeof(T), defVal);
        }
        private object GetConfigItemValue(string appName, string[] pathItemNames, Type valType, object defVal)
        {
            ConfigItem item = GetConfigItem(appName, pathItemNames);
            return item == null ? defVal : ConfigHelper.ChangeType(item.Value, valType, defVal);
        }
        public ConfigItem GetConfigItem(string id)
        {
            TransactionOptions transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                using (ConfigDbContext context = new ConfigDbContext())
                {
                    ConfigItem item = context.ConfigItems.Where(e => e.Id == id && ! e.IsDeleted).SingleOrDefault();
                    transaction.Complete();
                    return item;
                }
            }
        }
        public ConfigItem AddConfigItem(string appName, string[] parentPathItemNames, string name, string friendlyName, string desc, string val, string sourceId, string valType, string valTypeEnum,bool isCompositeValue)
        {
            List<string> pathItemNames = new List<string>();
            string parentId = null;
            if (parentPathItemNames != null && parentPathItemNames.Length != 0)
            {
                parentId = ConfigHelper.GenerateConfigItemId(appName, parentPathItemNames);
                pathItemNames.AddRange(parentPathItemNames);
            }
            pathItemNames.Add(name);
            string id = ConfigHelper.GenerateConfigItemId(appName, pathItemNames.ToArray());

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            ConfigItem item;
            bool isNew = true;
            using (ConfigDbContext context = new ConfigDbContext())
            {
                item = context.ConfigItems.Where(e => e.Id == id).SingleOrDefault();
                //节点已经存在，并且没有标记为删除，直接返回
                if (item != null && !item.IsDeleted)
                    return item;
                //节点已经存在，但是标记为已删除,添加时将其恢复
                if (item != null && item.IsDeleted)
                {
                    isNew = false;
                    item.IsDeleted = false;
                    item.ItemsInited = false;
                    item.ModifiedOn = DateTime.Now;
                }
                else
                {
                    item = new ConfigItem();
                }
                item.Id = id;
                item.ParentId = parentId;
                item.AppName = appName;
                item.Name = name;
                item.FriendlyName = friendlyName;
                item.Description = desc;
                item.Value = val;
                item.SourceId = sourceId;
                item.ValueType = valType;
                item.ValueTypeEnum = valTypeEnum;
                item.IsCompositeValue = isCompositeValue;
                if (isNew)
                    context.ConfigItems.Add(item);

                try
                    {
                        context.SaveChanges();
                        context.Entry(item).Reload();
                        return item;
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("添加配置项失败，详细信息：{0}",ex);
                            throw;
                    }
                
            }
            return item;
        }
        public bool IsConfigItemExists(string appName, params string[] pathItemNames)
        {
            ConfigItem item = GetConfigItem(appName, pathItemNames);
            return item != null;
        }
        
        public void SetItemsInited(string appName, params string[] pathItemNames)
        {
            if (pathItemNames == null)
                throw new ArgumentNullException("pathItemNames");
            string id = ConfigHelper.GenerateConfigItemId(appName, pathItemNames);
            using (ConfigDbContext context = new ConfigDbContext())
            {
                context.Database.ExecuteSqlCommand("UPDATE ConfigItem SET ItemsInited = 1 WHERE Id = @Id AND ItemsInited != 1",
                                                                        new SqlParameter("Id", id));
            }
        }

        public void SaveConfigItem(string id, string friendlyName, string desc, string val)
        {
            using (ConfigDbContext context = new ConfigDbContext())
            {
                ConfigItem item = context.ConfigItems.Where(e => e.Id == id && ! e.IsDeleted)
                                          .SingleOrDefault();
                if (item == null)
                    return;
                item.FriendlyName = friendlyName;
                item.Description = desc;
                item.Value = val;
                item.ModifiedOn = DateTime.Now;
                context.SaveChanges();
            }
        }

        public void RemoveConfigItem(string id)
        {
            ConfigItem[] childConfigItems = GetChildConfigItems(id);
            if (childConfigItems == null || childConfigItems.Length == 0)
            {
                InternalRemoveConfigItem(id);
                return;
            }
            foreach (ConfigItem configItem in childConfigItems)
            {
                RemoveConfigItem(configItem.Id);
            }
            InternalRemoveConfigItem(id);

        }

        internal void InternalRemoveConfigItem(string id)
        {
            using (ConfigDbContext context = new ConfigDbContext())
            {
                ConfigItem configItem = context.ConfigItems.Where(e => e.Id == id && ! e.IsDeleted).SingleOrDefault();
                if (configItem == null)
                    return;
                configItem.IsDeleted = true;
                context.SaveChanges();
            }
        }


        public ConfigItem GetConfigItem(string appName, params string[] pathItemNames)
        {
            if (pathItemNames == null)
                throw new ArgumentNullException("pathItemNames");
            string pathHash = ConfigHelper.GenerateConfigItemId(appName, pathItemNames);
            TransactionOptions transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                using (ConfigDbContext context = new ConfigDbContext())
                {
                    var configItem = context.ConfigItems.Where(e => e.Id == pathHash && ! e.IsDeleted).SingleOrDefault();
                    transactionScope.Complete();
                    return configItem;
                }
            }
        }
        public ConfigItem[] GetChildConfigItems(string appName, params string[] parentPathItemNames)
        {
            if (parentPathItemNames == null)
                throw new ArgumentNullException("parentPathItemNames");
            string parentId = ConfigHelper.GenerateConfigItemId(appName, parentPathItemNames);
            TransactionOptions transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                using (ConfigDbContext context = new ConfigDbContext())
                {
                    var configItems = context.ConfigItems.Where(e => e.ParentId == parentId && ! e.IsDeleted)
                        .ToArray();
                    transactionScope.Complete();
                    return configItems;
                }
            }
        }
        public ConfigItem[] GetChildConfigItems(string parentId)
        {
            using (ConfigDbContext context = new ConfigDbContext())
            {
                return context.ConfigItems.Where(e => e.ParentId == parentId && ! e.IsDeleted).OrderBy(e => e.CreatedOn)
                    .ToArray();
            }
        }
        public ConfigItem[] GetTopLevelConfigItems(string appName)
        {
            using (ConfigDbContext context = new ConfigDbContext())
            {
                ConfigItem[] configItems;
                if (appName == null)
                    configItems = context.Database.SqlQuery<ConfigItem>("SELECT * FROM ConfigItem WHERE (AppName IS NULL) AND (ParentId IS NULL) AND IsDeleted = 0").ToArray();
                else
                    configItems = context.Database.SqlQuery<ConfigItem>("SELECT * FROM ConfigItem WHERE  (AppName = @AppName)  AND (ParentId IS NULL) AND IsDeleted = 0",
                                                                        new SqlParameter("AppName", appName)).ToArray();
                return configItems;
            }
        }

        public ConfigItem[] GetLastUpdatedConfigItems(string appName, ref byte[] lastRowVersion, bool includeDeletedItems)
        {
            try
            {
                using (ConfigDbContext context = new ConfigDbContext())
                {
                    ConfigItem[] configItems;
                    if (lastRowVersion == null)
                        configItems = context.Database.SqlQuery<ConfigItem>(
                            "SELECT * FROM ConfigItem WHERE (AppName IS NULL OR AppName = @AppName) ORDER BY RowVersion",
                            new SqlParameter("AppName", appName)).ToArray();
                    else
                        configItems = context.Database.SqlQuery<ConfigItem>(
                            "SELECT * FROM ConfigItem  WHERE (AppName IS NULL OR AppName = @AppName)  AND (RowVersion > @RowVersion) ORDER BY RowVersion",
                            new SqlParameter("RowVersion", lastRowVersion), new SqlParameter("AppName", appName)).ToArray();

                    if (configItems != null && configItems.Length != 0)
                    {
                        lastRowVersion = configItems[configItems.Length - 1].RowVersion;
                    }
                    return configItems;
                }
            }
            catch(Exception ex)
            {
                LocalLoggingService.Error(ex.ToString());
            }
            return null;
        }


        public ConfigItem[] GetLastUpdatedConfigItems(string appName, ref byte[] lastRowVersion, int limit)
        {
            try
            {
                using (ConfigDbContext context = new ConfigDbContext())
                {
                    ConfigItem[] configItems;
                    if (lastRowVersion == null)
                        configItems = context.Database.SqlQuery<ConfigItem>(
                            string.Format("SELECT TOP {0} * FROM ConfigItem WHERE (AppName IS NULL OR AppName = @AppName) ORDER BY RowVersion", limit),
                            new SqlParameter("AppName", appName)).ToArray();
                    else
                        configItems = context.Database.SqlQuery<ConfigItem>(
                            string.Format("SELECT TOP {0} * FROM ConfigItem  WHERE (AppName IS NULL OR AppName = @AppName)  AND (RowVersion > @RowVersion) ORDER BY RowVersion", limit),
                            new SqlParameter("RowVersion", lastRowVersion), new SqlParameter("AppName", appName)).ToArray();

                    if (configItems != null && configItems.Length != 0)
                    {
                        lastRowVersion = configItems[configItems.Length - 1].RowVersion;
                    }
                    return configItems;
                }
            }
            catch(Exception ex)
            {
                LocalLoggingService.Error(ex.ToString());
            }
            return null;
        }
    }
}
