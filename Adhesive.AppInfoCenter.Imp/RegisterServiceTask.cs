
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.AppInfoCenter.Imp
{
    public class RegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }

        public override string Description
        {
            get
            {
                return "注册日志、异常、性能、状态服务";
            }
        }
        public override TaskContinuation Execute()
        {
            container.RegisterTypeAsSingleton<ILoggingService, LoggingService>();
            container.RegisterTypeAsSingleton<IExceptionService, ExceptionService>();
            container.RegisterTypeAsSingleton<ICodePerformanceService, CodePerformanceService>();
            container.RegisterTypeAsTransient<IStateService, StateService>();

            return TaskContinuation.Continue;
        }

        protected override void InternalDispose()
        {
            container.Resolve<ILoggingService>().Dispose();
            container.Resolve<IExceptionService>().Dispose();
            container.Resolve<ICodePerformanceService>().Dispose();
            container.Resolve<IStateService>().Dispose();
        }
    }
}
