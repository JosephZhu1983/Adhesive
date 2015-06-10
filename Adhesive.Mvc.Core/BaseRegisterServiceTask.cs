
using Adhesive.Common;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace Adhesive.Mvc.Core
{
    public class BaseRegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public BaseRegisterServiceTask(IUnityContainer container) : base(container) { }

        public override string Description
        {
            get
            {
                return "注入Mvc内部的一些类型";
            }
        }

        protected virtual void Register(IUnityContainer container)
        {

        }

        public override TaskContinuation Execute()
        {
            container.RegisterInstanceAsSingleton<GlobalFilterCollection>(GlobalFilters.Filters);
            container.RegisterInstanceAsSingleton<RouteCollection>(RouteTable.Routes);
            container.RegisterInstanceAsSingleton<ControllerBuilder>(ControllerBuilder.Current);
            container.RegisterInstanceAsSingleton<ValueProviderFactoryCollection>(ValueProviderFactories.Factories);
            container.RegisterInstanceAsSingleton<ModelBinderDictionary>(ModelBinders.Binders);
            AreaRegistration.RegisterAllAreas();
            Register(container);
            return TaskContinuation.Continue;
        }

        protected override void InternalDispose()
        {
        }
    }
}
