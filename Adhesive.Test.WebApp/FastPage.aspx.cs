using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Adhesive.AppInfoCenter.Imp;

namespace Adhesive.Test.WebApp
{
    public partial class FastPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("<br/>" + Common.CommonConfiguration.GetConfig().ApplicationName);
            Response.Write("<br/>" + AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PagePerformanceServiceConfig.Enabled);
            Response.Write("<br/>" + AppInfoCenterConfiguration.GetConfig().CommonConfig.EmbedInfoToPage);

            var allow = AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PagePerformanceServiceConfig.AllowUrls;
            var deny = AppInfoCenterConfiguration.GetConfig().PerformanceServiceConfig.PagePerformanceServiceConfig.DenyUrls;

            Response.Write("<br/>" + allow.Count);
            Response.Write("<br/>" + deny.Count);
        }
    }
}