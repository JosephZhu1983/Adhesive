using System;
using System.ServiceProcess;
using Adhesive.Common;


namespace Adhesive.Config.Imp.WindowsServiceHost
{
    public partial class AdhesiveConfigService : ServiceBase
    {

        public AdhesiveConfigService()
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
                Environment.Exit(0);
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
