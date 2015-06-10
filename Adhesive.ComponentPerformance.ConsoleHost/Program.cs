using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adhesive.ComponentPerformance.Core;
using Adhesive.Common;
using System.ServiceModel;

namespace Adhesive.ComponentPerformance.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Service.Start();
           
            Console.WriteLine("服务启动成功...");
            Console.ReadLine();
        }
    }
}
