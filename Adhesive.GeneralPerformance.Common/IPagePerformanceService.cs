using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adhesive.GeneralPerformance.Common
{
    public interface IPagePerformanceService
    {
        void SubmitPagePerformance(PagePerformanceInfo info);
    }
}
