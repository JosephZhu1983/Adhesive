
using Adhesive.Config;
namespace Adhesive.AppInfoCenter.Imp
{
    [ConfigEntity(FriendlyName = "包含信息策略_环境信息")]
    public class IncludeInfoStrategyForEnvironmentInfo
    {
        [ConfigItem(FriendlyName = "是否包含环境信息")]
        public bool Include { get; set; }
    }

    [ConfigEntity(FriendlyName = "包含信息策略_位置信息")]
    public class IncludeInfoStrategyForLocationInfo
    {
        [ConfigItem(FriendlyName = "是否包含位置信息")]
        public bool Include { get; set; }

        [ConfigItem(FriendlyName = "是否包含堆栈信息")]
        public bool IncludeStackTrace { get; set; }
    }

    [ConfigEntity(FriendlyName = "包含信息策略_MVC上下文信息")]
    public class IncludeInfoStrategyForMvcContextInfo
    {
        [ConfigItem(FriendlyName = "是否包含MVC上下文信息")]
        public bool Include { get; set; }

        [ConfigItem(FriendlyName = "是否包含路由数据")]
        public bool IncludeRouteData { get; set; }

        [ConfigItem(FriendlyName = "是否包含临时数据")]
        public bool IncludeTempData { get; set; }

        [ConfigItem(FriendlyName = "是否包含视图数据")]
        public bool IncludeViewData { get; set; }

        [ConfigItem(FriendlyName = "是否参数数据")]
        public bool IncludeParameterData { get; set; }
    }

    [ConfigEntity(FriendlyName = "包含信息策略_Http上下文信息")]
    public class IncludeInfoStrategyForHttpContextInfo
    {
        [ConfigItem(FriendlyName = "是否包含Http上下文信息")]
        public bool Include { get; set; }

        [ConfigItem(FriendlyName = "是否包含HttpContext的Items")]
        public bool IncludeHttpContextItems { get; set; }

        [ConfigItem(FriendlyName = "是否包含HttpContext的会话")]
        public bool IncludeHttpContextSessions { get; set; }

        [ConfigItem(FriendlyName = "Http请求的配置")]
        public IncludeInfoStrategyForRequestInfo IncludeInfoStrategyForRequestInfo { get; set; }

        [ConfigItem(FriendlyName = "Http响应的配置")]
        public IncludeInfoStrategyForResponseInfo IncludeInfoStrategyForResponseInfo { get; set; }
    }

    [ConfigEntity(FriendlyName = "包含信息策略_Http请求信息")]
    public class IncludeInfoStrategyForRequestInfo
    {
        [ConfigItem(FriendlyName = "是否包含请求信息")]
        public bool Include { get; set; }

        [ConfigItem(FriendlyName = "是否包含基本请求信息")]
        public bool IncludeBasicInfo { get; set; }

        [ConfigItem(FriendlyName = "是否包含请求的Get信息")]
        public bool IncludeRequestQueryStrings { get; set; }

        [ConfigItem(FriendlyName = "是否包含请求的Post信息")]
        public bool IncludeRequestForms { get; set; }

        [ConfigItem(FriendlyName = "是否包含请求的头")]
        public bool IncludeRequestHeaders { get; set; }

        [ConfigItem(FriendlyName = "是否包含请求的Cookie")]
        public bool IncludeRequestCookies { get; set; }
    }

    [ConfigEntity(FriendlyName = "包含信息策略_Http响应信息")]
    public class IncludeInfoStrategyForResponseInfo
    {
        [ConfigItem(FriendlyName = "是否包含响应信息")]
        public bool Include { get; set; }

        [ConfigItem(FriendlyName = "是否包含基本响应信息")]
        public bool IncludeBasicInfo { get; set; }

        [ConfigItem(FriendlyName = "是否包含响应的Cookie")]
        public bool IncludeResponseCookies { get; set; }
    }

    [ConfigEntity(FriendlyName = "包含信息策略")]
    public class IncludeInfoStrategy
    {
        [ConfigItem(FriendlyName = "策略名")]
        public string Name { get; set; }

        [ConfigItem(FriendlyName = "Http上下文信息的配置")]
        public IncludeInfoStrategyForHttpContextInfo IncludeInfoStrategyForHttpContextInfo { get; set; }

        [ConfigItem(FriendlyName = "Mvc上下文信息的配置")]
        public IncludeInfoStrategyForMvcContextInfo IncludeInfoStrategyForMvcContextInfo { get; set; }

        [ConfigItem(FriendlyName = "环境信息的配置")]
        public IncludeInfoStrategyForEnvironmentInfo IncludeInfoStrategyForEnvironmentInfo { get; set; }

        [ConfigItem(FriendlyName = "位置信息的配置")]
        public IncludeInfoStrategyForLocationInfo IncludeInfoStrategyForLocationInfo { get; set; }

        public IncludeInfoStrategy()
        {
           IncludeInfoStrategyForHttpContextInfo = new IncludeInfoStrategyForHttpContextInfo
           {
               IncludeInfoStrategyForRequestInfo = new IncludeInfoStrategyForRequestInfo()
               {
                   Include = false,
               },
               IncludeInfoStrategyForResponseInfo = new IncludeInfoStrategyForResponseInfo()
               {
                   Include = false,
               }
           };
           IncludeInfoStrategyForMvcContextInfo = new Imp.IncludeInfoStrategyForMvcContextInfo
           {
               Include = false,
           };
           IncludeInfoStrategyForEnvironmentInfo = new IncludeInfoStrategyForEnvironmentInfo
           {
               Include = false,
           };
           IncludeInfoStrategyForLocationInfo = new IncludeInfoStrategyForLocationInfo
           {
               Include = false,
           };
        }
    }
}
