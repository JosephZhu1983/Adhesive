
//using System;
//using System.Collections.Generic;
//using Adhesive.AppInfoCenter;

//namespace Adhesive.DistributedService.Imp
//{
//    internal class WcfClientStateService
//    {
//        private static object locker = new object();
//        private static Dictionary<string, WcfClientStateItem> state = new Dictionary<string, WcfClientStateItem>();

//        internal static List<BaseInfo> GetState()
//        {
//            var s = new List<BaseInfo>() 
//            {
//                new WcfClientStateInfo
//                {
//                    WcfClientStateItems = state,
//                }
//            };
//            return s;
//        }

//        internal static void BeginInvoke(string contractName)
//        {
//            try
//            {
//                lock (locker)
//                {
//                    if (state.ContainsKey(contractName))
//                    {
//                        WcfClientStateItem si = state[contractName];
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
//                        state.Add(contractName, new WcfClientStateItem
//                        {
//                            TotalRequestCount = 1,
//                            CurrentRequestCount = 1,
//                            ContractName = contractName,
//                        });
//                    }
//                }
//            }
//            catch(Exception ex)
//            {
//                ex.Handle(WcfLogProvider.ModuleName, "WcfClientStateService", "BeginInvoke");
//            }
//        }

//        internal static void EndInvoke(string contractName, long time, bool success)
//        {
//            try
//            {
//                if (state.ContainsKey(contractName))
//                {
//                    WcfClientStateItem si = state[contractName];
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
//                ex.Handle(WcfLogProvider.ModuleName, "WcfClientStateService", "EndInvoke");
//            }
//        }
//    }
//}
