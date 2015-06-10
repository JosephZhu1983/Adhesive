
using Adhesive.GeneralPerformance.Core;
using System;
namespace Adhesive.GeneralPerformance.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Service.Start();
            Console.WriteLine("启动成功...");
            Console.ReadLine();
        }
    }
}
