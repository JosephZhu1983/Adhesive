using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Adhesive.Common;

namespace Adhesive.AppInfoCenter.Imp
{
    public class SpiderExceptionFilter : IUnhandledExceptionFilter
    {
        public bool DoFilter(HttpContext httpContext)
        {
            var config = AppInfoCenterConfiguration.GetConfig().ExceptionServiceConfig.UnhandledExceptionFilterConfig;
            if (config.SpiderExceptionFilterConfig.Enabled)
            {
                var spiderIdList = config.SpiderExceptionFilterConfig.SpiderIdList;
                if (spiderIdList != null && spiderIdList.Count > 0)
                {
                    foreach (var spiderId in spiderIdList)
                    {
                        if (!string.IsNullOrEmpty(spiderId) && httpContext.Request.UrlReferrer == null && httpContext.Request.UserAgent.ToLower().Contains(spiderId.ToLower()))
                        {
                            LocalLoggingService.Debug("SpiderExceptionFilter catched.SpiderId:" + spiderId);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
