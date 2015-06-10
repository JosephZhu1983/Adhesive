
using Adhesive.Common;
using Adhesive.Config;
namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "日志策略")]
    public class LogStrategy
    {
        [ConfigItem(FriendlyName = "模块名", Description = "必须填写")]
        public string ModuleName { get; set; }

        [ConfigItem(FriendlyName = "日志级别", Description = "必须填写")]
        public LogLevel LogLevel { get; set; }

        [ConfigItem(FriendlyName = "是否记录远程日志")]
        public bool RemoteLog { get; set; }

        [ConfigItem(FriendlyName = "是否记录本地日志")]
        public bool LocalLog { get; set; }

        public LogStrategy()
        {
            ModuleName = AbstractInfoProvider.DefaultModuleName;
            LogLevel = LogLevel.None;
            RemoteLog = true;
            LocalLog = true;
        }
    }
}
