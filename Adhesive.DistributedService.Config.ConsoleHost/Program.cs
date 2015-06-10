using System;
using Adhesive.Common;
using Adhesive.DistributedService.Imp;

namespace Adhesive.DistributedService.Config.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            AdhesiveFramework.Start();
            var host = WcfServiceHostFactory.CreateServiceHost<WcfConfigService>();
            host.Open();
            Console.WriteLine("Adhesive.DistributedService.Config.ConsoleHost started...");
            Console.ReadLine();
            host.Close();
            AdhesiveFramework.End();
        }
    }
}
