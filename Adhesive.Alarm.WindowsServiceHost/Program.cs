using System.ServiceProcess;

namespace Adhesive.Alarm.WindowsServiceHost
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new AdhesiveAlarmService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
