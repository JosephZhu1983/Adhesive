
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Adhesive.Common;

namespace Adhesive.AppInfoCenter
{
    /// <summary>
    /// 应用程序信息中心模块入口
    /// </summary>
    public class AppInfoCenterService
    {
        public static readonly string ModuleName = "信息中心模块";

        private static Timer healthTimer;

        public static bool HealthActionRegistered
        {
            get
            {
                return healthTimer != null;
            }
        }

        /// <summary>
        /// 注册健康检测回调方法
        /// </summary>
        /// <param name="healthCallback">回调方法，返回空字符串表示健康，否则返回错误提示</param>
        /// <param name="checkInterval">健康检测的周期</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RegisterHealthAction(Func<string> healthCallback, TimeSpan checkInterval)
        {
            if (healthCallback != null && healthTimer == null)
            {
                healthTimer = new Timer(state =>
                {
                    try
                    {
                        var result = healthCallback();
                        if (!string.IsNullOrEmpty(result))
                        {
                            LoggingService.Error(AppInfoCenterService.ModuleName, "AppInfoCenterService", "RegisterHealthAction", string.Format("检查健康状态的回调方法返回的状态值为：'{0}'", result));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Handle(AppInfoCenterService.ModuleName, "AppInfoCenterService", "RegisterHealthAction", "检查健康状态的回调方法出错");
                    }
                }, null, checkInterval, checkInterval);
            }
        }

        /// <summary>
        /// 日志服务
        /// </summary>
        public static ILoggingService LoggingService
        {
            get
            {
                return LocalServiceLocator.GetService<ILoggingService>();
            }
        }

        /// <summary>
        /// 异常服务
        /// </summary>
        public static IExceptionService ExceptionService
        {
            get
            {
                return LocalServiceLocator.GetService<IExceptionService>();
            }
        }

        /// <summary>
        /// 性能服务
        /// </summary>
        public static ICodePerformanceService PerformanceService
        {
            get
            {
                return LocalServiceLocator.GetService<ICodePerformanceService>();
            }
        }

        /// <summary>
        /// 状态服务
        /// </summary>
        public static IStateService StateService
        {
            get
            {
                return LocalServiceLocator.GetService<IStateService>();
            }
        }
    }
}
