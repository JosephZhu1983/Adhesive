using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Adhesive.AppInfoCenter;

namespace Adhesive.Test.WebApp
{
    public class Global : System.Web.HttpApplication
    {
        //private static IStateService testStateService;
        //public static int stateValue = 100;
        protected void Application_Start(object sender, EventArgs e)
        {
            Adhesive.Common.AdhesiveFramework.Start();
            //testStateService = AppInfoCenterService.StateService;
            //testStateService.Init(new StateServiceConfiguration("Adhesive.Test.WebApp.TestState", () =>
            //{
            //    return new List<BaseInfo> { new TestState
            //    {
            //        StateValue = stateValue,
            //    } };
            //}));
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
            Adhesive.Common.AdhesiveFramework.End();
        }
    }
}