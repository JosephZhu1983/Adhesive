

using Adhesive.Common;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Adhesive.AppInfoCenter.Imp;
namespace Adhesive.Mvc.Core
{
    public class RegisterDependencyResolverTask : InitServiceBootstrapperTask
    {
        public RegisterDependencyResolverTask(IUnityContainer container)
            : base(container)
        {

        }

        public override TaskContinuation Execute()
        {
            container.RegisterTypeAsSingleton<IDependencyResolver, UnityDependencyResolver>();
            DependencyResolver.SetResolver(container.Resolve<IDependencyResolver>());
            BaseService.RegisterExternalInfoProvider(new MvcContextInfoProvider());
            return TaskContinuation.Continue;
        }
    }
}
