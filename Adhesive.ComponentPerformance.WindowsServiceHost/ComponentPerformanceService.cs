using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Adhesive.ComponentPerformance.Core;
using Adhesive.Common;
using System.ServiceModel;

namespace Adhesive.ComponentPerformance.WindowsServiceHost
{
    public partial class ComponentPerformanceService : ServiceBase
    {
        public ComponentPerformanceService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Service.Start();
        }

        protected override void OnStop()
        {
            Service.Stop();
        }
    }
}
