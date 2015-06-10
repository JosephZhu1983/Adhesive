using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Adhesive.GeneralPerformance.Core;

namespace Adhesive.GeneralPerformance.WindowsServiceSetup
{
    public partial class GeneralPerformanceService : ServiceBase
    {
        public GeneralPerformanceService()
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
