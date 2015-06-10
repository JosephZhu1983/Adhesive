using System;
using Adhesive.Common;

namespace Adhesive.Mongodb.Silverlight.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AdhesiveFramework.Start();
        }

        protected void Application_End()
        {
            AdhesiveFramework.End();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}