
//using System;
//using System.Collections.Generic;
//using Adhesive.AppInfoCenter;

//namespace Adhesive.DistributedService.Imp
//{
//    internal class WcfServerStateService
//    {
//        private static object locker = new object();
//        private static Dictionary<string, WcfServerStateItem> state = new Dictionary<string, WcfServerStateItem>();

//        internal static List<BaseInfo> GetState()
//        {
//            var s = new List<BaseInfo>() 
//            {
//                new WcfServerStateInfo
//                {
//                    WcfServerStateItems = state,
//                }
//            };
//            return s;
//        }

//        internal static void BeginInvoke(string serviceName)
//        {
//            try
//            {
//                lock (locker)
//                {
//                    if (state.ContainsKey(serviceName))
//                    {
//                        WcfServerStateItem si = state[serviceName];
//                        si.TotalRequestCount++;
//                        si.CurrentRequestCount++;
//                        if (si.CurrentRequestCount > si.MaxRequestCount)
//                        {
//                            si.MaxRequestCount = si.CurrentRequestCount;
//                            si.MaxRequestCountOccur = DateTime.Now;
//                        }
//                    }
//                    else
//                    {
//                        state.Add(serviceName, new WcfServerStateItem
//                        {
//                            TotalRequestCount = 1,
//                            CurrentRequestCount = 1,
//                            ServiceName = serviceName,
//                        });
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.Handle(WcfLogProvider.ModuleName, "WcfServerStateService", "BeginInvoke");
//            }
//        }

//        internal static void EndInvoke(string serviceName, long time, bool success)
//        {
//            try
//            {
//                if (state.ContainsKey(serviceName))
//                {
//                    WcfServerStateItem si = state[serviceName];
//                    lock (si)
//                    {
//                        if (!success)
//                        {
//                            si.TotalErrorRequestCount++;
//                            si.LastErrorRequestExecutionTimeOccur = DateTime.Now;
//                            si.LastErrorRequestExecutionTime = time;
//                        }
//                        else
//                        {
//                            si.TotalRequestExecutionTime += time;
//                            if (si.CurrentRequestCount > 0)
//                                si.CurrentRequestCount--;
//                            if (si.TotalRequestCount > 1)
//                                si.AverageRequestExecutionTime = (si.TotalRequestExecutionTime - si.MaxRequestExecutionTime) / (si.TotalRequestCount - 1);
//                            if (si.MaxRequestExecutionTime < time)
//                            {
//                                si.MaxRequestExecutionTimeOccur = DateTime.Now;
//                                si.MaxRequestExecutionTime = time;
//                            }
//                        }
//                        si.LastRequestExecutionTime = time;
//                        si.LastRequestExecutionTimeOccur = DateTime.Now;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.Handle(WcfLogProvider.ModuleName, "WcfServerStateService", "EndInvoke");
//            }
//        }
//    }
//}
