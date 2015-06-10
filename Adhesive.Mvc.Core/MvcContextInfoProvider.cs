
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Adhesive.AppInfoCenter.Imp;

namespace Adhesive.Mvc.Core
{
    public class MvcContextInfoProvider : IInfoProvider
    {
        public void ProcessInfo(IncludeInfoStrategy strategy, AbstractInfo info)
        {
            if (!strategy.IncludeInfoStrategyForMvcContextInfo.Include) return;

            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                var mvcContextInfo = new MvcContextInfo();
                ControllerContext controllerContext = null;
                var isMvcApp = false;
                var actionExecutedContext = httpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey]
                    as ActionExecutedContext;
                if (actionExecutedContext != null)
                {
                    isMvcApp = true;
                    mvcContextInfo.ControllerName = actionExecutedContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    mvcContextInfo.ActionName = actionExecutedContext.ActionDescriptor.ActionName;
                    if (actionExecutedContext.Result != null)
                        mvcContextInfo.ActionResultType = actionExecutedContext.Result.ToString();

                    if (actionExecutedContext.Controller != null)
                        controllerContext = actionExecutedContext.Controller.ControllerContext;

                    mvcContextInfo.State = MvcActionStage.ActionExecuted;
                }

                var actionExecutingContext = httpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey]
                    as ActionExecutingContext;
                if (actionExecutingContext != null)
                {
                    isMvcApp = true;
                    if (strategy.IncludeInfoStrategyForMvcContextInfo.IncludeParameterData)
                    {
                        var parameterData = new Dictionary<string, string>();
                        foreach (var key in actionExecutingContext.ActionParameters.Keys)
                        {
                            parameterData.Add(key, (actionExecutingContext.ActionParameters[key] ?? "").ToString());
                        }
                        mvcContextInfo.ParameterData = parameterData;
                    }

                    mvcContextInfo.ControllerName = actionExecutingContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    mvcContextInfo.ActionName = actionExecutingContext.ActionDescriptor.ActionName;
                    if (actionExecutingContext.Result != null)
                        mvcContextInfo.ActionResultType = actionExecutingContext.Result.ToString();

                    if (actionExecutingContext.Controller != null)
                        controllerContext = actionExecutingContext.Controller.ControllerContext;

                    mvcContextInfo.State = MvcActionStage.ActionExecuting;
                }

                var resultExecutedContext = httpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey]
                    as ResultExecutedContext;
                if (resultExecutedContext != null)
                {
                    isMvcApp = true;
                    if (resultExecutedContext.Result != null)
                        mvcContextInfo.ActionResultType = resultExecutedContext.Result.ToString();

                    if (resultExecutedContext.Controller != null)
                        controllerContext = resultExecutedContext.Controller.ControllerContext;

                    mvcContextInfo.State = MvcActionStage.ResultExecuted;
                }

                var resultExecutingContext = httpContext.Items[AppInfoCenterConfiguration.Const.ControllerContextIdentityKey]
                    as ResultExecutingContext;
                if (resultExecutingContext != null)
                {
                    isMvcApp = true;
                    if (actionExecutedContext.Result != null)
                        mvcContextInfo.ActionResultType = resultExecutingContext.Result.ToString();

                    if (resultExecutingContext.Controller != null)
                        controllerContext = resultExecutingContext.Controller.ControllerContext;

                    mvcContextInfo.State = MvcActionStage.ResultExecuting;
                }

                if (controllerContext != null)
                {
                    mvcContextInfo.IsChildAction = controllerContext.IsChildAction;

                    if (strategy.IncludeInfoStrategyForMvcContextInfo.IncludeViewData)
                    {
                        var viewData = new Dictionary<string, string>();
                        foreach (var key in controllerContext.Controller.ViewData.Keys)
                        {
                            viewData.Add(key, (controllerContext.Controller.ViewData[key] ?? "").ToString());
                        }
                        mvcContextInfo.ViewData = viewData;
                    }

                    if (strategy.IncludeInfoStrategyForMvcContextInfo.IncludeRouteData)
                    {
                        var routeData = new Dictionary<string, string>();
                        foreach (var key in controllerContext.RouteData.Values.Keys)
                        {
                            routeData.Add(key, (controllerContext.RouteData.Values[key] ?? "").ToString());
                        }
                        mvcContextInfo.RouteData = routeData;
                    }

                    if (strategy.IncludeInfoStrategyForMvcContextInfo.IncludeTempData)
                    {
                        var tempData = new Dictionary<string, string>();
                        foreach (var key in controllerContext.Controller.TempData.Keys)
                        {
                            tempData.Add(key, (controllerContext.Controller.TempData[key] ?? "").ToString());
                        }
                        mvcContextInfo.TempData = tempData;
                    }
                }

                if (isMvcApp)
                    info.MvcContextInfo = mvcContextInfo;
            }
        }
    }
}
