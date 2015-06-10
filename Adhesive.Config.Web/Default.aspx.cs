using System;

namespace Adhesive.Config.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("ViewAppNameList.aspx");
        }
    }
}