
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;

//namespace Adhesive.AppInfoCenter.Imp
//{
//    internal class ApplicationStateService
//    {
//        internal static IEnumerable<BaseInfo> GetState()
//        {
//            var state = new ApplicationStateInfo();

//            try
//            {
//                Process process = Process.GetCurrentProcess();
//                state.WorkingSet64 = process.WorkingSet64;
//                state.VirtualMemorySize64 = process.VirtualMemorySize64;
//                state.PrivateMemorySize64 = process.PrivateMemorySize64;
//                state.PagedSystemMemorySize64 = process.PagedSystemMemorySize64;
//                state.PagedMemorySize64 = process.PagedMemorySize64;
//                state.NonpagedSystemMemorySize64 = process.NonpagedSystemMemorySize64;
//                state.ProcessName = process.ProcessName;

//            }
//            catch
//            {
//                state.WorkingSet64 = -1;
//                state.VirtualMemorySize64 = -1;
//                state.PrivateMemorySize64 = -1;
//                state.PagedSystemMemorySize64 = -1;
//                state.PagedMemorySize64 = -1;
//                state.NonpagedSystemMemorySize64 = -1;
//            }

//            int a, b;
//            int c, d;
//            try
//            {
//                ThreadPool.GetAvailableThreads(out a, out b);
//                ThreadPool.GetMaxThreads(out c, out d);
//                state.CurrentWorkThreadCount = c - a;
//                state.CurrentCompletionPortThreadCount = d - b;
//            }
//            catch
//            {
//            }
//            return new List<BaseInfo> { state };
//        }
//    }
//}
