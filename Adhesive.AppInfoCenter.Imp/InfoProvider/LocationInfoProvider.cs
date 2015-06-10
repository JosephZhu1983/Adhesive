
using System;
using System.Diagnostics;
using System.Threading;

namespace Adhesive.AppInfoCenter.Imp
{
    internal class LocationInfoProvider : IInfoProvider
    {
        public void ProcessInfo(IncludeInfoStrategy strategy, AbstractInfo info)
        {
            if (!strategy.IncludeInfoStrategyForLocationInfo.Include) return;

            var locationInfo = new LocationInfo()
            {
                AppDomainName = AppDomain.CurrentDomain.FriendlyName,
                ManagedThreadId = Thread.CurrentThread.ManagedThreadId,
            };

            StackTrace strackTrace = new StackTrace();
            StackFrame[] stackFrames = strackTrace.GetFrames();
            int skip = 1;
            for (int i = 0; i < stackFrames.Length; i++)
            {
                if (stackFrames[i].GetMethod().ReflectedType != null &&
                    stackFrames[i].GetMethod().ReflectedType.Assembly.FullName.Contains("Adhesive.AppInfoCenter"))
                {
                    skip++;
                }
            }
            if (stackFrames.Length >= skip)
            {
                StackFrame stackFrame = stackFrames[skip];
                if (stackFrame != null)
                {
                    var methodBase = stackFrame.GetMethod();

                    if (methodBase != null)
                    {
                        locationInfo.AssemblyName = methodBase.Module.Assembly.ToString();
                        locationInfo.ModuleName = methodBase.Module.ToString();
                        locationInfo.TypeName = methodBase.ReflectedType.ToString();
                        locationInfo.MethodName = methodBase.ToString();
                    }
                }

                StackTrace skipped = new StackTrace(skip, true);
                if (skipped != null)
                {
                    if (strategy.IncludeInfoStrategyForLocationInfo.IncludeStackTrace)
                        locationInfo.StackTrace = skipped.ToString();
                    else
                        locationInfo.StackTrace = string.Empty;
                }
            }

            info.LocationInfo = locationInfo;
        }
    }
}
