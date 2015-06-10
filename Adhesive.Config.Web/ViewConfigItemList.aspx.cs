using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Adhesive.Config.Web
{
    public partial class ViewConfigItemList : System.Web.UI.Page
    {
        private string _parentId = null;
        private string _prevParentId = null;
        private string _appName = null;
        private string _parentPathItemNames = null;
        private string _backUrl = null;
        private static readonly IConfigServer ConfigServer = ConfigServiceLocator.GetService();
        protected void Page_Load(object sender, EventArgs e)
        {
            GetParams();
            if (!ValidateParams())
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('参数存在异常！')", true);
                return;
            }
            GetBackUrl();
            GetLocation();
            if (!IsPostBack)
            {
                DataBindConfigItems();
            }
        }
        private void GetParams()
        {
            _parentId = Request.QueryString["parentId"];
            _prevParentId = Request.QueryString["prevParentId"];
            _appName = Request.QueryString["appName"];
            _parentPathItemNames = Request.QueryString["parentPathItemNames"];
        }
        private bool ValidateParams()
        {
            if (string.IsNullOrEmpty(_parentId))
                return false;
            if (string.IsNullOrEmpty(_parentPathItemNames))
                return false;
            return true;
        }
        private void GetBackUrl()
        {
            ConfigItem prevParentConfigitem = string.IsNullOrEmpty(_prevParentId)
                                                  ? null
                                                  : ConfigServer.GetConfigItem(_prevParentId);
            if (prevParentConfigitem == null)
                _backUrl = string.Format("ViewTopLevelConfigItemList.aspx?appName={0}", _appName);
            else
                _backUrl = string.Format("ViewConfigItemList.aspx?appName={0}&parentPathItemNames={1}&prevParentId={2}&parentId={3}", _appName, GetPrevParentPathItemNames(), prevParentConfigitem.ParentId, _prevParentId);
        }
        protected string CombinePathItemNames(string name)
        {
            return string.Format("{0},{1}", _parentPathItemNames, name);
        }
        protected string GetParentPathItemNames()
        {
            return _parentPathItemNames;
        }
        private string GetPrevParentPathItemNames()
        {
            int index = _parentPathItemNames.LastIndexOf(",");
            if (index < 0)
            {
                return null;
            }
            return _parentPathItemNames.Substring(0, index);
        }
        private void DataBindConfigItems()
        {
            ConfigItem[] configItems = ConfigServer.GetChildConfigItems(_parentId);
            dgConfigItems.DataSource = configItems;
            dgConfigItems.DataBind();

        }

        protected void btnGotoParent_Click(object sender, EventArgs e)
        {
            Response.Redirect(_backUrl);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string url = string.Format("EditConfigItem.aspx?appName={0}&parentPathItemNames={1}&parentId={2}", _appName, _parentPathItemNames, _parentId);
            Response.Redirect(url);
        }
        protected void GetLocation()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.IsNullOrEmpty(_appName) ? "全局配置" : _appName);
            if (!string.IsNullOrEmpty(_parentPathItemNames))
            {
                sb.Append(" > ");
                sb.Append(_parentPathItemNames.Replace(",", " > "));
            }
            lblLocation.Text = sb.ToString();
        }

        protected void dgConfigItems_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                e.Item.Attributes.Add("onmouseover", "this.style.backgroundColor='#ffb5ff'");
                e.Item.Attributes.Add("onmouseout", "this.style.backgroundColor='#FFFFFF'");
            }
        }

        protected void btnGotoRoot_Click(object sender, EventArgs e)
        {
            string url = string.Format("ViewTopLevelConfigItemList.aspx?appName={0}", _appName);
            Response.Redirect(url);
        }
    }
}