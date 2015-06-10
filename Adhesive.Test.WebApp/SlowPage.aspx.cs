using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace Adhesive.Test.WebApp
{
    public partial class SlowPage : System.Web.UI.Page
    {
        private static Random r = new Random();
        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.Sleep(r.Next(100, 200));
        }
    }
}