
using System;
using Microsoft.Practices.Unity;

namespace Adhesive.Common
{
    public class AdhesiveFramework
    {
        private static readonly IUnityContainer container = new UnityContainer();
        private static Bootstrapper bootstrapper;
        private const string Version = "2012-06-01-08";//年-月-日-时

        public AdhesiveFramework()
        {
            Status = AdhesiveFrameworkStatus.NotStarted;
        }

        public static IUnityContainer Container
        {
            get
            {
                return container;
            }
        }

        public static AdhesiveFrameworkStatus Status { get; set; }


        public static void Start()
        {
            Status = AdhesiveFrameworkStatus.Starting;
            LocalLoggingService.Info("AdhesiveFramework开始启动...版本号：{0}", Version);
            bootstrapper = new Bootstrapper(container);
            if (bootstrapper.Execute())
            {
                Status = AdhesiveFrameworkStatus.Started;
                LocalLoggingService.Info("AdhesiveFramework启动完成！...版本号：{0}", Version);
            }
            else
            {
                Status = AdhesiveFrameworkStatus.FailedToStart;
                throw new Exception("AdhesiveFramework启动失败！");
            }           
        }

        public static void End()
        {
            Status = AdhesiveFrameworkStatus.Ending;
            LocalLoggingService.Info("AdhesiveFramework开始清理...");
            bootstrapper.Dispose();
            Status = AdhesiveFrameworkStatus.Ended;
            LocalLoggingService.Info("AdhesiveFramework清理完成！");
            LocalLoggingService.Close();
        }
    }
}
