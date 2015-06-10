
using System.Web.Mvc;
using Adhesive.AppInfoCenter.Imp;

namespace Adhesive.Mvc.Core
{
    class CrossCuttingActionFilter : ActionFilterAttribute, IActionFilter, IResultFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey] = filterContext;
            base.OnActionExecuted(filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey] = filterContext;
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey] = filterContext;
            base.OnResultExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey] = filterContext;
            base.OnResultExecuting(filterContext);
        }
    }
}
