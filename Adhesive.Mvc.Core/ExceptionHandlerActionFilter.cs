
using System.Web.Mvc;
using Adhesive.AppInfoCenter;
using Adhesive.AppInfoCenter.Imp;

namespace Adhesive.Mvc.Core
{
    class ExceptionHandlerActionFilter : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            ((ExceptionService)AppInfoCenterService.ExceptionService).MvcUnhandledException(filterContext.Exception, filterContext.Controller.GetType().Name);
            filterContext.ExceptionHandled = !filterContext.HttpContext.Request.IsLocal;
        }
    }
}
