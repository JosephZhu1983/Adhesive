
using Adhesive.Config;
namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "异常策略")]
    public class ExceptionStrategy
    {
        [ConfigItem(FriendlyName = "模块名", Description = "必须填写")]
        public string ModuleName { get; set; }

        [ConfigItem(FriendlyName = "异常类型名", Description = "如果要适用所有异常则写空")]
        public string ExceptionTypeName { get; set; }

        [ConfigItem(FriendlyName = "异常分类名", Description = "必须填写")]
        public string ExceptionInfoTypeName { get; set; }

        [ConfigItem(FriendlyName = "出异常后的重定向Url地址", Description = "只对WebSiteUnhandledExceptionInfo有效")]
        public string RedirectUrl { get; set; }

        [ConfigItem(FriendlyName = "出异常后响应状态码", Description = "只对WebSiteUnhandledExceptionInfo有效")]
        public int ResponseStatusCode { get; set; }

        [ConfigItem(FriendlyName = "是否清除异常")]
        public bool ClearException { get; set; }

        [ConfigItem(FriendlyName = "是否记录远程日志")]
        public bool RemoteLog { get; set; }

        [ConfigItem(FriendlyName = "是否记录本地日志")]
        public bool LocalLog { get; set; }

        public ExceptionStrategy()
        {
            ModuleName = AbstractInfoProvider.DefaultModuleName;
            ExceptionInfoTypeName = "";
            ExceptionTypeName = "";
            RemoteLog = true;
            LocalLog = true;
            ClearException = true;
            ResponseStatusCode = 200;
        }
    }
}
