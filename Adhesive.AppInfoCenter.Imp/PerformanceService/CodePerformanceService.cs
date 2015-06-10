

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class CodePerformanceService : BaseService, ICodePerformanceService
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime,
           out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        internal void EndPerformanceMeasure(long executionTime)
        {
            if (!AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PerformanceMeasureConfig.Enabled) return;

            if (executionTime < AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PerformanceMeasureConfig.PageExecutionMilliSecondsThreshold ||
                DateTime.Now < AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PerformanceMeasureConfig.BeginTime ||
                DateTime.Now > AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PerformanceMeasureConfig.EndTime)
                return;

            if (HttpContext.Current == null || HttpContext.Current.Items == null) return;

            var info = HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextPerformanceMeasureKey] as Dictionary<string, PerformanceInfo>;
            if (info == null) return;

            foreach (var item in info)
            {
                try
                {
                    var pi = item.Value;
                    ProcessInfo(pi);
                    if (pi.PerformancePoints != null)
                    {
                        pi.TotalCPUTime = pi.PerformancePoints.SelectMany(p => p.Value.PerformancePointItems).Sum(p => p.CPUTime);
                        pi.TotalTimeElapsed = pi.PerformancePoints.SelectMany(p => p.Value.PerformancePointItems).Sum(p => p.TimeElapsed);
                        pi.TotalPerformancePointCount = pi.PerformancePoints.Count;
                    }

                    foreach (var point in pi.PerformancePoints)
                    {
                        point.Value.TotalPerformancePointItemCount = point.Value.PerformancePointItems.Count;
                        point.Value.AverageCPUTime = Convert.ToInt32(point.Value.PerformancePointItems.Average(ppi => ppi.CPUTime));
                        point.Value.AverageTimeElapsed = Convert.ToInt32(point.Value.PerformancePointItems.Average(ppi => ppi.TimeElapsed));
                    }
                    MongodbService.MongodbInsertService.Insert(pi);

                }
                catch (Exception ex)
                {
                    ex.Handle(AppInfoCenterService.ModuleName, ServiceName, "EndPerformanceMeasure");
                }
            }

        }

        private long GetCurrentThreadTimes()
        {
            long l;
            long kernelTime, userTimer;
            GetThreadTimes(GetCurrentThread(), out l, out l, out kernelTime, out userTimer);
            return kernelTime + userTimer;
        }

        public void StartPerformanceMeasure(string name, ExtraInfo extraInfo = null)
        {
            StartPerformanceMeasure(name, string.Empty, string.Empty, extraInfo);
        }

        public void StartPerformanceMeasure(string name, string categoryName, string subcategoryName, ExtraInfo extraInfo = null)
        {
            try
            {
                if (!AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PerformanceMeasureConfig.Enabled) return;
                if (HttpContext.Current == null) return;
                Dictionary<string, PerformanceInfo> info = HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextPerformanceMeasureKey] as Dictionary<string, PerformanceInfo>;
                if (info == null) info = new Dictionary<string, PerformanceInfo>();

                if (!info.ContainsKey(name))
                {
                    info.Add(name, new PerformanceInfo
                    {
                        Name = name,
                        sw = Stopwatch.StartNew(),
                        threadTime = GetCurrentThreadTimes(),
                        PerformancePoints = new Dictionary<string, PerformancePoint>(),
                    });
                }
                HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextPerformanceMeasureKey] = info;
            }
            catch (Exception ex)
            {
                ex.Handle(AppInfoCenterService.ModuleName, ServiceName, "StartPerformanceMeasure");
            }
        }

        public void SetPerformanceMeasurePoint(string name, string pointName)
        {
            try
            {
                if (HttpContext.Current == null) return;
                Dictionary<string, PerformanceInfo> info = HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextPerformanceMeasureKey] as Dictionary<string, PerformanceInfo>;
                if (info == null || !info.ContainsKey(name)) return;

                PerformanceInfo pi = info[name];
                var time = GetCurrentThreadTimes();
                var ppItem = new PerformancePointItem
                {
                    CPUTime = (time - pi.threadTime) * 100,
                    TimeElapsed = pi.sw.ElapsedMilliseconds,
                    Time = DateTime.Now,
                };
                pi.threadTime = time;
                pi.sw = Stopwatch.StartNew();
                if (!pi.PerformancePoints.ContainsKey(pointName))
                {
                    pi.PerformancePoints[pointName] = new PerformancePoint
                    {
                        PerformancePointItems = new List<PerformancePointItem>(),
                    };
                }
                pi.PerformancePoints[pointName].PerformancePointItems.Add(ppItem);
                HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextPerformanceMeasureKey] = info;
            }
            catch (Exception ex)
            {
                ex.Handle(AppInfoCenterService.ModuleName, ServiceName, "SetPerformanceMeasurePoint");
            }
        }
    }
}
