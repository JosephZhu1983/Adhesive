using System;
using Adhesive.Common;
using Adhesive.DistributedService.Imp;

namespace Adhesive.DistributedService.TestService.Host
{
    class Program
    {

        static void Main(string[] args)
        {
            AdhesiveFramework.Start();
            var host = WcfServiceHostFactory.CreateServiceHost<FuckService>();
            host.Open();
            Console.WriteLine("FuckService started...");
            Console.ReadLine();
            host.Close();
            AdhesiveFramework.End();
        }
    }
}
