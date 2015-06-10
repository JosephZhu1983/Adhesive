using System;

using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.Mongodb.Imp
{
    public class InitServiceTask : InitServiceBootstrapperTask
    {
        //private static IStateService stateService;
        public InitServiceTask(IUnityContainer container)
            : base(container)
        {
        }

        public override string Description
        {
            get
            {
                return "初始化Mongodb客户端服务";
            }
        }

        public override TaskContinuation Execute()
        {
            try
            {
                //stateService = LocalServiceLocator.GetService<IStateService>();
                //stateService.Init(new StateServiceConfiguration(typeof(MongodbServiceStateInfo).FullName, MongodbInsertService.GetMemoeyQueueState));
                return TaskContinuation.Continue;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("初始化Mongodb客户端出错，异常信息：{0}", ex);
                return TaskContinuation.Break;
            }
        }
    }
}
