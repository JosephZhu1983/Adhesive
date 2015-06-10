

using System;
using Adhesive.Common;
using Microsoft.Practices.Unity;
namespace Adhesive.Mongodb.Server.Imp
{
    public class InitServerTask : BootstrapperTask
    {
        //private static IStateService stateService;
        public InitServerTask(IUnityContainer container)
            : base(container)
        {
        }

        public override int Order
        {
            get
            {
                return 4;
            }
        }
        public override string Description
        {
            get
            {
                return "初始化Mongodb服务端、进行第一次数据库元数据维护";
            }
        }

        public override TaskContinuation Execute()
        {
            try
            {
                MongodbServerMaintainceCenter.Init();
                //stateService = LocalServiceLocator.GetService<IStateService>();
                //stateService.Init(new StateServiceConfiguration(typeof(MongodbServerStateInfo).FullName, MongodbServer.GetState));
                return TaskContinuation.Continue;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("初始化Mongodb服务端出错，异常信息：{0}", ex);
                return TaskContinuation.Break;
            }
        }
    }
}
