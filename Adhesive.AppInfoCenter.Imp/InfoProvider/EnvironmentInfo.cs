
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    public class EnvironmentInfo
    {
        [MongodbPersistenceItem(ColumnName = "PN")]
        [MongodbPresentationItem(DisplayName = "进程名")]
        public string ProcessName { get; set; }

        [MongodbPersistenceItem(ColumnName = "PI")]
        [MongodbPresentationItem(DisplayName = "进程Id")]
        public int? ProcessId { get; set; }

        [MongodbPersistenceItem(ColumnName = "DIR")]
        [MongodbPresentationItem(DisplayName = "当前目录")]
        public string CurrentDirectory { get; set; }

        [MongodbPersistenceItem(ColumnName = "MN")]
        [MongodbPresentationItem(DisplayName = "机器名")]
        public string MachineName { get; set; }

        [MongodbPersistenceItem(ColumnName = "OS")]
        [MongodbPresentationItem(DisplayName = "操作系统")]
        public string OperatingSystem { get; set; }

        [MongodbPersistenceItem(ColumnName = "UDN")]
        [MongodbPresentationItem(DisplayName = "当前用户的域")]
        public string UserDomainName { get; set; }

        [MongodbPersistenceItem(ColumnName = "UI")]
        [MongodbPresentationItem(DisplayName = "是否可交互")]
        public bool? UserInteractive { get; set; }

        [MongodbPersistenceItem(ColumnName = "UN")]
        [MongodbPresentationItem(DisplayName = "当前用户名")]
        public string UserName { get; set; }

        [MongodbPersistenceItem(ColumnName = "V")]
        [MongodbPresentationItem(DisplayName = "CLR版本")]
        public string Version { get; set; }
    }
}
