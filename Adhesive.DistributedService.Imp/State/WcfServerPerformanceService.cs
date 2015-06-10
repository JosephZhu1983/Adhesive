using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Adhesive.AppInfoCenter.Imp;
using Adhesive.AppInfoCenter;
using Adhesive.Common;

namespace Adhesive.DistributedService.Imp
{
    internal class WcfServerPerformanceService
    {
        //private Dictionary<string, GeneralPerformanceInfo> state = new Dictionary<string, GeneralPerformanceInfo>();
        //private Thread thread;
        //private WcfPerformanceServiceSetting config;
        //private string name;

        internal WcfServerPerformanceService(string name, WcfPerformanceServiceSetting config)
        {
            //if (config == null)
            //{
            //    AppInfoCenterService.LoggingService.Warning("WcfServerPerformanceService " + name + "没有获取到config！！");
            //    return;
            //}
            //this.config = config;
            //this.name = name;
            //if (!config.Enabled) return;
            //thread = new Thread(Report)
            //{
            //    Name = "WcfServerPerformanceService" + name,
            //    IsBackground = true,
            //};
            //thread.Start();
        }

        private void Report()
        {
          
        }

     

        internal void EndInvoke(string url, long time, bool success)
        {
         
        }
    }
}
