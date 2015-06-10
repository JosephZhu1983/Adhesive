using System.ServiceModel;
using Adhesive.Common;
using Adhesive.DistributedService.Imp;
using Adhesive.GeneralPerformance.Common;
using System.Collections.Generic;

namespace Adhesive.GeneralPerformance.Core
{
    public class Service : IService
    {
        private static ServiceHost host;
        private static Dictionary<string, GreneralPerformanceCollector> collectors = new Dictionary<string, GreneralPerformanceCollector>();
        private static Dictionary<string, GeneralPerformanceAggregator> aggregators = new Dictionary<string, GeneralPerformanceAggregator>();
        
        public void SubmitPagePerformanceInfo(GeneralPerformanceInfo info)
        {
            collectors[Configuration.GetConfig().PagePerformance.Name].AddData(info);
        }

        public void SubmitWcfClientPerformanceInfo(GeneralPerformanceInfo info)
        {
            collectors[Configuration.GetConfig().WcfClientPerformance.Name].AddData(info);
        }

        public void SubmitWcfServerPerformanceInfo(GeneralPerformanceInfo info)
        {
            collectors[Configuration.GetConfig().WcfServerPerformance.Name].AddData(info);
        }

        public static void Start()
        {
            AdhesiveFramework.Start();
            LocalLoggingService.Info("框架启动成功");
            StartService();
            LocalLoggingService.Info("WCF服务启动成功");
            var config = Configuration.GetConfig();
            var pp = new GreneralPerformanceCollector(config.PagePerformance);
            pp.Start();
            collectors.Add(config.PagePerformance.Name, pp);
            //var wcp = new GreneralPerformanceCollector(config.WcfClientPerformance);
            //wcp.Start();
            //collectors.Add(config.WcfClientPerformance.Name, wcp);
            var wsp = new GreneralPerformanceCollector(config.WcfServerPerformance);
            wsp.Start();
            collectors.Add(config.WcfServerPerformance.Name,wsp);
            LocalLoggingService.Info("收集器启动成功");

            var ppg = new GeneralPerformanceAggregator(config.PagePerformance);
            ppg.Start();
            aggregators.Add(config.PagePerformance.Name, ppg);
            //var wcpg = new GeneralPerformanceAggregator(config.WcfClientPerformance);
            //wcpg.Start();
            //aggregators.Add(config.WcfClientPerformance.Name, wcpg);
            var wspg = new GeneralPerformanceAggregator(config.WcfServerPerformance);
            wspg.Start();
            aggregators.Add(config.WcfServerPerformance.Name, wspg);
            LocalLoggingService.Info("聚合器启动成功");
        }

        public static void Stop()
        {
            collectors.Clear();
            aggregators.Clear();
            AdhesiveFramework.End();
            host.Close();
        }

        private static void StartService()
        {
            host = WcfServiceHostFactory.CreateServiceHost<Service>();
            host.Open();
        }
    }
}
