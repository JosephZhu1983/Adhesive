
using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;

namespace Adhesive.AppInfoCenter.Imp
{
    public class AppInfoCenterConfigurationDefaultConfig
    {
        private static List<StateServiceConfigurationItem> stateServiceConfigurationItems = new List<StateServiceConfigurationItem>();
        private static List<IncludeInfoStrategyConfigurationItem> includeInfoStrategyConfigurationItems = new List<IncludeInfoStrategyConfigurationItem>();

        public static void RegisterStateServiceConfigurationItem(StateServiceConfigurationItem item)
        {
            stateServiceConfigurationItems.Add(item);
        }

        public static void RegisterIncludeInfoStrategyConfigurationItem(IncludeInfoStrategyConfigurationItem item)
        {
            includeInfoStrategyConfigurationItems.Add(item);
        }

        internal static List<IncludeInfoStrategyConfigurationItem> GetIncludeInfoStrategyConfigurationItems()
        {
            return new List<IncludeInfoStrategyConfigurationItem>
            {
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(LogInfo).FullName,
#if DEBUG
                    IncludeInfoStrategyName = "Simple",
#else
                    IncludeInfoStrategyName = "None",
#endif
                    Conditions = new Dictionary<string, object>
                    {
                        { "LogLevel", LogLevel.Debug }
                    }
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(LogInfo).FullName,
                    IncludeInfoStrategyName = "Simple",
                    Conditions = new Dictionary<string, object>
                    {
                        { "LogLevel", LogLevel.Info  }
                    }
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(LogInfo).FullName,
                    IncludeInfoStrategyName = "Normal",
                    Conditions = new Dictionary<string, object>
                    {
                        { "LogLevel", LogLevel.Warning }
                    }
                }, 
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(LogInfo).FullName,
                    IncludeInfoStrategyName = "Detail",
                    Conditions = new Dictionary<string, object>
                    {
                        { "LogLevel", LogLevel.Error }
                    }
                },

                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(WcfUnhandledClientExceptionInfo).FullName,
                    IncludeInfoStrategyName = "Detail",
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(WcfUnhandledServerExceptionInfo).FullName,
                    IncludeInfoStrategyName = "Detail",
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(WebSiteUnhandledExceptionInfo).FullName,
                    IncludeInfoStrategyName = "Full",
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(MvcUnhandledExceptionInfo).FullName,
                    IncludeInfoStrategyName = "Full",
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(AppDomainUnhandledExceptionInfo).FullName,
                    IncludeInfoStrategyName = "Full",
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(HandledExceptionInfo).FullName,
                    IncludeInfoStrategyName = "Detail",
                },
                new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(WebsitePageExecutionInfo).FullName,
                    IncludeInfoStrategyName = "Detail",
                },
                 new IncludeInfoStrategyConfigurationItem
                {
                    TypeFullName = typeof(PerformanceInfo).FullName,
                    IncludeInfoStrategyName = "Normal",
                }
            }.Concat(includeInfoStrategyConfigurationItems).ToList();
        }

        internal static List<StateServiceConfigurationItem> GetStateServiceConfigurationItems()
        {
            return new List<StateServiceConfigurationItem>
            {
                //new StateServiceConfigurationItem
                //{
                //    Enabled = true,
                //    ReportStateIntervalMilliSeconds = CommonConfiguration.GetConfig().StateServiceDefaultReportStateIntervalMilliSeconds,
                //    TypeFullName = typeof(ApplicationStateInfo).FullName,
                //},
                //new StateServiceConfigurationItem
                //{
                //    Enabled = true,
                //    ReportStateIntervalMilliSeconds = CommonConfiguration.GetConfig().StateServiceDefaultReportStateIntervalMilliSeconds,
                //    TypeFullName = typeof(WebsiteRequestStateInfo).FullName,
                //},
            }.Concat(stateServiceConfigurationItems).ToList();
        }

        internal static List<LogStrategy> GetLogStrategy()
        {
            return new List<LogStrategy>
            {
                new LogStrategy
                {
                    LogLevel = LogLevel.Debug,
                    LocalLog = false,
                    RemoteLog = false,                 
                },
                new LogStrategy
                {
                    LogLevel = LogLevel.Info,
                    LocalLog = false,
                    RemoteLog = true,
                },
                new LogStrategy
                {
                    LogLevel = LogLevel.Warning,
                    LocalLog = false,
                    RemoteLog = true,

                },
                new LogStrategy
                {
                    LogLevel = LogLevel.Error,
                    LocalLog = false,
                    RemoteLog = true,
                }
            };
        }

        internal static List<ExceptionStrategy> GetExceptionStrategys()
        {
            return new List<ExceptionStrategy>
            {
                new ExceptionStrategy
                {
                    ExceptionInfoTypeName = typeof(WebSiteUnhandledExceptionInfo).Name,
                    ExceptionTypeName = "",
#if DEBUG
#else
                    ResponseStatusCode = 500,
                    RedirectUrl = "http://error.5173.com",
#endif
                    LocalLog = true,
                    RemoteLog = true,
                },
                new ExceptionStrategy
                {
                    ExceptionInfoTypeName = typeof(MvcUnhandledExceptionInfo).Name,
                    ExceptionTypeName = "",
#if DEBUG
#else
                    ResponseStatusCode = 500,
                    RedirectUrl = "http://error.5173.com",
#endif
                    LocalLog = true,
                    RemoteLog = true,
                },
                new ExceptionStrategy
                {
                    ExceptionInfoTypeName = typeof(WcfUnhandledClientExceptionInfo).Name,
                    ExceptionTypeName = "",
                    LocalLog = false,
                    RemoteLog = true,
                },
                new ExceptionStrategy
                {
                    ExceptionInfoTypeName = typeof(WcfUnhandledServerExceptionInfo).Name,
                    ExceptionTypeName = "",
                    LocalLog = false,
                    RemoteLog = true,
                }, 
                new ExceptionStrategy
                {
                    ExceptionInfoTypeName = typeof(HandledExceptionInfo).Name,
                    ExceptionTypeName = "",
                    LocalLog = false,
                    RemoteLog = true,
                },
                new ExceptionStrategy
                {
                    ExceptionInfoTypeName = typeof(AppDomainUnhandledExceptionInfo).Name,
                    ExceptionTypeName = "",
                    LocalLog = true,
                    RemoteLog = true,
                }
            };
        }

        internal static List<IncludeInfoStrategy> GetIncludeInfoStrategys()
        {
            return new List<IncludeInfoStrategy>
            {
                new IncludeInfoStrategy
                {
                    Name = "None",
                },
                new IncludeInfoStrategy
                {
                    Name = "Simple",
                    IncludeInfoStrategyForEnvironmentInfo = new IncludeInfoStrategyForEnvironmentInfo
                    {
                        Include = false,
                    },
                    IncludeInfoStrategyForHttpContextInfo = new IncludeInfoStrategyForHttpContextInfo
                    {
                        Include = true,
                        IncludeHttpContextItems = false,
                        IncludeHttpContextSessions = false,
                        IncludeInfoStrategyForRequestInfo = new IncludeInfoStrategyForRequestInfo
                        {
                            Include = true,
                            IncludeBasicInfo = false,
                            IncludeRequestHeaders = false,
                            IncludeRequestCookies = true,
                            IncludeRequestQueryStrings = true,
                            IncludeRequestForms = false,
                        },
                        IncludeInfoStrategyForResponseInfo = new IncludeInfoStrategyForResponseInfo
                        {
                            Include = true,
                            IncludeBasicInfo = false,
                            IncludeResponseCookies = false,
                        }
                    },
                    IncludeInfoStrategyForLocationInfo = new IncludeInfoStrategyForLocationInfo
                    {
                        Include = false,
                        IncludeStackTrace = true,
                    },
                    IncludeInfoStrategyForMvcContextInfo = new IncludeInfoStrategyForMvcContextInfo
                    {
                        Include = true,
                        IncludeParameterData = false,
                        IncludeRouteData = false,
                        IncludeTempData = false,
                        IncludeViewData = false,
                    }
                },
                new IncludeInfoStrategy
                {
                    Name = "Normal",
                    IncludeInfoStrategyForEnvironmentInfo = new IncludeInfoStrategyForEnvironmentInfo
                    {
                        Include = false,
                    },
                    IncludeInfoStrategyForHttpContextInfo = new IncludeInfoStrategyForHttpContextInfo
                    {
                        Include = true,
                        IncludeHttpContextItems = false,
                        IncludeHttpContextSessions = false,
                        IncludeInfoStrategyForRequestInfo = new IncludeInfoStrategyForRequestInfo
                        {
                            Include = true,
                            IncludeBasicInfo = false,
                            IncludeRequestHeaders = true,
                            IncludeRequestCookies = true,
                            IncludeRequestQueryStrings = true,
                            IncludeRequestForms = true,
                        },
                        IncludeInfoStrategyForResponseInfo = new IncludeInfoStrategyForResponseInfo
                        {
                            Include = true,
                            IncludeBasicInfo = false,
                            IncludeResponseCookies = true,
                        }
                    },
                    IncludeInfoStrategyForLocationInfo = new IncludeInfoStrategyForLocationInfo
                    {
                        Include = true,
                        IncludeStackTrace = true,
                    },
                    IncludeInfoStrategyForMvcContextInfo = new IncludeInfoStrategyForMvcContextInfo
                    {
                        Include = true,
                        IncludeParameterData = true,
                        IncludeRouteData = true,
                        IncludeTempData = false,
                        IncludeViewData = false,
                    }
                },
                new IncludeInfoStrategy
                {
                    Name = "Detail",
                    IncludeInfoStrategyForEnvironmentInfo = new IncludeInfoStrategyForEnvironmentInfo
                    {
                        Include = true,
                    },
                    IncludeInfoStrategyForHttpContextInfo = new IncludeInfoStrategyForHttpContextInfo
                    {
                        Include = true,
                        IncludeHttpContextItems = false,
                        IncludeHttpContextSessions = false,
                        IncludeInfoStrategyForRequestInfo = new IncludeInfoStrategyForRequestInfo
                        {
                            Include = true,
                            IncludeBasicInfo = true,
                            IncludeRequestHeaders = true,
                            IncludeRequestCookies = true,
                            IncludeRequestQueryStrings = true,
                            IncludeRequestForms = true,
                        },
                        IncludeInfoStrategyForResponseInfo = new IncludeInfoStrategyForResponseInfo
                        {
                            Include = true,
                            IncludeBasicInfo = false,
                            IncludeResponseCookies = true,
                        }
                    },
                    IncludeInfoStrategyForLocationInfo = new IncludeInfoStrategyForLocationInfo
                    {
                        Include = true,
                        IncludeStackTrace = true,
                    },
                    IncludeInfoStrategyForMvcContextInfo = new IncludeInfoStrategyForMvcContextInfo
                    {
                        Include = true,
                        IncludeParameterData = true,
                        IncludeRouteData = true,
                        IncludeTempData = true,
                        IncludeViewData = false,
                    }
                },
                new IncludeInfoStrategy
                {
                    Name = "Full",
                    IncludeInfoStrategyForEnvironmentInfo = new IncludeInfoStrategyForEnvironmentInfo
                    {
                        Include = true,
                    },
                    IncludeInfoStrategyForHttpContextInfo = new IncludeInfoStrategyForHttpContextInfo
                    {
                        Include = true,
                        IncludeHttpContextItems = true,
                        IncludeHttpContextSessions = true,
                        IncludeInfoStrategyForRequestInfo = new IncludeInfoStrategyForRequestInfo
                        {
                            Include = true,
                            IncludeBasicInfo = true,
                            IncludeRequestHeaders = true,
                            IncludeRequestCookies = true,
                            IncludeRequestQueryStrings = true,
                            IncludeRequestForms = true,
                        },
                        IncludeInfoStrategyForResponseInfo = new IncludeInfoStrategyForResponseInfo
                        {
                            Include = true,
                            IncludeBasicInfo = false,
                            IncludeResponseCookies = true,
                        }
                    },
                    IncludeInfoStrategyForLocationInfo = new IncludeInfoStrategyForLocationInfo
                    {
                        Include = true,
                        IncludeStackTrace = true,
                    },
                    IncludeInfoStrategyForMvcContextInfo = new IncludeInfoStrategyForMvcContextInfo
                    {
                        Include = true,
                        IncludeParameterData = true,
                        IncludeRouteData = true,
                        IncludeTempData = true,
                        IncludeViewData = true,
                    }
                },
            };
        }
    }

}
