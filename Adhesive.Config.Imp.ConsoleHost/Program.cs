using System;
using Adhesive.Common;

namespace Adhesive.Config.Imp.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            AdhesiveFramework.Start();
            Console.ReadLine();
            AdhesiveFramework.End();

        }
    }
}
