using System;

using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    internal class WebsitePageExecutionStateService : BaseService
    {
        internal static void Report(long executionTime)
        {
            try
            {
                if (!AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.WebsitePageExecutionStateConfig.Enabled ||
                    executionTime < AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.WebsitePageExecutionStateConfig.LogSlowPageExecutionMilliSecondsThreshold)
                    return;
                var info = new WebsitePageExecutionInfo();
                info.PageExecutionTime = executionTime;
                if (executionTime < 1000)
                    info.ExecutionTime = ExecutionTime.Less_1;
                else if (executionTime >= 1000 && executionTime < 2000)
                    info.ExecutionTime = ExecutionTime.Between_1_2;
                else if (executionTime >= 2000 && executionTime < 3000)
                    info.ExecutionTime = ExecutionTime.Between_2_3;
                else if (executionTime >= 3000 && executionTime < 5000)
                    info.ExecutionTime = ExecutionTime.Between_3_5;
                else
                    info.ExecutionTime = ExecutionTime.Greater_5;
                ProcessInfo(info);
                MongodbService.MongodbInsertService.Insert(info);
            }
            catch (Exception ex)
            {
                ex.Handle(AppInfoCenterService.ModuleName, "WebsitePageExecutionStateService", "Report");
            }
        }
    }
}
