using System.ServiceProcess;

namespace Adhesive.DistributedService.Config.WindowsServiceHost
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
				new AdhesiveDistributedServiceConfigService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
