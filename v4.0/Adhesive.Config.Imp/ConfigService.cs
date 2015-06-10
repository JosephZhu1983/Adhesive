
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Adhesive.Common;

namespace Adhesive.Config.Imp
{
    public class ConfigService : AbstractService, IConfigService
    {
        private readonly IConfigServer ConfigServer;
        private string _appName;
        private byte[] _lastRowVersion;
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<string, ConfigItem> _configCache = new Dictionary<string, ConfigItem>();
        private readonly Dictionary<string, ConfigItemValueUpdateCallback> _dicConfigItemValueUpdateCallback = new Dictionary<string, ConfigItemValueUpdateCallback>();
        private Thread _syncConfigCacheThread;
        private const int DefaultSyncConfigCacheInterval = 10000;
        private int _syncConfigCacheInterval = DefaultSyncConfigCacheInterval;
        private const int DefaultActionNumRetries = 3;
        private int _actionNumRetries = DefaultActionNumRetries;
        private const int DefaultActionRetryTimeout = 10;
        private int _actionRetryTimeout = DefaultActionRetryTimeout;
        private const int DefaultBatchLoadSize = 1000;
        private int _batchLoadSize = DefaultBatchLoadSize;
        private bool _tryCreateConfigItemIfNotExists = true;
        private readonly string BooleanTypeItemName = typeof(bool).FullName;
        private bool _isStoped = false;
        public ConfigService()
        {
            ConfigServer = ConfigServiceLocator.GetService();
        }
        public void Init()
        {
            _syncConfigCacheThread = new Thread(SyncConfigCacheWorker);
            _appName = CommonConfiguration.GetConfig().ApplicationName;
            RegisterApplication(_appName);
            EnsureTypeRepositoryItemAdded();
            EnsureUnderlyingTypeItemAdded();
            EnsureBooleanTypeItemAdded();
            BatchLoadConfigItems();
            _syncConfigCacheInterval = GetConfigItemValue(true, this.GetType().FullName, "SyncConfigCacheInterval", DefaultSyncConfigCacheInterval, SyncConfigCacheIntervalUpdateCallback);
            _actionNumRetries = GetConfigItemValue(true, this.GetType().FullName, "ActionNumRetries", DefaultActionNumRetries, ActionNumRetriesUpdateCallback);
            _actionRetryTimeout = GetConfigItemValue(true, this.GetType().FullName, "ActionRetryTimeout", DefaultActionRetryTimeout, ActionRetryTimeoutUpdateCallback);
            _tryCreateConfigItemIfNotExists = GetConfigItemValue(true, this.GetType().FullName, "TryCreateConfigItemIfNotExists", _tryCreateConfigItemIfNotExists);
            _batchLoadSize = GetConfigItemValue(true, this.GetType().FullName, "BatchLoadSize", DefaultBatchLoadSize);
            _syncConfigCacheThread.IsBackground = true;
            _syncConfigCacheThread.Start();
        }

