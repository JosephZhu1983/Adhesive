using System;
using Adhesive.Common;
using System.ServiceModel;

namespace Adhesive.Mongodb.Server.Imp.ConsoleHost
{
    class Program
    {
        private static ServiceHost _host;
        static void Main(string[] args)
        {
            AdhesiveFramework.Start();

            LocalLoggingService.Info("开始启动Mongodb数据服务");
            try
            {
                _host = new ServiceHost(typeof(MongodbServer));
                _host.Open();
                LocalLoggingService.Info("完成启动Mongodb数据服务");
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("Mongodb数据服务启动失败，异常信息：{0}", ex);
            }
            Console.WriteLine("Adhesive.Mongodb.Server started...");
            Console.ReadLine();

            _host.Close();
            LocalLoggingService.Info("结束Mongodb数据服务");
            AdhesiveFramework.End();
        }
    }
}
