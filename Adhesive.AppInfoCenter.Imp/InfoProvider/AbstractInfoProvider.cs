
using System;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace Adhesive.AppInfoCenter.Imp
{
    public class AbstractInfoProvider : IInfoProvider
    {
        internal static readonly string DefaultModuleName = "主模块";

        public void ProcessInfo(IncludeInfoStrategy strategy, AbstractInfo info)
        {
            if (info.ContextIdentity == null)
            {
                if (HttpContext.Current != null && HttpContext.Current.Items != null)
                {
                    if (HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextIdentityKey] == null)
                        HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextIdentityKey] = Guid.NewGuid().ToString();
                    info.ContextIdentity = HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextIdentityKey].ToString();
                }
                else
                {
                    if (CallContext.GetData(AppInfoCenterConfiguration.Const.ContextIdentityKey) == null)
                        CallContext.SetData(AppInfoCenterConfiguration.Const.ContextIdentityKey, Guid.NewGuid().ToString());
                    info.ContextIdentity = CallContext.GetData(AppInfoCenterConfiguration.Const.ContextIdentityKey).ToString();
                }
            }

            if (string.IsNullOrEmpty(info.ModuleName))
            {
                info.ModuleName = DefaultModuleName;
            }

            if (string.IsNullOrEmpty(info.CategoryName) && info.ModuleName == DefaultModuleName)
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Request != null
                    && HttpContext.Current.Request.Url != null
                    && HttpContext.Current.Request.Url.AbsolutePath != null)
                {
                    info.CategoryName = HttpContext.Current.Request.Url.AbsolutePath.ToLower();
                }
                else if (info.LocationInfo != null && !string.IsNullOrEmpty(info.LocationInfo.TypeName))
                {
                    info.CategoryName = info.LocationInfo.TypeName;
                }
            }

            if (HttpContext.Current != null
                    && HttpContext.Current.Request != null
                    && HttpContext.Current.Request.Url != null
                    && HttpContext.Current.Request.Url.Host != null)
                info.DomainName = HttpContext.Current.Request.Url.Host;
        }
    }
}
