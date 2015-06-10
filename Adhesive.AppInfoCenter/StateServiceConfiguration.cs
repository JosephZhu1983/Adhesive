
using System;
using System.Collections.Generic;

namespace Adhesive.AppInfoCenter
{
    public class StateServiceConfiguration
    {
        public Func<IEnumerable<BaseInfo>> ReportStateFunc { get; private set; }

        public string TypeFullName { get; private set; }

        public StateServiceConfiguration(string typeFullName, Func<IEnumerable<BaseInfo>> reportStateFunc)
        {
            this.ReportStateFunc = reportStateFunc;
            this.TypeFullName = typeFullName;
        }
    }
}
