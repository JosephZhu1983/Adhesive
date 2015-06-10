
using System;
using System.Diagnostics;

namespace Adhesive.AppInfoCenter.Imp
{
    public class EnvironmentInfoProvider : IInfoProvider
    {
        private static EnvironmentInfo environmentInfo;
        public void ProcessInfo(IncludeInfoStrategy strategy, AbstractInfo info)
        {
            if (!strategy.IncludeInfoStrategyForEnvironmentInfo.Include) return;

            if (environmentInfo == null)
            {
                environmentInfo = new EnvironmentInfo()
                {
                    CurrentDirectory = Environment.CurrentDirectory,
                    MachineName = Environment.MachineName,
                    OperatingSystem = Environment.OSVersion.ToString(),
                    //ProcessId = Process.GetCurrentProcess().Id,
                    //ProcessName = Process.GetCurrentProcess().ProcessName,
                    UserInteractive = Environment.UserInteractive,
                    UserDomainName = Environment.UserDomainName,
                    UserName = Environment.UserName,
                    Version = Environment.Version.ToString(),
                };
            }
            info.EnvironmentInfo = environmentInfo;
        }
    }
}
