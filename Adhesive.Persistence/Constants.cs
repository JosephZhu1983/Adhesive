
using System;
using System.IO;

namespace Adhesive.Persistence
{
    public class Constants
    {
        public const string StorageConfigFileName = "Storage.config";
        public static string StorageConfigFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("Config\\{0}", StorageConfigFileName));
        public const string DefaultContextName = "DefaultContext";
    }
}
