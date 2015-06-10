

using Adhesive.AppInfoCenter.Imp;
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.DistributedService.Imp
{
    public class RegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }

        public override string Description
        {
            get
            {
                return "注册Wcf定位服务";
            }
        }

        public override TaskContinuation Execute()
        {
            container.RegisterTypeAsSingleton<IWcfServiceLocator, WcfServiceLocator>();

            AppInfoCenterConfigurationDefaultConfig.RegisterIncludeInfoStrategyConfigurationItem(new IncludeInfoStrategyConfigurationItem
            {
                TypeFullName = typeof(ClientStartInfo).FullName,
                IncludeInfoStrategyName = "None",
            });

            AppInfoCenterConfigurationDefaultConfig.RegisterIncludeInfoStrategyConfigurationItem(new IncludeInfoStrategyConfigurationItem
            {
                TypeFullName = typeof(ServerStartInfo).FullName,
                IncludeInfoStrategyName = "None",
            });

            AppInfoCenterConfigurationDefaultConfig.RegisterIncludeInfoStrategyConfigurationItem(new IncludeInfoStrategyConfigurationItem
            {
                TypeFullName = typeof(ClientInvokeInfo).FullName,
                IncludeInfoStrategyName = "Simple",
            });

            AppInfoCenterConfigurationDefaultConfig.RegisterIncludeInfoStrategyConfigurationItem(new IncludeInfoStrategyConfigurationItem
            {
                TypeFullName = typeof(ServerInvokeInfo).FullName,
                IncludeInfoStrategyName = "Simple",
            });

            AppInfoCenterConfigurationDefaultConfig.RegisterIncludeInfoStrategyConfigurationItem(new IncludeInfoStrategyConfigurationItem
            {
                TypeFullName = typeof(ClientMessageInfo).FullName,
                IncludeInfoStrategyName = "Simple",
            });

            AppInfoCenterConfigurationDefaultConfig.RegisterIncludeInfoStrategyConfigurationItem(new IncludeInfoStrategyConfigurationItem
            {
                TypeFullName = typeof(ServerMessageInfo).FullName,
                IncludeInfoStrategyName = "Simple",
            });

            //AppInfoCenterConfigurationDefaultConfig.RegisterStateServiceConfigurationItem(new StateServiceConfigurationItem
            //{
            //    Enabled = true,
            //    TypeFullName = typeof(WcfServerStateInfo).FullName,
            //});

            //AppInfoCenterConfigurationDefaultConfig.RegisterStateServiceConfigurationItem(new StateServiceConfigurationItem
            //{
            //    Enabled = true,
            //    TypeFullName = typeof(WcfClientStateInfo).FullName,
            //});
            return TaskContinuation.Continue;
        }
    }
}
