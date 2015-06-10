
using System;
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.AppInfoCenter.Imp
{
    public class InitServiceTask : InitServiceBootstrapperTask
    {
        //private static IStateService applicationStateService;
        //private static IStateService activeRequestStateService;

        public override string Description
        {
            get
            {
                return "挂载应用程序未处理异常、启动页面性能统计服务";
            }
        }

        public InitServiceTask(IUnityContainer container)
            : base(container)
        {
        }

        public override TaskContinuation Execute()
        {
            try
            {
                if (!AppInfoCenterService.HealthActionRegistered)
                    LocalLoggingService.Debug("请调用AppInfoCenterService.RegisterHealthAction()注册健康检测回调方法！");

                //应用程序域未处理异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                //页面性能统计服务
                //PagePerformanceService.Init();

                //applicationStateService = AppInfoCenterService.StateService;
                //applicationStateService.Init(new StateServiceConfiguration(typeof(ApplicationStateInfo).FullName, ApplicationStateService.GetState));
                //if (HttpContext.Current != null)
                //{
                //    activeRequestStateService = AppInfoCenterService.StateService;
                //    activeRequestStateService.Init(new StateServiceConfiguration(typeof(WebsiteRequestStateInfo).FullName, ActiveRequestStateService.GetState));
                //}
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("初始化信息中心服务出错，异常信息：{0}", ex);
                return TaskContinuation.Break;
            }
            return TaskContinuation.Continue;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                ((ExceptionService)AppInfoCenterService.ExceptionService).AppDomainUnhandledException(e.ExceptionObject as Exception, e.IsTerminating);
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("处理应用程序域未处理异常出错，异常信息：{0}", ex);
            }
        }
    }
}
