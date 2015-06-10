
//using System;
//using System.Collections.Generic;

//namespace Adhesive.AppInfoCenter.Imp
//{
//    internal class ActiveRequestStateService
//    {
//        private static object locker = new object();
//        private static Dictionary<string, WebsiteRequestStateItem> state = new Dictionary<string, WebsiteRequestStateItem>();

//        internal static List<BaseInfo> GetState()
//        {
//            var s = new List<BaseInfo>() 
//            {
//                new WebsiteRequestStateInfo
//                {
//                    WebsiteRequestStateItems = state,
//                }
//            };
//            return s;
//        }

//        internal static void BeginRequest(string url)
//        {
//            try
//            {
//                lock (locker)
//                {
//                    if (state.ContainsKey(url))
//                    {
//                        WebsiteRequestStateItem ar = state[url];
//                        ar.TotalRequestCount++;
//                        ar.CurrentRequestCount++;
//                        if (ar.CurrentRequestCount > ar.MaxRequestCount)
//                        {
//                            ar.MaxRequestCount = ar.CurrentRequestCount;
//                            ar.MaxRequestCountOccur = DateTime.Now;
//                        }
//                    }
//                    else
//                    {
//                        state.Add(url, new WebsiteRequestStateItem
//                        {
//                            TotalRequestCount = 1,
//                            CurrentRequestCount = 1,
//                            Url = url,
//                        });
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.Handle(AppInfoCenterService.ModuleName, "ActiveRequestStateService", "BeginRequest");
//            }
//        }

//        internal static void EndRequest(string url, long time)
//        {
//            try
//            {
//                if (state.ContainsKey(url))
//                {
//                    WebsiteRequestStateItem ar = state[url];
//                    lock (ar)
//                    {
//                        ar.TotalRequestExecutionTime += time;
//                        if (ar.CurrentRequestCount > 0)
//                            ar.CurrentRequestCount--;
//                        if (ar.TotalRequestCount > 1)
//                            ar.AverageRequestExecutionTime = (ar.TotalRequestExecutionTime - ar.MaxRequestExecutionTime) / (ar.TotalRequestCount - 1);
//                        ar.LastRequestExecutionTime = time;
//                        ar.LastRequestExecutionTimeOccur = DateTime.Now;
//                        if (ar.MaxRequestExecutionTime < time)
//                        {
//                            ar.MaxRequestExecutionTimeOccur = DateTime.Now;
//                            ar.MaxRequestExecutionTime = time;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ex.Handle(AppInfoCenterService.ModuleName, "ActiveRequestStateService", "EndRequest");
//            }
//        }
//    }
//}
