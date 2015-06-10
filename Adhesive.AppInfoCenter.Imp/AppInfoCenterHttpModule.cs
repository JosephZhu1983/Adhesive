
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Adhesive.Common;

namespace Adhesive.AppInfoCenter.Imp
{
    internal class AppInfoCenterHttpModule : IHttpModule
    {
        private static IEnumerable<Type> _filterTypes;
        static AppInfoCenterHttpModule()
        {
            _filterTypes = BuildManagerWrapper.Current.ConcreteTypes.Where(type => typeof(IUnhandledExceptionFilter).IsAssignableFrom(type));
        }
        public void Dispose()
        {

        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(application_BeginRequest);
            application.EndRequest += new EventHandler(application_EndRequest);
            application.Error += new EventHandler(application_Error);
        }

        private void application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                if (AdhesiveFramework.Status != AdhesiveFrameworkStatus.Started) return;

                HttpContext httpContext = HttpContext.Current;
                if (httpContext == null) return;

                httpContext.Items[AppInfoCenterConfiguration.Const.ContextIdentityKey] = Guid.NewGuid();
                httpContext.Items[AppInfoCenterConfiguration.Const.ContextStopwatchKey] = Stopwatch.StartNew();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("application_BeginRequest" + ex.ToString());
            }
        }

        private void application_EndRequest(object sender, EventArgs e)
        {
            try
            {
                if (AdhesiveFramework.Status != AdhesiveFrameworkStatus.Started) return;

                HttpContext httpContext = HttpContext.Current;
                if (httpContext == null) return;

                long pageExecutionTime = -1;
                if (httpContext.Items.Contains(AppInfoCenterConfiguration.Const.ContextStopwatchKey))
                {
                    Stopwatch sw = httpContext.Items[AppInfoCenterConfiguration.Const.ContextStopwatchKey] as Stopwatch;
                    if (sw != null)
                    {
                        pageExecutionTime = sw.ElapsedMilliseconds;
                        //网站慢页面请求
                        WebsitePageExecutionStateService.Report(pageExecutionTime);
                        //代码性能测量服务
                        ((CodePerformanceService)AppInfoCenterService.PerformanceService).EndPerformanceMeasure(pageExecutionTime);
                    }
                }

                //嵌入页面
                if (AppInfoCenterConfiguration.GetConfig().CommonConfig.EmbedInfoToPage)
                {
                    httpContext.Response.Write("<!--");
                    httpContext.Response.Write(httpContext.Server.MachineName);
                    httpContext.Response.Write(", ");
                    httpContext.Response.Write(DateTime.Now.ToString());
                    httpContext.Response.Write(", ");
                    httpContext.Response.Write(string.Concat(pageExecutionTime.ToString(), " ms"));
                    httpContext.Response.Write("-->");
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("application_EndRequest" + ex.ToString());
            }
        }

        private void application_Error(object sender, EventArgs e)
        {
            try
            {
                if (AdhesiveFramework.Status != AdhesiveFrameworkStatus.Started) return;

                if (!AppInfoCenterConfiguration.GetConfig().ExceptionServiceConfig.Enabled) return;

                HttpContext httpContext = HttpContext.Current;
                if (httpContext == null || httpContext.Server == null) return;

                if (httpContext.Handler is DefaultHttpHandler)
                    return;


                Exception exception = httpContext.Server.GetLastError();

                //提取内部异常
                if (exception is HttpUnhandledException && exception.InnerException != null)
                    exception = exception.InnerException;

                if (exception is HttpException && exception.InnerException != null)
                    exception = exception.InnerException;

                if (exception == null) return;
                if (exception is HttpRequestValidationException) return;
                bool filtered = false;
                if (AppInfoCenterConfiguration.GetConfig().ExceptionServiceConfig.UnhandledExceptionFilterConfig.Enabled)
                {
                    foreach (var filterType in _filterTypes)
                    {
                        IUnhandledExceptionFilter filter = (IUnhandledExceptionFilter)Activator.CreateInstance(filterType);
                        if (filter.DoFilter(httpContext))
                        {
                            filtered = true;
                            break;
                        }
                    }
                }
                //处理
                if (!filtered)
                    ((ExceptionService)AppInfoCenterService.ExceptionService).WebSiteUnhandledException(exception);
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("application_Error" + ex.ToString());
            }
        }
    }
}
