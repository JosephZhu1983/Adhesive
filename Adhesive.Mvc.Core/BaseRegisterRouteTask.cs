

using Adhesive.Common;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
namespace Adhesive.Mvc.Core
{
    public class BaseRegisterRouteTask : InitServiceBootstrapperTask
    {
        public BaseRegisterRouteTask(IUnityContainer container) : base(container) { }

        protected virtual void Register(RouteCollection routes)
        {
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        public override TaskContinuation Execute()
        {
            var routes = container.Resolve<RouteCollection>();
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            Register(routes);
            return TaskContinuation.Continue;
        }
    }
}
