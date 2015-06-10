
using System;
namespace Adhesive.AppInfoCenter
{
    public interface ICodePerformanceService : IDisposable
    {
        void StartPerformanceMeasure(string name, ExtraInfo extraInfo = null);

        void StartPerformanceMeasure(string name, string categoryName, string subcategoryName, ExtraInfo extraInfo = null);

        void SetPerformanceMeasurePoint(string name, string pointName);
    }
}
