using System;
using System.ServiceModel;
using System.ServiceProcess;
using Adhesive.Common;
using Adhesive.DistributedService.Imp;

namespace Adhesive.DistributedService.Config.WindowsServiceHost
{
    public partial class AdhesiveDistributedServiceConfigService : ServiceBase
    {
        private ServiceHost host;
        public AdhesiveDistributedServiceConfigService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                AdhesiveFramework.Start();
                host = WcfServiceHostFactory.CreateServiceHost<WcfConfigService>();
                host.Open();
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
                host.Close();
                AdhesiveFramework.End();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("Windows服务关闭的时候出错，信息为 {0}", ex.ToString());
            }
        }
    }
}
