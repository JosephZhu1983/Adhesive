using System;
using System.ServiceProcess;
using Adhesive.Common;

namespace Adhesive.Alarm.WindowsServiceHost
{
    public partial class AdhesiveAlarmService : ServiceBase
    {
        public AdhesiveAlarmService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                AdhesiveFramework.Start();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("Windows服务启动的时候出错，信息为 {0}", ex.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                AdhesiveFramework.End();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("Windows服务关闭的时候出错，信息为 {0}", ex.ToString());
            }
        }
    }
}
