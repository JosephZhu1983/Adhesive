
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using Adhesive.Common;
using Adhesive.Common.FastReflection;
//using Adhesive.DistributedService;
using Adhesive.MemoryQueue;
using Adhesive.Mongodb.Server;

namespace Adhesive.Mongodb.Imp
{
    public class MongodbInsertService : AbstractService, IMongodbInsertService
    {
        private static Dictionary<string, IMemoryQueueService> submitDataMemoryQueueServices = new Dictionary<string, IMemoryQueueService>();
        private static Dictionary<string, MongodbDatabaseDescription> mongodbDatabaseDescriptionCache = new Dictionary<string, MongodbDatabaseDescription>();
        private static Dictionary<string, Dictionary<PropertyInfo, ProperyInfoConfig>> propertyConfigCache = new Dictionary<string, Dictionary<PropertyInfo, ProperyInfoConfig>>();
        private static Dictionary<string, List<PropertyInfo>> propertyInfoCache = new Dictionary<string, List<PropertyInfo>>();

        internal static void ConfigUpdateCallback()
        {
            lock (submitDataMemoryQueueServices)
            {
                //释放老的线程！
                foreach (var submitDataMemoryQueueService in submitDataMemoryQueueServices)
                {
                    submitDataMemoryQueueService.Value.Dispose();
                }
                submitDataMemoryQueueServices.Clear();
            }
        }

        //internal static List<BaseInfo> GetMemoeyQueueState()
        //{
        //    lock (submitDataMemoryQueueServices)
        //    {
        //        var state = new List<BaseInfo>() 
        //        {
        //            new MongodbServiceStateInfo
        //            {
        //                MemoryQueueServiceStates = submitDataMemoryQueueServices.ToDictionary(s=>s.Key, s=>s.Value.GetState()),
        //            }
        //        };
        //        return state;
        //    }
        //}

        public void Insert(object item)
        {
            if (!Enabled) return;

            var type = item.GetType();
            var typeFullName = type.FullName;

            try
            {
                var config = MongodbServiceConfiguration.GetMongodbInsertServiceConfigurationItem(typeFullName);
                if (config == null)
                {
                    //AppInfoCenterService.LoggingService.Warning(MongodbServiceConfiguration.ModuleName, "Insert", typeFullName,
                    //    string.Format("没取到类型名为 {0} 的配置信息！", typeFullName));
                    return;
                }

                if (!submitDataMemoryQueueServices.ContainsKey(typeFullName))
                {
                    lock (submitDataMemoryQueueServices)
                    {
                        if (!submitDataMemoryQueueServices.ContainsKey(typeFullName))
                        {
                            var memoryQueueService = LocalServiceLocator.GetService<IMemoryQueueService>();
                            memoryQueueService.Init(new MemoryQueueServiceConfiguration(string.Format("{0}_{1}", ServiceName, typeFullName), InternalSubmitData)
                            {
                                ConsumeErrorAction = config.ConsumeErrorAction,
                                ConsumeIntervalMilliseconds = config.ConsumeIntervalMilliseconds,
                                ConsumeIntervalWhenErrorMilliseconds = config.ConsumeIntervalWhenErrorMilliseconds,
                                ConsumeItemCountInOneBatch = config.ConsumeItemCountInOneBatch,
                                ConsumeThreadCount = config.ConsumeThreadCount,
                                MaxItemCount = config.MaxItemCount,
                                NotReachBatchCountConsumeAction = config.NotReachBatchCountConsumeAction,
                                ReachMaxItemCountAction = config.ReachMaxItemCountAction,
                            });
                            submitDataMemoryQueueServices.Add(typeFullName, memoryQueueService);
                        }
                    }
                }

                if (!mongodbDatabaseDescriptionCache.ContainsKey(typeFullName))
                {
                    lock (mongodbDatabaseDescriptionCache)
                    {
                        if (!mongodbDatabaseDescriptionCache.ContainsKey(typeFullName))
                        {
                            MongodbDatabaseDescription mongodbDatabaseDescription = GetMongodbDatabaseDescription(item);
                            CheckMongodbDatabaseDescription(mongodbDatabaseDescription);
                            mongodbDatabaseDescriptionCache.Add(typeFullName, mongodbDatabaseDescription);
                        }
                    }
                }

                if (config.SubmitToServer && submitDataMemoryQueueServices.ContainsKey(typeFullName))
                {
                    submitDataMemoryQueueServices[typeFullName].Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}",MongodbServiceConfiguration.ModuleName, "Insert", typeFullName, ex.Message);
                throw;
            }
        }

