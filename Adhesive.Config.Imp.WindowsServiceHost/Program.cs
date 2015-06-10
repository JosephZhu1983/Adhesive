using System.ServiceProcess;

namespace Adhesive.Config.Imp.WindowsServiceHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new AdhesiveConfigService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
