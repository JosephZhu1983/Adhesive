using System;
using Adhesive.Common;

namespace Adhesive.Mongodb.Client.UI.Web
{
    public partial class Detail : System.Web.UI.Page
    {
        private static IMongodbQueryService service = LocalServiceLocator.GetService<IMongodbQueryService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var data = service.GetDetailData(Request.QueryString["databasePrefix"],
                Request.QueryString["databasename"],
                Request.QueryString["tablename"],
                Request.QueryString["pkcolumnname"],
                Request.QueryString["id"]);

            Response.Write(data);
        }
    }
}