        public T GetConfigItemValue<T>(string cateName, T defVal)
        {
            return (T)InternalGetConfigItemValue(null, new[] { cateName }, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(string cateName, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(null, new[] { cateName }, typeof(T), defVal, callback);
        }

        public T GetConfigItemValue<T>(string cateName, string itemName, T defVal)
        {
            return (T)InternalGetConfigItemValue(null, new[] { cateName, itemName }, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(string cateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(null, new[] { cateName, itemName }, typeof(T), defVal, callback);
        }

        public T GetConfigItemValue<T>(string cateName, string subcateName, string itemName, T defVal)
        {
            return (T)InternalGetConfigItemValue(null, new[] { cateName, subcateName, itemName }, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(string cateName, string subcateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(null, new[] { cateName, subcateName, itemName }, typeof(T), defVal, callback);
        }

        public T GetConfigItemValue<T>(string[] pathItemNames, T defVal)
        {
            return (T)InternalGetConfigItemValue(null, pathItemNames, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(string[] pathItemNames, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(null, pathItemNames, typeof(T), defVal, callback);
        }

        public T GetConfigItemValue<T>(bool global, string cateName, T defVal)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, new[] { cateName }, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(bool global, string cateName, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, new[] { cateName }, typeof(T), defVal, callback);
        }

        public T GetConfigItemValue<T>(bool global, string cateName, string itemName, T defVal)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, new[] { cateName, itemName }, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(bool global, string cateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, new[] { cateName, itemName }, typeof(T), defVal, callback);
        }

        public T GetConfigItemValue<T>(bool global, string cateName, string subcateName, string itemName, T defVal)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, new[] { cateName, subcateName, itemName }, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(bool global, string cateName, string subcateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, new[] { cateName, subcateName, itemName }, typeof(T), defVal, callback);
        }

        public T GetConfigItemValue<T>(bool global, string[] pathItemNames, T defVal)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, pathItemNames, typeof(T), defVal, null);
        }

        public T GetConfigItemValue<T>(bool global, string[] pathItemNames, T defVal, ConfigItemValueUpdateCallback callback)
        {
            return (T)InternalGetConfigItemValue(global ? null : _appName, pathItemNames, typeof(T), defVal, callback);
        }


        private ConfigItem EnsureEnumTypeAdded(string appName, Type enumType)
        {
            string enumTypeName = ConfigHelper.GetConfigItemValueType(enumType);
            ConfigItem item = InternalGetConfigItem(appName, Constants.TypeRepositoryItemName, enumTypeName);
            DescriptionAttribute descriptionAttribute = enumType.GetCustomAttributes(false).OfType<DescriptionAttribute>().SingleOrDefault();
            string friendlyName = descriptionAttribute == null ? enumTypeName : (string.IsNullOrEmpty(descriptionAttribute.Description) ? enumTypeName : descriptionAttribute.Description);

            ConfigItem parentItem = InternalGetConfigItem(appName, Constants.TypeRepositoryItemName, enumTypeName);
            if (parentItem == null)
            {
                parentItem = InternalAddConfigItem(appName, new[] { Constants.TypeRepositoryItemName }, enumTypeName, friendlyName, null, null, null, null, null, ConfigHelper.IsCompositeValue(enumType));
            }
            if (parentItem == null)
                return null;
            foreach (FieldInfo fi in enumType.GetFields())
            {
                if (fi.Name == "value__")
                    continue;
                string itemName = fi.Name;
                descriptionAttribute = fi.GetCustomAttributes(false).OfType<DescriptionAttribute>().SingleOrDefault();
                friendlyName = descriptionAttribute == null ? itemName : (string.IsNullOrEmpty(descriptionAttribute.Description) ? itemName : descriptionAttribute.Description);
                ConfigItem childItem = InternalGetConfigItem(appName, Constants.TypeRepositoryItemName, enumTypeName, itemName);
                if (childItem == null)
                {
                    string value = null;
                    try
                    {
                        value = (fi.GetValue(Activator.CreateInstance(enumType))).ToString();
                    }
                    catch (Exception ex) { }
                    childItem = InternalAddConfigItem(appName, new string[] { Constants.TypeRepositoryItemName, enumTypeName }, itemName, friendlyName, null, value, null, ConfigHelper.GetConfigItemValueType(enumType), ConfigHelper.GetConfigItemValueTypeEnum(enumType), ConfigHelper.IsCompositeValue(enumType));
                }
            }
            return item;
        }

        private ConfigItem InternalGetConfigItem(string appName, params string[] pathItemNames)
        {
            string pathHash = ConfigHelper.GenerateConfigItemId(appName, pathItemNames);
            return InternalGetConfigItem(pathHash);
        }
        private ConfigItem InternalGetConfigItem(string id)
        {
            ConfigItem item = GetConfigItemFromCache(id);
            if (item == null)
            {
                item = ConfigServer.GetConfigItem(id);
                UpdateConfigItemCache(item);
            }
            return item;
        }
        private ConfigItem GetConfigItemFromCache(string id)
        {
            ConfigItem item = null;
            _locker.EnterUpgradeableReadLock();
            try
            {
                if (_configCache.TryGetValue(id, out item))
                {
                    return item;
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
            return null;
        }
        private ConfigItem[] InternalGetChildConfigItems(string appName, params string[] parentPathItemNames)
        {
            ConfigItem[] configItems = null;
            ConfigItem parentConfigItem = InternalGetConfigItem(appName, parentPathItemNames);
            if (parentConfigItem == null)
                return null;
            if (parentConfigItem.ChildItems == null || parentConfigItem.ChildItems.Count == 0)
            {
                configItems = ConfigServer.GetChildConfigItems(appName, parentPathItemNames);
                if (configItems != null)
                    configItems.Each(e => UpdateConfigItemCache(e));
                return configItems;
            }
            return parentConfigItem.ChildItems.ToArray();
        }
        private object InternalGetConfigItemValue(string appName, string[] pathItemNames, Type valType, object defVal, ConfigItemValueUpdateCallback configItemValueUpdateCallback)
        {
            try
            {
                
                ConfigItem item = InternalGetConfigItem(appName, pathItemNames);
                if (item != null && item.ObjectValue != null)
                {
                    return item.ObjectValue;
                }
                if (_tryCreateConfigItemIfNotExists)
                    item = EnsureConfigItemAdded(appName, pathItemNames, valType, defVal);
                if (item == null)
                {
                    LocalLoggingService.Warning("配置项为NULL，valType：{0}，appName:{1}，pathItemNames：{2}", valType,appName, string.Join(",", pathItemNames.ToArray()));
                    return defVal;
                }
                object objectValue = GetTypeConfigItemValue(item, valType, ConfigHelper.IsUnderlyingType(valType) ? defVal : Activator.CreateInstance(valType), new List<string>(pathItemNames));
                _locker.EnterUpgradeableReadLock();
                try
                {
                    if (!_configCache.TryGetValue(item.Id, out item))
                        return objectValue;
                    _locker.EnterWriteLock();
                    try
                    {
                        _configCache[item.Id].ObjectValue = objectValue;
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }
                finally
                {
                    _locker.ExitUpgradeableReadLock();
                }
                if (configItemValueUpdateCallback != null)
                {
                    _locker.EnterUpgradeableReadLock();
                    try
                    {
                        string id = ConfigHelper.GenerateConfigItemId(appName, pathItemNames);
                        if (!_dicConfigItemValueUpdateCallback.ContainsKey(id))
                        {
                            _locker.EnterWriteLock();
                            try
                            {
                                _dicConfigItemValueUpdateCallback[id] = configItemValueUpdateCallback;
                            }
                            finally
                            {
                                _locker.ExitWriteLock();
                            }
                        }
                    }
                    finally
                    {
                        _locker.ExitUpgradeableReadLock();
                    }
                }
                return objectValue;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("获取配置项值出错，异常信息：{0}", ex);
                return defVal;
            }
        }
        private ConfigItem EnsureParentConfigItemsAdded(string appName, List<string> parentPathItemNames)
        {
            if (parentPathItemNames == null || parentPathItemNames.Count == 0)
                return null;
            ConfigItem parentConfigItem = null;
            int n = 0;
            foreach (string pathItemName in parentPathItemNames)
            {
                ConfigItem configItem = InternalGetConfigItem(appName, parentPathItemNames.Take(n + 1).ToArray());
                if (configItem == null)
                {
                    try
                    {
                        parentConfigItem = InternalAddConfigItem(appName, parentPathItemNames.Take(n).ToArray(), pathItemName, pathItemName, null, null, null, null, null, false);
                    }
                    catch (Exception ex)
                    {
                        parentConfigItem = null;
                    }
                }
                else
                    parentConfigItem = configItem;
                if (parentConfigItem == null)
                    return null;
                n++;
            }
            return parentConfigItem;
        }
        private ConfigItem EnsureConfigItemAdded(string appName, string[] pathItemNames, Type valType, object defVal)
        {
            List<string> parentPathItemNames = pathItemNames.Take(pathItemNames.Length - 1).ToList();
            ConfigItem parentConfigItem = EnsureParentConfigItemsAdded(appName, parentPathItemNames);
            if (parentConfigItem == null && parentPathItemNames.Count > 0)
                return null;
            string name = pathItemNames.LastOrDefault();
            return AddTypeConfigItem(true, appName, parentPathItemNames, name, name, null, valType, defVal);
        }
        private ConfigItem AddTypeConfigItem(bool isTopLevel, string appName, List<string> parentPathItemNames, string name, string friendlyName, string desc, Type valType, object defVal)
        {
            ConfigItemAttribute configItemAttribute;
            ConfigEntityAttribute configEntityAttribute;
            string value = null;

            if (ConfigHelper.IsUnderlyingType(valType))
            {
                value = defVal != null ? defVal.ToString() : null;
            }
            else if (isTopLevel)
            {
                configEntityAttribute = valType.GetCustomAttributes(false).OfType<ConfigEntityAttribute>().SingleOrDefault();
                if (configEntityAttribute == null)
                    configEntityAttribute = new ConfigEntityAttribute();
                friendlyName = configEntityAttribute.FriendlyName ?? name;
                desc = configEntityAttribute.Description;
            }
            List<string> childPathItemnames = new List<string>(parentPathItemNames) { name };
            ConfigItem configItem = InternalGetConfigItem(appName, childPathItemnames.ToArray());
            if (configItem == null)
            {
                configItem = InternalAddConfigItem(appName, parentPathItemNames.ToArray(), name, friendlyName, desc, value,GetConfigItemSourceId(valType), ConfigHelper.GetConfigItemValueType(valType),ConfigHelper.GetConfigItemValueTypeEnum(valType),ConfigHelper.IsCompositeValue(valType));
            }
            if (ConfigHelper.IsUnderlyingType(valType))
            {
                if (valType.IsEnum)
                    EnsureEnumTypeAdded(null, valType);
                return configItem;
            }
            List<string> pathItemNames = new List<string>(parentPathItemNames) { configItem.Name };
            if (typeof(IList).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length != 1 || !valType.IsGenericType)
                    return configItem;
                Type listItemType = genericArguments.First();
                int listIndex = 0;
                IList defListVal = (IList)defVal;
                if (defListVal != null && ! configItem.ItemsInited)
                {
                    foreach (object listItem in defListVal)
                    {
                        listItemType = listItem.GetType();
                        name = string.Format("{0}_{1}", configItem.Name, listIndex++);
                        AddTypeConfigItem(false, appName, pathItemNames, name, name, null, listItemType, listItem);
                    }
                    ConfigServer.SetItemsInited(appName, pathItemNames.ToArray());
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length != 2 || genericArguments.First() != typeof(string) || !valType.IsGenericType)
                    return configItem;
                Type dicItemTypeOfValue = genericArguments.Last();
                IDictionary defDicVal = (IDictionary)defVal;
                if (defDicVal != null && !configItem.ItemsInited)
                {
                    foreach (DictionaryEntry dicItem in defDicVal)
                    {
                        dicItemTypeOfValue = dicItem.Value.GetType();
                        name = string.Format("{0}", dicItem.Key);
                        AddTypeConfigItem(false, appName, pathItemNames, name, name, null, dicItemTypeOfValue, dicItem.Value);
                    }
                    ConfigServer.SetItemsInited(appName, pathItemNames.ToArray());
                }
            }
            else
            {
                FieldInfo[] fis = valType.GetFields();
                foreach (FieldInfo fi in fis)
                {
                    if (valType == fi.FieldType)
                        continue;
                    configItemAttribute = fi.GetCustomAttributes(false).OfType<ConfigItemAttribute>().SingleOrDefault();
                    if (configItemAttribute == null)
                        configItemAttribute = new ConfigItemAttribute();
                    friendlyName = configItemAttribute.FriendlyName ?? fi.Name;
                    desc = configItemAttribute.Description;
                    AddTypeConfigItem(false, appName, pathItemNames, fi.Name, friendlyName, desc, fi.FieldType, ConfigHelper.IsUnderlyingType(fi.FieldType) ? fi.GetValue(defVal) : fi.GetValue(defVal) ?? Activator.CreateInstance(fi.FieldType));
                }

                PropertyInfo[] pis = valType.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (valType == pi.PropertyType)
                        continue;
                    configItemAttribute = pi.GetCustomAttributes(false).OfType<ConfigItemAttribute>().SingleOrDefault();
                    if (configItemAttribute == null)
                        configItemAttribute = new ConfigItemAttribute();
                    friendlyName = configItemAttribute.FriendlyName ?? pi.Name;
                    desc = configItemAttribute.Description;
                    AddTypeConfigItem(false, appName, pathItemNames, pi.Name, friendlyName, desc, pi.PropertyType, ConfigHelper.IsUnderlyingType(pi.PropertyType) ? pi.GetValue(defVal, null) : pi.GetValue(defVal, null) ?? Activator.CreateInstance(pi.PropertyType));

                }

            }
            return configItem;
        }
        private object GetTypeConfigItemValue(ConfigItem configItem, Type valType, object defVal, List<string> pathItemNames)
        {
            if (configItem == null)
            {
                LocalLoggingService.Warning("配置项为NULL，valType：{0}，pathItemNames：{1}", valType, string.Join(",", pathItemNames.ToArray()));
                return defVal;
            }
            if (ConfigHelper.IsUnderlyingType(valType))
            {
                return ConfigHelper.ChangeType(configItem.Value, valType, defVal);
            }
            if (typeof(IList).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length != 1 || !valType.IsGenericType)
                    return null;
                Type listItemType = genericArguments.First();
                IList list = (IList)Activator.CreateInstance(valType);
                ConfigItem[] childConfigItems = InternalGetChildConfigItems(configItem.AppName, pathItemNames.ToArray());
                if (childConfigItems == null || childConfigItems.Length == 0)
                    return list;
                foreach (ConfigItem childItem in childConfigItems)
                {
                    listItemType = listItemType != typeof(object) ? listItemType : Type.GetType(childItem.ValueType);
                    List<string> childPathItemNames = new List<string>(pathItemNames) { childItem.Name };
                    if (listItemType == null)
                    {
                        LocalLoggingService.Warning("listItemType 为NULL,childItem.ValueType：{0},pathItemNames：{1}", childItem.ValueType, string.Join(",", childPathItemNames.ToArray()));
                        return defVal;
                    }
                    object itemValue = GetTypeConfigItemValue(childItem, listItemType, listItemType == typeof(string) ? null : Activator.CreateInstance(listItemType), childPathItemNames);
                    list.Add(itemValue);
                }
                return list;
            }
            else if (typeof(IDictionary).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length != 2 || genericArguments.First() != typeof(string) || !valType.IsGenericType)
                    return null;
                Type dicItemTypeOfValue = null;
                IDictionary dic = (IDictionary)Activator.CreateInstance(valType);
                ConfigItem[] childConfigItems = InternalGetChildConfigItems(configItem.AppName, pathItemNames.ToArray());
                if (childConfigItems == null || childConfigItems.Length == 0)
                    return dic;
                foreach (ConfigItem childItem in childConfigItems)
                {
                    dicItemTypeOfValue = genericArguments.Last();
                    dicItemTypeOfValue = dicItemTypeOfValue != typeof(object) ? dicItemTypeOfValue : Type.GetType(childItem.ValueType);
                    List<string> childPathItemNames = new List<string>(pathItemNames) { childItem.Name };
                    if (dicItemTypeOfValue == null)
                    {
                        LocalLoggingService.Warning("dicItemTypeOfValue 为NULL,childItem.ValueType：{0},pathItemNames：{1}", childItem.ValueType, string.Join(",", childPathItemNames.ToArray()));
                        return defVal;
                    }
                    object itemValue = GetTypeConfigItemValue(childItem, dicItemTypeOfValue, dicItemTypeOfValue == typeof(string) ? null : Activator.CreateInstance(dicItemTypeOfValue), childPathItemNames);
                    dic.Add(childItem.Name, itemValue);
                }
                return dic;
            }
            else
            {
                FieldInfo[] fis = valType.GetFields();
                foreach (FieldInfo fi in fis)
                {
                    if (valType == fi.FieldType)
                        continue;
                    List<string> childPathItemNames = new List<string>(pathItemNames) { fi.Name };
                    ConfigItem childItem = InternalGetConfigItem(configItem.AppName, childPathItemNames.ToArray());
                    if (childItem == null)
                    {
                        ConfigItemAttribute configItemAttribute = fi.GetCustomAttributes(false).OfType<ConfigItemAttribute>().SingleOrDefault();
                        if (configItemAttribute == null)
                            configItemAttribute = new ConfigItemAttribute();
                        string friendlyName = configItemAttribute.FriendlyName ?? fi.Name;
                        string desc = configItemAttribute.Description;
                        string value = null;
                        if (ConfigHelper.IsUnderlyingType(fi.FieldType))
                        {
                            if (fi.FieldType.IsEnum)
                                EnsureEnumTypeAdded(null, fi.FieldType);
                            value = fi.GetValue(defVal) != null ? (fi.GetValue(defVal)).ToString() : null;
                        }
                        childItem = InternalAddConfigItem(configItem.AppName, pathItemNames.ToArray(), fi.Name, friendlyName, desc, value, GetConfigItemSourceId(fi.FieldType), ConfigHelper.GetConfigItemValueType(fi.FieldType), ConfigHelper.GetConfigItemValueTypeEnum(fi.FieldType), ConfigHelper.IsCompositeValue(fi.FieldType));
                    }
                    object itemValue = GetTypeConfigItemValue(childItem, fi.FieldType, ConfigHelper.IsUnderlyingType(fi.FieldType) ? fi.GetValue(defVal) : fi.GetValue(defVal) ?? Activator.CreateInstance(fi.FieldType), childPathItemNames);
                    fi.SetValue(defVal, itemValue);
                }

                PropertyInfo[] pis = valType.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (valType == pi.PropertyType)
                        continue;
                    List<string> childPathItemNames = new List<string>(pathItemNames) { pi.Name };
                    ConfigItem childItem = InternalGetConfigItem(configItem.AppName, childPathItemNames.ToArray());
                    if (childItem == null)
                    {
                        ConfigItemAttribute configItemAttribute = pi.GetCustomAttributes(false).OfType<ConfigItemAttribute>().SingleOrDefault();
                        if (configItemAttribute == null)
                            configItemAttribute = new ConfigItemAttribute();
                        string friendlyName = configItemAttribute.FriendlyName ?? pi.Name;
                        string desc = configItemAttribute.Description;
                        string value = null;
                        if (ConfigHelper.IsUnderlyingType(pi.PropertyType))
                        {
                            if (pi.PropertyType.IsEnum)
                                EnsureEnumTypeAdded(null, pi.PropertyType);
                            value = pi.GetValue(defVal,null) != null ? (pi.GetValue(defVal,null)).ToString() : null;
                        }
                        childItem = InternalAddConfigItem(configItem.AppName, pathItemNames.ToArray(), pi.Name, friendlyName, desc, value, GetConfigItemSourceId(pi.PropertyType), ConfigHelper.GetConfigItemValueType(pi.PropertyType), ConfigHelper.GetConfigItemValueTypeEnum(pi.PropertyType), ConfigHelper.IsCompositeValue(pi.PropertyType));
                    }
                    if (pi.CanWrite)
                    {
                        object itemValue = GetTypeConfigItemValue(childItem, pi.PropertyType, ConfigHelper.IsUnderlyingType(pi.PropertyType) ? pi.GetValue(defVal, null) : pi.GetValue(defVal, null) ?? Activator.CreateInstance(pi.PropertyType), childPathItemNames);
                        pi.SetValue(defVal, itemValue, null);
                    }
                }
            }
            return defVal;
        }
        private ConfigItem InternalAddConfigItem(string appName, string[] parentPathItemNames, string name, string friendlyName, string desc, string val, string sourceId, string valType, string valTypeEnum, bool isCompositeValue)
        {
            ConfigItem configItem = null;
            try
            {
                configItem = RetryUtility.RetryAction(() => ConfigServer.AddConfigItem(appName, parentPathItemNames, name, friendlyName, desc, val, sourceId, valType, valTypeEnum, isCompositeValue), _actionNumRetries, _actionRetryTimeout, true, null);
            }
            catch (Exception ex)
            {
                LocalLoggingService.Warning("配置项添加失败！AppName:{0},PathItemNames:{1},{2},异常信息：{3}", appName, string.Join(",", parentPathItemNames.ToArray()), name, ex);
                throw;
            }
            UpdateConfigItemCache(configItem);
            return configItem;
        }
        private void BatchLoadConfigItems()
        {
            int loadedItemsCount = 0;
            LocalLoggingService.Info("开始批量加载配置项");
            try
            {
                ConfigItem[] configItems = null;
                int batchCount = 0;
                do
                {
                    configItems = ConfigServer.GetLastUpdatedConfigItems(_appName, ref _lastRowVersion, _batchLoadSize);
                    if (configItems != null && configItems.Length > 0)
                    {
                        foreach (ConfigItem configItem in configItems)
                        {
                            UpdateConfigItemCache(configItem);
                            loadedItemsCount++;
                        }
                        batchCount++;
                        LocalLoggingService.Info("加载第 {0} 批配置项结束", batchCount);
                    }
                }
                while (configItems != null && configItems.Length > 0);

            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("批量加载配置项错误！{0}", ex);
            }
            LocalLoggingService.Info("结束批量加载配置项，共加载 {0} 个配置项。", loadedItemsCount);
        }

        private void SyncConfigCacheWorker()
        {
            while (!_isStoped)
            {
                LocalLoggingService.Debug("开始配置项缓存同步");
                int syncedItemsCount = 0;
                try
                {
                    ConfigItem[] lastUpdatedConfigItems = ConfigServer.GetLastUpdatedConfigItems(_appName, ref _lastRowVersion, true);
                    if (lastUpdatedConfigItems != null && lastUpdatedConfigItems.Length > 0)
                    {
                        LocalLoggingService.Info(string.Format("获取 {0} 个变更的配置项", lastUpdatedConfigItems.Length));
                        foreach (ConfigItem updatedConfigItem in lastUpdatedConfigItems)
                        {
                            ConfigItemState configItemState = UpdateConfigItemCache(updatedConfigItem);
                            LocalLoggingService.Info("配置项缓存已同步，Id：{0},State：{1}", updatedConfigItem.Id, configItemState);
                            syncedItemsCount++;
                        }

                    }
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error("配置项缓存同步错误。 {0}", ex);
                }
                LocalLoggingService.Debug("结束配置项缓存同步，共同步 {0} 个配置项，本次同步版本号：{1},休眠 {2} ms。", syncedItemsCount, _lastRowVersion == null ? "NULL" : BitConverter.ToString(_lastRowVersion), _syncConfigCacheInterval);
                Thread.Sleep(_syncConfigCacheInterval);
            }

        }
        private ConfigItemState UpdateConfigItemCache(ConfigItem updatedConfigItem)
        {
            if (updatedConfigItem == null)
                return ConfigItemState.Invalid;
            ConfigItemState configItemState;
            ConfigItem parentConfigItem = null;
            ConfigItem childConfigItem = null;
            string parentId;
            ConfigItem originalConfigItem;
            ConfigItemValueUpdateCallback configItemValueUpdateCallback;
            _locker.EnterUpgradeableReadLock();
            try
            {
                _configCache.TryGetValue(updatedConfigItem.Id, out originalConfigItem);
                if (originalConfigItem == null)
                {
                    configItemState = updatedConfigItem.IsDeleted ? ConfigItemState.Invalid : ConfigItemState.Added;
                }
                else if (BitConverter.ToUInt64(originalConfigItem.RowVersion.Reverse().ToArray(), 0) < BitConverter.ToUInt64(updatedConfigItem.RowVersion.Reverse().ToArray(), 0))
                {
                    configItemState = updatedConfigItem.IsDeleted ? ConfigItemState.Deleted : ConfigItemState.Modified;
                }
                else
                    configItemState = ConfigItemState.Unchanged;

                _locker.EnterWriteLock();
                try
                {
                    switch (configItemState)
                    {
                        case ConfigItemState.Added:
                            _configCache.Add(updatedConfigItem.Id, updatedConfigItem);
                            if (_dicConfigItemValueUpdateCallback.TryGetValue(updatedConfigItem.Id, out configItemValueUpdateCallback))
                            {
                                configItemValueUpdateCallback(new ConfigItemValueUpdateArguments(updatedConfigItem.Id));
                            }
                            parentId = updatedConfigItem.ParentId;
                            if (parentId != null && !_configCache.TryGetValue(parentId, out parentConfigItem))
                            {
                                parentConfigItem = ConfigServer.GetConfigItem(parentId);
                                if (parentConfigItem != null)
                                    _configCache.Add(parentId, parentConfigItem);
                            }
                            if (updatedConfigItem.ParentId != null && parentConfigItem != null)
                            {
                                parentConfigItem.ChildItems.Add(updatedConfigItem);
                                while (parentConfigItem != null && parentConfigItem.ValueTypeEnum != null && parentConfigItem.ValueTypeEnum != ValueTypeEnum.Underlying.ToString())
                                {
                                    parentConfigItem.ObjectValue = null;
                                    if (_dicConfigItemValueUpdateCallback.TryGetValue(parentConfigItem.Id, out configItemValueUpdateCallback))
                                    {
                                        configItemValueUpdateCallback(new ConfigItemValueUpdateArguments(parentConfigItem.Id));
                                    }
                                    if (parentConfigItem.ParentId == null)
                                        break;
                                    childConfigItem = parentConfigItem;
                                    parentId = parentConfigItem.ParentId;
                                    if (!_configCache.TryGetValue(parentId, out parentConfigItem))
                                    {
                                        parentConfigItem = ConfigServer.GetConfigItem(parentId);
                                        if (parentConfigItem != null)
                                            _configCache.Add(parentId, parentConfigItem);
                                    }
                                    if (parentConfigItem != null && !parentConfigItem.ChildItems.Contains(childConfigItem))
                                        parentConfigItem.ChildItems.Add(childConfigItem);
                                }
                            }
                            break;
                        case ConfigItemState.Modified:
                            _configCache[updatedConfigItem.Id] = updatedConfigItem;
                            if (_dicConfigItemValueUpdateCallback.TryGetValue(updatedConfigItem.Id, out configItemValueUpdateCallback))
                            {
                                configItemValueUpdateCallback(new ConfigItemValueUpdateArguments(updatedConfigItem.Id));
                            }
                            parentId = updatedConfigItem.ParentId;
                            if (parentId != null && !_configCache.TryGetValue(parentId, out parentConfigItem))
                            {
                                parentConfigItem = ConfigServer.GetConfigItem(parentId);
                                if (parentConfigItem != null)
                                    _configCache.Add(parentId, parentConfigItem);
                            }
                            if (updatedConfigItem.ParentId != null && parentConfigItem != null)
                            {
                                int index = parentConfigItem.ChildItems.IndexOf(updatedConfigItem);
                                if (index != -1)
                                {
                                    parentConfigItem.ChildItems[index] = updatedConfigItem;
                                }
                                while (parentConfigItem != null && parentConfigItem.ValueTypeEnum != null && parentConfigItem.ValueTypeEnum != ValueTypeEnum.Underlying.ToString())
                                {
                                    parentConfigItem.ObjectValue = null;
                                    if (_dicConfigItemValueUpdateCallback.TryGetValue(parentConfigItem.Id, out configItemValueUpdateCallback))
                                    {
                                        configItemValueUpdateCallback(new ConfigItemValueUpdateArguments(parentConfigItem.Id));
                                    }
                                    if (parentConfigItem.ParentId == null)
                                        break;
                                    parentId = parentConfigItem.ParentId;
                                    if (!_configCache.TryGetValue(parentId, out parentConfigItem))
                                    {
                                        parentConfigItem = ConfigServer.GetConfigItem(parentId);
                                        if (parentConfigItem != null)
                                            _configCache.Add(parentId, parentConfigItem);
                                    }
                                }
                            }
                            break;
                        case ConfigItemState.Deleted:
                            _configCache.Remove(updatedConfigItem.Id);
                            if (_dicConfigItemValueUpdateCallback.TryGetValue(updatedConfigItem.Id, out configItemValueUpdateCallback))
                            {
                                configItemValueUpdateCallback(new ConfigItemValueUpdateArguments(updatedConfigItem.Id));
                            }
                            parentId = updatedConfigItem.ParentId;
                            if (parentId != null && !_configCache.TryGetValue(parentId, out parentConfigItem))
                            {
                                parentConfigItem = ConfigServer.GetConfigItem(parentId);
                                if (parentConfigItem != null)
                                    _configCache.Add(parentId, parentConfigItem);
                            }
                            if (updatedConfigItem.ParentId != null && parentConfigItem != null)
                            {
                                parentConfigItem.ChildItems.Remove(updatedConfigItem);
                                while (parentConfigItem != null && parentConfigItem.ValueTypeEnum != null && parentConfigItem.ValueTypeEnum != ValueTypeEnum.Underlying.ToString())
                                {
                                    parentConfigItem.ObjectValue = null;
                                    if (_dicConfigItemValueUpdateCallback.TryGetValue(parentConfigItem.Id, out configItemValueUpdateCallback))
                                    {
                                        configItemValueUpdateCallback(new ConfigItemValueUpdateArguments(parentConfigItem.Id));
                                    }
                                    if (parentConfigItem.ParentId == null)
                                        break;
                                    childConfigItem = parentConfigItem;
                                    parentId = parentConfigItem.ParentId;
                                    if (!_configCache.TryGetValue(parentId, out parentConfigItem))
                                    {
                                        parentConfigItem = ConfigServer.GetConfigItem(parentId);
                                        if (parentConfigItem != null)
                                            _configCache.Add(parentId, parentConfigItem);
                                    }
                                    if (parentConfigItem != null && !parentConfigItem.ChildItems.Contains(childConfigItem))
                                        parentConfigItem.ChildItems.Add(childConfigItem);
                                }
                            }
                            break;
                    }
                }
                finally
                {
                    _locker.ExitWriteLock();
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
            return configItemState;
        }

        private ConfigItem EnsureAppNameListItemAdded()
        {
            ConfigItem item = ConfigServer.GetConfigItem((string)null, Constants.AppNameListItemName);
            if (item != null)
                return item;
            return InternalAddConfigItem(null, null, Constants.AppNameListItemName, Constants.AppNameListItemFriendlyName, null, null, null, null, null, false);
        }

        private void RegisterApplication(string appName)
        {
            ConfigItem item = ConfigServer.GetConfigItem(null, Constants.AppNameListItemName, appName);
            if (item == null)
            {
                ConfigItem parentItem = EnsureAppNameListItemAdded();
                if (parentItem == null)
                {
                    string errMsg = "注册应用程序失败！";
                    LocalLoggingService.Error(errMsg);
                }
                InternalAddConfigItem(null, new string[] { Constants.AppNameListItemName }, appName, appName, null, null, null, ConfigHelper.GetConfigItemValueType(typeof(string)), ConfigHelper.GetConfigItemValueTypeEnum(typeof(string)), false);
            }
        }
        private ConfigItem EnsureTypeRepositoryItemAdded()
        {
            ConfigItem item = ConfigServer.GetConfigItem((string)null, Constants.TypeRepositoryItemName);
            if (item != null)
                return item;
            return InternalAddConfigItem(null, null, Constants.TypeRepositoryItemName, Constants.TypeRepositoryItemName, null, null, null, null, null, false);
        }
        private void EnsureUnderlyingTypeItemAdded()
        {
            Type[] underlyingTypes = { typeof(int), 
                                       typeof(Int64), 
                                       typeof(float), 
                                       typeof(double), 
                                       typeof(decimal), 
                                       typeof(DateTime), 
                                       typeof(TimeSpan), 
                                       typeof(TimeSpanEx),
                                       typeof(string)};
            foreach (Type type in underlyingTypes)
            {
                InternalAddConfigItem(null, new[] { Constants.TypeRepositoryItemName }, ConfigHelper.GetConfigItemValueType(type), ConfigHelper.GetConfigItemValueType(type), null, null, null, ConfigHelper.GetConfigItemValueType(type), ConfigHelper.GetConfigItemValueTypeEnum(type), false);
            }
        }

        private ConfigItem EnsureBooleanTypeItemAdded()
        {
            ConfigItem item = ConfigServer.GetConfigItem((string)null, Constants.TypeRepositoryItemName, BooleanTypeItemName);
            if (item != null)
                return item;
            item = InternalAddConfigItem(null, new[] { Constants.TypeRepositoryItemName }, BooleanTypeItemName, BooleanTypeItemName, null, null, null, null, null, false);
            if (item == null)
                return null;
            InternalAddConfigItem(null, new[] { Constants.TypeRepositoryItemName, BooleanTypeItemName }, "True", "True", null, "True", null, ConfigHelper.GetConfigItemValueType(typeof(bool)), ConfigHelper.GetConfigItemValueTypeEnum(typeof(bool)), false);
            InternalAddConfigItem(null, new[] { Constants.TypeRepositoryItemName, BooleanTypeItemName }, "False", "False", null, "False", null, ConfigHelper.GetConfigItemValueType(typeof(bool)), ConfigHelper.GetConfigItemValueTypeEnum(typeof(bool)), false);
            return item;
        }
        private string GetConfigItemSourceId(Type valType)
        {
            if (ConfigHelper.IsUnderlyingType(valType))
                return ConfigHelper.GenerateConfigItemId(null, Constants.TypeRepositoryItemName, ConfigHelper.GetConfigItemValueType(valType));
            return null;
        }
        private void SyncConfigCacheIntervalUpdateCallback(ConfigItemValueUpdateArguments arguments)
        {
            _syncConfigCacheInterval = GetConfigItemValue(true, this.GetType().FullName, "SyncConfigCacheInterval", DefaultSyncConfigCacheInterval);
        }
        private void ActionNumRetriesUpdateCallback(ConfigItemValueUpdateArguments arguments)
        {
            _actionNumRetries = GetConfigItemValue(true, this.GetType().FullName, "ActionNumRetries", DefaultActionNumRetries);
        }
        private void ActionRetryTimeoutUpdateCallback(ConfigItemValueUpdateArguments arguments)
        {
            _actionRetryTimeout = GetConfigItemValue(true, this.GetType().FullName, "ActionRetryTimeout", DefaultActionRetryTimeout);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void InternalDispose()
        {
            LocalLoggingService.Info("清理配置服务");
            _configCache.Clear();
            _isStoped = true;
            _syncConfigCacheThread.Join();
            _syncConfigCacheThread.Abort();

        }

    }
}