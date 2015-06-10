
using System;

using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.Alarm
{
    public class InitServiceTask : InitServiceBootstrapperTask
    {
        public override string Description
        {
            get
            {
                return "初始化报警服务";
            }
        }

        public InitServiceTask(IUnityContainer container)
            : base(container)
        {
        }

        public override TaskContinuation Execute()
        {
            try
            {
                AlarmService.Init();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("初始化报警服务失败，异常信息：{0}", ex);
                return TaskContinuation.Break;
            }
            return TaskContinuation.Continue;
        }

        protected override void InternalDispose()
        {
            AlarmService.Dispose();
        }
    }
}
