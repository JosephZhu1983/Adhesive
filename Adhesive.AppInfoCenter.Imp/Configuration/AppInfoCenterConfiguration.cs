
using System;
using System.Linq;
using Adhesive.Common;
using Adhesive.Config;
using System.Collections.Generic;

namespace Adhesive.AppInfoCenter.Imp
{
    public class AppInfoCenterConfiguration
    {
        private static IConfigService configService = LocalServiceLocator.GetService<IConfigService>();

        public static AppInfoCenterConfigurationEntity GetConfig()
        {
            var defaultConfig = new AppInfoCenterConfigurationEntity
            {
                IncludeInfoStrategys = AppInfoCenterConfigurationDefaultConfig.GetIncludeInfoStrategys().ToDictionary(s => s.Name),

                CommonConfig = new CommonConfig
                {
                    EmbedInfoToPage = false,
                },
                ExceptionServiceConfig = new ExceptionServiceConfig
                {
                    Enabled = true,
                    UnhandledExceptionMessage = "如果您看到这个页面，说明系统出错了，请记录错误ID并且汇报给我们的客服以便我们改进系统，谢谢。（错误ID：{0}）",
                    StrategyList = AppInfoCenterConfigurationDefaultConfig.GetExceptionStrategys().ToDictionary(s => string.Format("{0}_{1}_{2}", s.ModuleName, s.ExceptionInfoTypeName, s.ExceptionTypeName)),
                    UnhandledExceptionFilterConfig = new UnhandledExceptionFilterConfig
                        {
                            Enabled = false,
                            SpiderExceptionFilterConfig = new SpiderExceptionFilterConfig
                            {
                               Enabled = false,
                               SpiderIdList = new List<string>()
                            }
                        }
                },
                LoggingServiceConfig = new LoggingServiceConfig
                {
                    Enabled = true,
                    StrategyList = AppInfoCenterConfigurationDefaultConfig.GetLogStrategy().ToDictionary(s => string.Format("{0}_{1}", s.ModuleName, s.LogLevel.ToString()))
                },
                PerformanceServiceConfig = new PerformanceServiceConfig
                {
                    WebsitePageExecutionStateConfig = new WebsitePageExecutionStateConfig
                    {
                        Enabled = false,
                        LogSlowPageExecutionMilliSecondsThreshold = 1000,
                    },
                    PerformanceMeasureConfig = new PerformanceMeasureConfig
                    {
                        Enabled = true,
                        BeginTime = DateTime.MinValue,
                        EndTime = DateTime.MaxValue,
                        PageExecutionMilliSecondsThreshold = 0,
                    },
                    PagePerformanceServiceConfig = new PagePerformanceServiceConfig
                    {
                        AllowUrls = new List<string> { "*" },
                        DenyUrls = new List<string>(),
                        Enabled = false,
                        ReportStateIntervalMilliSeconds = 1000 * 10,
                    },
                },
                StateServiceConfig = new StateServiceConfig
                {
                    StateServiceConfigurationItems = AppInfoCenterConfigurationDefaultConfig.GetStateServiceConfigurationItems().ToDictionary(s => s.TypeFullName),
                },
                IncludeInfoStrategyConfigurations = AppInfoCenterConfigurationDefaultConfig.GetIncludeInfoStrategyConfigurationItems().ToDictionary(s =>
                {
                    var name = s.TypeFullName;
                    var conditions = s.Conditions;
                    if (conditions != null)
                    {
                        foreach (var condition in conditions)
                        {
                            name += string.Format("_{0}={1}", condition.Key, condition.Value);
                        }
                    }
                    return name;
                }),
            };

            var config = configService.GetConfigItemValue(false, "AppInfoCenterConfiguration", defaultConfig);
            return config;
        }

        public static class Const
        {
            public static readonly string ContextIdentityKey = "ContextIdentity";
            public static readonly string ControllerContextIdentityKey = "ControllerContextIdentity";
            internal static readonly string ContextStopwatchKey = "ContextStopwatch";
            internal static readonly string ContextPerformanceMeasureKey = "ContextPerformanceMeasure";
            
        }
    }
}
