
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Adhesive.Common
{
    public class LocalConfigService
    {
        private const string UnderlyingConfigFileName = "Configuration.config";
        private static readonly string LocalConfigDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
        private static readonly Dictionary<string, object> ConfigCache = new Dictionary<string, object>();
        private static readonly Dictionary<string, ConfigFileWatcher> ConfigFileWatcherCache = new Dictionary<string, ConfigFileWatcher>();
        private static readonly object Locker = new object();

        public static event ConfigFileChangedEventHandler ConfigFileChanged;

        public static T GetConfig<T>(T defVal)
        {
            string fileName = string.Format("{0}.config", typeof(T).Name);
            return GetConfig(fileName, defVal);
        }
        public static T GetConfig<T>(string fileName, T defVal)
        {
            object instance = null;
            string fileFullName = GetConfigFileFullName(fileName);
            if (ConfigCache.TryGetValue(fileFullName, out instance))
                return (T)instance;
            lock (Locker)
            {
                if (ConfigCache.TryGetValue(fileFullName, out instance))
                    return (T)instance;
                if (!File.Exists(fileFullName))
                {
                    TryCreateConfig(fileName, defVal);
                    return defVal;
                }
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(fileFullName);
                }
                catch (Exception ex)
                {
                    string errMsg = string.Format("加载配置文件 {0} 失败！异常信息：{1}", fileFullName, ex);
                    LocalLoggingService.Error(errMsg);
                    return defVal;
                }
                ConfigFileWatcher configFileWatcher = null;
                if (!ConfigFileWatcherCache.TryGetValue(fileFullName, out configFileWatcher))
                    ConfigFileWatcherCache.Add(fileFullName, new ConfigFileWatcher(fileFullName, OnConfigChanged));
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringReader sr = new StringReader(doc.OuterXml))
                {
                    try
                    {
                        instance = (T)xmlSerializer.Deserialize(sr);
                        ConfigCache.Add(fileFullName, instance);
                        return (T)instance;
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Debug("反序列化异常，类型名称：{0}，异常信息：{1}", typeof(T).Name, ex.ToString());
                        return defVal;
                    }
                }
            }
        }
        private static void TryCreateConfig<T>(string fileName, T defVal)
        {
            if (!EnsureConfigDirectoryExists())
                return;
            string fileFullName = GetConfigFileFullName(fileName);
            if (File.Exists(fileFullName))
                return;
            if (defVal == null)
                return;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            XmlSerializer xs = new XmlSerializer(typeof(T), (string)null);
            using (FileStream fs = new FileStream(fileFullName, FileMode.Create))
            {
                using (XmlWriter xw = XmlWriter.Create(fs, settings))
                {
                    try
                    {
                        xs.Serialize(xw, defVal);
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("序列化异常，类型名称：{0}，异常信息：{1}", typeof(T).Name, ex.ToString());
                    }
                }
            }
        }
        private static bool EnsureConfigDirectoryExists()
        {
            if (Directory.Exists(LocalConfigDirectory))
                return true;
            try
            {
                Directory.CreateDirectory(LocalConfigDirectory);
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("创建目录 {0} 失败！异常信息：{1}", LocalConfigDirectory, ex);
                LocalLoggingService.Error(errMsg);
                return false;
            }
        }
        private static string GetConfigFileFullName(string fielName)
        {
            return Path.Combine(LocalConfigDirectory, fielName);
        }
        private static void OnConfigChanged(object configFile)
        {
            if (ConfigCache.ContainsKey(configFile.ToString()))
            {
                lock (Locker)
                {
                    if (ConfigCache.ContainsKey(configFile.ToString()))
                    {
                        ConfigCache.Remove(configFile.ToString());
                        if (ConfigFileChanged != null)
                            ConfigFileChanged(configFile);
                    }
                }
            }
        }
    }
}
