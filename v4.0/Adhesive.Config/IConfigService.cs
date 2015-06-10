

namespace Adhesive.Config
{
    public interface IConfigService
    {
        T GetConfigItemValue<T>(string cateName, T defVal);
        T GetConfigItemValue<T>(string cateName, T defVal, ConfigItemValueUpdateCallback callback);
        T GetConfigItemValue<T>(string cateName, string itemName, T defVal);
        T GetConfigItemValue<T>(string cateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback);
        T GetConfigItemValue<T>(string cateName, string subcateName, string itemName, T defVal);
        T GetConfigItemValue<T>(string cateName, string subcateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback);
        T GetConfigItemValue<T>(string[] pathItemNames, T defVal);
        T GetConfigItemValue<T>(string[] pathItemNames, T defVal, ConfigItemValueUpdateCallback callback);
        T GetConfigItemValue<T>(bool global, string cateName, T defVal);
        T GetConfigItemValue<T>(bool global, string cateName, T defVal, ConfigItemValueUpdateCallback callback);
        T GetConfigItemValue<T>(bool global, string cateName, string itemName, T defVal);
        T GetConfigItemValue<T>(bool global, string cateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback);
        T GetConfigItemValue<T>(bool global, string cateName, string subcateName, string itemName, T defVal);
        T GetConfigItemValue<T>(bool global, string cateName, string subcateName, string itemName, T defVal, ConfigItemValueUpdateCallback callback);
        T GetConfigItemValue<T>(bool global, string[] pathItemNames, T defVal);
        T GetConfigItemValue<T>(bool global, string[] pathItemNames, T defVal, ConfigItemValueUpdateCallback callback);
    }
}
