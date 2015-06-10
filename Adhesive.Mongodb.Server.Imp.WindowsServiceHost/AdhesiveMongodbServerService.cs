using System;
using System.ServiceModel;
using System.ServiceProcess;
using Adhesive.Common;

namespace Adhesive.Mongodb.Server.Imp.WindowsServiceHost
{
    public partial class AdhesiveMongodbServerService : ServiceBase
    {
        private ServiceHost host;
        public AdhesiveMongodbServerService()
        {
            InitializeComponent();
        }
        private static ServiceHost _host;
        protected override void OnStart(string[] args)
        {
            try
            {
                AdhesiveFramework.Start();
                try
                {
                    _host = new ServiceHost(typeof(MongodbServer));
                    _host.Open();
                    LocalLoggingService.Info("完成启动Mongodb数据服务");
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error("Mongodb数据服务启动失败，异常信息：{0}", ex);
                }
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
                _host.Close();
                AdhesiveFramework.End();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("Windows服务关闭的时候出错，信息为 {0}", ex.ToString());
            }
        }
    }
}
