

using System;
using System.ServiceModel;

namespace Adhesive.Config
{
    [ServiceContract(Namespace = "Adhesive.Config.Server.ConfigServer")]
    public interface IConfigServer
    {
        [OperationContract(Name = "GetConfigItem1")]
        ConfigItem GetConfigItem(string appName, string cateName);
        [OperationContract(Name = "GetConfigItem2")]
        ConfigItem GetConfigItem(string appName, string cateName, string itemName);
        [OperationContract(Name = "GetConfigItem3")]
        ConfigItem GetConfigItem(string appName, string cateName, string subcateName, string itemName);
        [OperationContract(Name = "GetConfigItemValue1")]
        object GetConfigItemValue(string appName, string cateName, Type valType, object defVal);
        [OperationContract(Name = "GetConfigItemValue2")]
        object GetConfigItemValue(string appName, string cateName, string itemName, Type valType, object defVal);
        [OperationContract(Name = "GetConfigItemValue3")]
        object GetConfigItemValue(string appName, string cateName, string subcateName, string itemName, Type valType, object defVal);
        T GetConfigItemValue<T>(string appName, string cateName, T defVal);
        T GetConfigItemValue<T>(string appName, string cateName, string itemName, T defVal);
        T GetConfigItemValue<T>(string appName, string cateName, string subcateName, string itemName, T defVal);

        [OperationContract(Name = "GetConfigItem4")]
        ConfigItem GetConfigItem(string id);
        [OperationContract(Name = "GetConfigItem5")]
        ConfigItem GetConfigItem(string appName, params string[] pathItemNames);
        [OperationContract(Name = "GetChildConfigItems1")]
        ConfigItem[] GetChildConfigItems(string appName, params string[] parentPathItemNames);
        [OperationContract(Name = "GetChildConfigItems2")]
        ConfigItem[] GetChildConfigItems(string parentId);
        [OperationContract]
        ConfigItem[] GetTopLevelConfigItems(string appName);
        [OperationContract]
        ConfigItem AddConfigItem(string appName, string[] parentPathItemNames, string name, string friendlyName, string desc, string val, string sourceId, string valType, string valTypeEnum,bool isCompositeValue);
        [OperationContract]
        void SaveConfigItem(string id, string friendlyName, string desc, string val);
        [OperationContract]
        void RemoveConfigItem(string id);
        [OperationContract]
        ConfigItem[] GetLastUpdatedConfigItems(string appName, ref byte[] lastRowVersion, bool includeDeletedItems);
        [OperationContract]
        bool IsConfigItemExists(string appName, params string[] pathItemNames);
        [OperationContract]
        void SetItemsInited(string appName, params string[] pathItemNames);
        [OperationContract(Name = "GetLastUpdatedConfigItems2")]
        ConfigItem[] GetLastUpdatedConfigItems(string appName, ref byte[] lastRowVersion, int limit);
    }
}