        protected override void InternalDispose()
        {
            foreach (var item in submitDataMemoryQueueServices)
                item.Value.Dispose();
            base.InternalDispose();
        }

        private void InternalSubmitData(IList<object> items)
        {
            if (items == null || items.Count == 0) return;

            var item = items.First();
            var type = item.GetType();
            var typeFullName = type.FullName;

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var mongodbDataList = items.Select(_ => ConvertItemToMongodbData(_)).Where(_ => _ != null).ToList();
                var desc = mongodbDatabaseDescriptionCache[typeFullName];
                MongodbServiceLocator.GetService().SubmitData(mongodbDataList, desc.SentToServer ? null : desc);
                //LocalLoggingService.Debug("AdhesiveFramework.MongodbInsertService 提交了 {0} 条数据到服务端，类型是 '{1}'，提交了描述信息？{2}，耗时：{3}毫秒", mongodbDataList.Count.ToString(), typeFullName, (!desc.SentToServer).ToString(), sw.ElapsedMilliseconds.ToString());
                desc.SentToServer = true;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "InternalSubmitData", typeFullName, ex.Message);
            }
        }

        private void CheckMongodbDatabaseDescription(MongodbDatabaseDescription mongodbDatabaseDescription)
        {
            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.IsTableColumn) > 1)
                throw new Exception("作为表名的列不能超过一个！");

            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.IsContextIdentityColumn) > 1)
                throw new Exception("作为上下文标识的列不能超过一个！");

            var tableColumn = mongodbDatabaseDescription.MongodbColumnDescriptionList.SingleOrDefault(column => column.IsTableColumn);
            if (tableColumn != null && tableColumn.MongodbIndexOption != MongodbIndexOption.None)
                throw new Exception(string.Format("作为表名的列 '{0}' 不能有索引！", tableColumn.Name));

            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.IsPrimaryKey) != 1)
                throw new Exception("必须有并且只有一个主键列！");

            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.IsTimeColumn) != 1)
                throw new Exception("必须有并且只有一个时间列！");

            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.MongodbSortOption != MongodbSortOption.None) > 1)
                throw new Exception("排序列不能超过两个！");

            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.MongodbCascadeFilterOption == MongodbCascadeFilterOption.LevelOne) > 1)
                throw new Exception("第一级的级联过滤列不能超过一个！");
            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.MongodbCascadeFilterOption == MongodbCascadeFilterOption.LevelTwo) > 1)
                throw new Exception("第二级的级联过滤列不能超过一个！");
            if (mongodbDatabaseDescription.MongodbColumnDescriptionList.Count(column => column.MongodbCascadeFilterOption == MongodbCascadeFilterOption.LevelThree) > 1)
                throw new Exception("第三级的级联过滤列不能超过一个！");

            var pkColumn = mongodbDatabaseDescription.MongodbColumnDescriptionList.Single(column => column.IsPrimaryKey);
            if (pkColumn.MongodbIndexOption != MongodbIndexOption.AscendingAndUnique && pkColumn.MongodbIndexOption != MongodbIndexOption.DescendingAndUnique)
                throw new Exception(string.Format("主键列' {0}' 必须具有唯一索引！", pkColumn.Name));

            var timeColumn = mongodbDatabaseDescription.MongodbColumnDescriptionList.Single(column => column.IsTimeColumn);
            if (timeColumn.MongodbIndexOption != MongodbIndexOption.Ascending && timeColumn.MongodbIndexOption != MongodbIndexOption.Descending)
                throw new Exception(string.Format("时间列 '{0}' 必须具有不唯一索引！", timeColumn.Name));

            if (timeColumn.TypeName != "DateTime")
                throw new Exception(string.Format("时间列 '{0}' 的类型必须是DateTime！", timeColumn));

            var sortColumn = mongodbDatabaseDescription.MongodbColumnDescriptionList.SingleOrDefault(column => column.MongodbSortOption != MongodbSortOption.None);
            if (sortColumn != null && sortColumn.MongodbIndexOption != MongodbIndexOption.Ascending && sortColumn.MongodbIndexOption != MongodbIndexOption.Descending)
                throw new Exception(string.Format("排序列 '{0}' 必须具有不唯一索引！", sortColumn.Name));

            var contextColumn = mongodbDatabaseDescription.MongodbColumnDescriptionList.SingleOrDefault(column => column.IsContextIdentityColumn);
            if (contextColumn != null && contextColumn.MongodbIndexOption != MongodbIndexOption.Ascending && contextColumn.MongodbIndexOption != MongodbIndexOption.Descending)
                throw new Exception(string.Format("上下文标识列 '{0}' 必须具有不唯一索引！", contextColumn.Name));

            var filterColumns = mongodbDatabaseDescription.MongodbColumnDescriptionList.Where(column => column.MongodbFilterOption != MongodbFilterOption.None ||
                column.MongodbCascadeFilterOption != MongodbCascadeFilterOption.None).ToList();
            filterColumns.Each(filterColumn =>
            {
                if (filterColumn.MongodbIndexOption == MongodbIndexOption.None)
                    throw new Exception(string.Format("作为搜索条件的列 '{0}' 必须具有索引！", filterColumn.Name));
            });

        }

        private MongodbData ConvertItemToMongodbData(object item)
        {
            var type = item.GetType();
            var typeFullName = type.FullName;

            try
            {
                var data = new MongodbData();
                data.TypeFullName = typeFullName;
                data.TableName = "General";

                if (!mongodbDatabaseDescriptionCache.ContainsKey(typeFullName))
                {
                    LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "ConvertItemToMongodbData", typeFullName,
                        string.Format("没取到类型为 '{0}' 的元数据！", typeFullName));
                    return null;
                }

                var mongodbDatabaseDescription = mongodbDatabaseDescriptionCache[typeFullName];
                data.DatabaseName = mongodbDatabaseDescription.DatabasePrefix;

                JavaScriptSerializer s = new JavaScriptSerializer();
                var rawData = GetMongodbData(data, type, item);
                var jsonData = s.Serialize(rawData);
                data.Data = jsonData;
                return data;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "ConvertItemToMongodbData", typeFullName, ex.Message);
                throw;
            }
        }

        private List<PropertyInfo> GetPropertyListFromCache(Type type)
        {
            var typeFullName = type.FullName;
            if (!propertyInfoCache.ContainsKey(typeFullName))
            {
                lock (propertyInfoCache)
                {
                    if (!propertyInfoCache.ContainsKey(typeFullName))
                        propertyInfoCache.Add(typeFullName, type.GetProperties().ToList());
                }
            }

            return propertyInfoCache[type.FullName];
        }

        private Dictionary<string, object> GetMongodbData(MongodbData data, Type type, object item)
        {
            var typeFullName = type.FullName;

            try
            {
                var returnDictionary = new Dictionary<string, object>();
                var properties = GetPropertyListFromCache(type);
                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        var configList = propertyConfigCache.ContainsKey(typeFullName) ? propertyConfigCache[typeFullName] : null;
                        if (configList == null) continue;

                        var config = configList.ContainsKey(property) ? configList[property] : null;
                        if (config == null) continue;

                        if (config.IsCascadeFilterLevelOne)
                        {
                            var value = (string)property.FastGetValue(item);
                            property.FastSetValue(item, value.Replace("__", "_"));
                        }
                        if (config.IsCascadeFilterLevelTwo)
                        {
                            var parentLevel = configList.Where(pc => pc.Value.IsCascadeFilterLevelOne).Select(pc => pc.Key).FirstOrDefault();
                            if (parentLevel != null)
                            {
                                var two = (string)property.FastGetValue(item);
                                if (two != null) two = two.Replace("__", "_");
                                property.FastSetValue(item, string.Format("{0}__{1}", parentLevel.FastGetValue(item), two));
                            }
                        }
                        else if (config.IsCascadeFilterLevelThree)
                        {
                            var parentLevel = configList.Where(pc => pc.Value.IsCascadeFilterLevelTwo).Select(pc => pc.Key).FirstOrDefault();
                            if (parentLevel != null)
                            {
                                var three = (string)property.FastGetValue(item);
                                if (three != null) three = three.Replace("__", "_");
                                property.FastSetValue(item, string.Format("{0}__{1}", parentLevel.FastGetValue(item), three));
                            }
                        }
                    }

                    foreach (var property in properties)
                    {
                        InternalGetMongodbData(data, typeFullName, item, returnDictionary, property);
                    }
                }

                return returnDictionary;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "GetMongodbData", typeFullName, ex.Message);
                throw;
            }
        }

        private void InternalGetMongodbData(MongodbData data, string typeFullName, object item, Dictionary<string, object> dic, PropertyInfo pi)
        {
            try
            {
                var val = pi.FastGetValue(item);
                if (val == null) return;

                var configList = propertyConfigCache.ContainsKey(data.TypeFullName) ? propertyConfigCache[data.TypeFullName] : null;
                var config = configList != null ? (configList.ContainsKey(pi) ? configList[pi] : null) : null;
                if (config != null && config.IsIgnore) return;

                var columnName = pi.Name;

                if (config != null && config.IsTableName && !string.IsNullOrEmpty(val.ToString()))
                    data.TableName = val.ToString();
                if (config != null && !string.IsNullOrEmpty(config.ColumnName))
                    columnName = config.ColumnName;
                if (config != null && config.IsDateColumn)
                    data.DatabaseName = string.Format("{0}__{1}", data.DatabaseName, ((DateTime)val).ToString("yyyyMM"));

                if (typeof(Delegate).IsAssignableFrom(pi.PropertyType)) return;

                if (val is IList)
                {
                    var listData = val as IList;

                    var subdic = new Dictionary<string, object>();
                    lock (listData)
                    {
                        for (int i = 0; i < listData.Count; i++)
                        {
                            var subitem = listData[i];
                            var subitemType = subitem.GetType();
                            object subData = null;
                            if (!subitemType.Assembly.GlobalAssemblyCache && subitemType.BaseType != typeof(Enum))
                            {
                                subData = GetMongodbData(data, subitemType, subitem);
                            }
                            else if (subitem != null)
                            {
                                subData = subitem.ToString();
                            }
                            if (subData != null)
                                subdic.Add(string.Format("{0}__{1}", subitemType.Name, i.ToString()), subData);
                        }
                        if (dic.ContainsKey(columnName))
                            LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "InternalGetMongodbData", typeFullName,
                               string.Format("遇到重复的列，类型：{0}，列名：{1}", typeFullName, columnName));
                        else
                            dic.Add(columnName, subdic);
                        return;
                    }
                }

                if (val is IDictionary)
                {
                    var dicData = val as IDictionary;
                    var subdic = new Dictionary<string, object>();
                    lock (dicData)
                    {
                        foreach (object key in dicData.Keys)
                        {
                            var subitem = dicData[key];
                            var subitemType = subitem.GetType();
                            object subData = null;
                            if (!subitemType.Assembly.GlobalAssemblyCache && subitemType.BaseType != typeof(Enum))
                            {
                                subData = GetMongodbData(data, subitemType, subitem);
                            }
                            else if (subitem != null)
                                subData = subitem.ToString();
                            if (subData != null)
                                subdic.Add(string.Format("{0}__{1}", subitemType.Name, key.ToString().Replace(".", "")), subData);
                        }
                        if (dic.ContainsKey(columnName))
                            LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "InternalGetMongodbData", typeFullName,
                                string.Format("遇到重复的列，类型：{0}，列名：{1}", typeFullName, columnName));
                        else
                            dic.Add(columnName, subdic);
                        return;
                    }
                }

                if (pi.PropertyType.Assembly.GlobalAssemblyCache || pi.PropertyType.BaseType == typeof(Enum))
                {
                    if (dic.ContainsKey(columnName))
                        LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "InternalGetMongodbData", typeFullName,
                           string.Format("遇到重复的列，类型：{0}，列名：{1}", typeFullName, columnName));
                    else
                        dic.Add(columnName, val);
                }
                else
                {
                    var subdic = new Dictionary<string, object>();
                    var properties = GetPropertyListFromCache(val.GetType());
                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            InternalGetMongodbData(data, typeFullName, val, subdic, property);
                        }
                        if (dic.ContainsKey(columnName))
                            LocalLoggingService.Warning("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "InternalGetMongodbData", typeFullName,
                           string.Format("遇到重复的列，类型：{0}，列名：{1}", typeFullName, columnName));
                        else
                            dic.Add(columnName, subdic);
                    }
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "InternalGetMongodbData", typeFullName, ex.Message);
                throw;
            }
        }

        private MongodbDatabaseDescription GetMongodbDatabaseDescription<T>(T item)
        {
            var type = item.GetType();
            var typeFullName = type.FullName;

            try
            {
                var persistenceEntityAttribute = type.GetCustomAttributes(typeof(MongodbPersistenceEntityAttribute), true).Cast<MongodbPersistenceEntityAttribute>().SingleOrDefault();
                if (persistenceEntityAttribute == null)
                    throw new Exception(string.Format("请在类型 '{0}' 上定义MongodbPersistenceEntityAttribute!", typeFullName));

                var mongodbDatabaseDescription = new MongodbDatabaseDescription();
                var entityName = persistenceEntityAttribute.Name ?? type.Name;
                var databasePrefix = string.Format("{0}__{1}", persistenceEntityAttribute.CategoryName, entityName);

                mongodbDatabaseDescription.DatabasePrefix = databasePrefix;
                mongodbDatabaseDescription.TypeFullName = typeFullName;
                mongodbDatabaseDescription.CategoryName = persistenceEntityAttribute.CategoryName;
                mongodbDatabaseDescription.Name = entityName;
                mongodbDatabaseDescription.DisplayName = persistenceEntityAttribute.DisplayName ?? entityName;
                mongodbDatabaseDescription.ExpireDays = persistenceEntityAttribute.ExpireDays;

                var mongodbColumnDescriptionList = new List<MongodbColumnDescription>();
                var mongodbEnumColumnDescriptionList = new List<MongodbEnumColumnDescription>();
                foreach (var property in GetPropertyListFromCache(type))
                {
                    GetMongodbColumnDescription(typeFullName, string.Empty, mongodbColumnDescriptionList, mongodbEnumColumnDescriptionList, property);
                }
                mongodbDatabaseDescription.MongodbColumnDescriptionList = mongodbColumnDescriptionList;
                mongodbDatabaseDescription.MongodbEnumColumnDescriptionList = mongodbEnumColumnDescriptionList;
                return mongodbDatabaseDescription;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "GetMongodbDatabaseDescription", typeFullName, ex.Message);
                throw;
            }
        }

        private void GetMongodbColumnDescription(string typeFullName, string levelPrefix, List<MongodbColumnDescription> columnDescriptionList, List<MongodbEnumColumnDescription> enumColumnDescriptionList, PropertyInfo pi)
        {
            try
            {
                var presentationAttribute = pi.GetCustomAttributes(typeof(MongodbPresentationItemAttribute), false).Cast<MongodbPresentationItemAttribute>().SingleOrDefault();
                if (presentationAttribute == null)
                    presentationAttribute = new MongodbPresentationItemAttribute();

                var persistenceAttribute = pi.GetCustomAttributes(typeof(MongodbPersistenceItemAttribute), false).Cast<MongodbPersistenceItemAttribute>().SingleOrDefault();
                if (persistenceAttribute == null)
                    persistenceAttribute = new MongodbPersistenceItemAttribute();

                var columnName = persistenceAttribute.ColumnName ?? pi.Name;

                var config = new ProperyInfoConfig
                {
                    IsTableName = false,
                    IsCascadeFilterLevelOne = false,
                    IsCascadeFilterLevelTwo = false,
                    IsCascadeFilterLevelThree = false,
                    IsIgnore = false,
                };

                if (!propertyConfigCache.ContainsKey(typeFullName))
                    propertyConfigCache.Add(typeFullName, new Dictionary<PropertyInfo, ProperyInfoConfig>());

                bool needConfig = false;

                var configList = propertyConfigCache[typeFullName];
                if (configList.ContainsKey(pi))
                {
                    config = configList[pi];
                }

                if (!string.IsNullOrEmpty(persistenceAttribute.ColumnName))
                {
                    config.ColumnName = columnName;
                    needConfig = true;
                }

                var fullName = string.IsNullOrEmpty(levelPrefix) ? columnName : levelPrefix + "." + columnName;
                var displayName = presentationAttribute.DisplayName ?? columnName;
                var description = presentationAttribute.Description;

                var type = pi.PropertyType;
                var columnDescription = new MongodbColumnDescription();
                columnDescription.Description = description;
                columnDescription.ColumnName = fullName;
                columnDescription.TypeName = type.Name;
                columnDescription.Name = pi.Name;
                columnDescription.MongodbIndexOption = persistenceAttribute.MongodbIndexOption;
                columnDescription.DisplayName = displayName.Replace('.', '_');
                columnDescription.MongodbFilterOption = presentationAttribute.MongodbFilterOption;
                columnDescription.MongodbSortOption = presentationAttribute.MongodbSortOption;
                columnDescription.MongodbCascadeFilterOption = presentationAttribute.MongodbCascadeFilterOption;
                columnDescription.ShowInTableView = presentationAttribute.ShowInTableView;
                columnDescription.IsTableColumn = persistenceAttribute.IsTableName;
                columnDescription.IsPrimaryKey = persistenceAttribute.IsPrimaryKey;
                columnDescription.IsTimeColumn = persistenceAttribute.IsTimeColumn;
                columnDescription.IsContextIdentityColumn = persistenceAttribute.IsContextIdentityColumn;
                columnDescriptionList.Add(columnDescription);

                if (typeof(Delegate).IsAssignableFrom(type)) return;

                if (typeof(IList).IsAssignableFrom(type))
                {
                    if (type.IsGenericType)
                    {
                        columnDescription.IsArrayColumn = true;
                        var argTypes = type.GetGenericArguments();
                        if (argTypes.Length == 1 && !argTypes.First().Assembly.GlobalAssemblyCache && argTypes.First().BaseType != typeof(Enum))
                        {
                            type = argTypes.First();
                            fullName += "." + type.Name;
                        }
                    }
                    else if (type.BaseType.IsGenericType)
                    {
                        columnDescription.IsArrayColumn = true;
                        var argTypes = type.BaseType.GetGenericArguments();
                        if (argTypes.Length == 1 && !argTypes.First().Assembly.GlobalAssemblyCache && argTypes.First().BaseType != typeof(Enum))
                        {
                            type = argTypes.First();
                            fullName += "." + type.Name;
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("对于列表列 '{0}' 只支持泛型的List<T>！", fullName));
                    }
                }
                else if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    if (type.IsGenericType)
                    {
                        columnDescription.IsArrayColumn = true;
                        var argTypes = type.GetGenericArguments();
                        if (argTypes.Length == 2 && !argTypes.Last().Assembly.GlobalAssemblyCache && argTypes.Last().BaseType != typeof(Enum))
                        {
                            type = argTypes.Last();
                            fullName += "." + type.Name;
                        }
                    }
                    else if (type.BaseType.IsGenericType)
                    {
                        columnDescription.IsArrayColumn = true;
                        var argTypes = type.BaseType.GetGenericArguments();
                        if (argTypes.Length == 2 && !argTypes.Last().Assembly.GlobalAssemblyCache && argTypes.Last().BaseType != typeof(Enum))
                        {
                            type = argTypes.Last();
                            fullName += "." + type.Name;
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("对于字典列 '{0}' 只支持泛型的Dictionary<TKey, TValue>！", fullName));
                    }
                }
                else if (type.Assembly.GlobalAssemblyCache || type.BaseType == typeof(Enum))
                {
                    if (persistenceAttribute.IsTableName)
                    {
                        config.IsTableName = true;
                        needConfig = true;
                    }
                    if (persistenceAttribute.IsIgnore)
                    {
                        config.IsIgnore = true;
                        needConfig = true;
                    }

                    if (persistenceAttribute.IsTimeColumn)
                    {
                        config.IsDateColumn = true;
                        needConfig = true;
                    }

                    if (presentationAttribute.MongodbCascadeFilterOption != MongodbCascadeFilterOption.None)
                    {
                        switch (presentationAttribute.MongodbCascadeFilterOption)
                        {
                            case MongodbCascadeFilterOption.LevelOne:
                                config.IsCascadeFilterLevelOne = true;
                                break;
                            case MongodbCascadeFilterOption.LevelTwo:
                                config.IsCascadeFilterLevelTwo = true;
                                break;
                            case MongodbCascadeFilterOption.LevelThree:
                                config.IsCascadeFilterLevelThree = true;
                                break;
                        }
                        needConfig = true;
                    }
                }

                if (needConfig)
                {
                    if (configList.ContainsKey(pi))
                    {
                        configList[pi] = config;
                    }
                    else
                    {
                        configList.Add(pi, config);
                    }
                }

                if (type.BaseType == typeof(Enum) && !enumColumnDescriptionList.Exists(c => c.Name == fullName))
                {
                    var enumDescription = new MongodbEnumColumnDescription();
                    enumDescription.Name = fullName;
                    enumDescription.EnumItems = new Dictionary<string, string>();
                    Enum.GetNames(type).ToList().ForEach(name =>
                    {
                        enumDescription.EnumItems.Add(((int)Enum.Parse(type, name)).ToString(), name);
                    });
                    enumColumnDescriptionList.Add(enumDescription);
                }

                if (!type.Assembly.GlobalAssemblyCache && type != pi.DeclaringType
                    && type.BaseType != typeof(Enum))
                {
                    columnDescription.IsEntityColumn = true;
                    var properties = GetPropertyListFromCache(type);
                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            GetMongodbColumnDescription(typeFullName, fullName, columnDescriptionList, enumColumnDescriptionList, property);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServiceConfiguration.ModuleName, "GetMongodbColumnDescription", typeFullName, ex.Message);
                throw;
            }
        }
    }
}
