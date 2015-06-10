using System;
using System.Web.UI.WebControls;

namespace Adhesive.Config.Web
{
    public partial class ViewAppNameList : System.Web.UI.Page
    {
        private static readonly IConfigServer ConfigServer = ConfigServiceLocator.GetService();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBindTopLevelConfigList();
            }

        }
        private void DataBindTopLevelConfigList()
        {
            ddlTolLevelConfigItems.Items.Add(new ListItem("全局配置", "-1"));
            ConfigItem[] configItems = ConfigServer.GetChildConfigItems(null, Constants.AppNameListItemName);
            if (configItems == null)
                return;
            foreach (ConfigItem configItem in configItems)
            {
                ddlTolLevelConfigItems.Items.Add(new ListItem(configItem.FriendlyName, configItem.Name));
            }
            ddlTolLevelConfigItems.SelectedIndex = 0;
        }

        protected void btnViewConfigItems_Click(object sender, EventArgs e)
        {
            string appName = ddlTolLevelConfigItems.SelectedValue;
            if (string.IsNullOrEmpty(appName))
                return;
            Response.Redirect(string.Format("ViewTopLevelConfigItemList.aspx?appName={0}", appName));
        }

    }
}