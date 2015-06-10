using System;
using System.Web.UI.WebControls;

namespace Adhesive.Config.Web
{
    public partial class ViewTopLevelConfigItemList : System.Web.UI.Page
    {
        private string _appName = string.Empty;
        private static readonly IConfigServer ConfigServer = ConfigServiceLocator.GetService();
        protected void Page_Load(object sender, EventArgs e)
        {
            _appName = Request.QueryString["appName"];
            if (string.IsNullOrEmpty(_appName) || _appName == "-1")
                _appName = null;
            GetLocation();
            if (!IsPostBack)
            {
                DataBindTopLevelConfigItemList();
            }
        }
        private void DataBindTopLevelConfigItemList()
        {
            ConfigItem[] configItems = ConfigServer.GetTopLevelConfigItems(_appName);
            dgTopLevelConfigItems.DataSource = configItems;
            dgTopLevelConfigItems.DataBind();
        }

        protected void btnGotoParent_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewAppNameList.aspx");
        }
        protected void GetLocation()
        {
            lblLocation.Text = _appName == null ? "全局配置" : _appName;
        }

        protected void dgTopLevelConfigItems_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                e.Item.Attributes.Add("onmouseover", "this.style.backgroundColor='#ffb5ff'");
                e.Item.Attributes.Add("onmouseout", "this.style.backgroundColor='#FFFFFF'");
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string url = string.Format("EditConfigItem.aspx?appName={0}&parentPathItemNames={1}&parentId={2}", _appName, null, null);
            Response.Redirect(url);
        }
    }
}