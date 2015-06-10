using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Adhesive.Common;
using System.Linq;
namespace Adhesive.Config.Web
{
    public partial class EditConfigItem : System.Web.UI.Page
    {
        private string _appName = null;
        private string _parentPathItemNames = null;
        private string _actionType = null;
        private string _parentId = null;
        private ConfigItem _parentConfigItem = null;
        private string _id = null;
        private ConfigItem _configItem = null;
        private string _backUrl = null;
        private string _name = null;
        private string _friendlyName = null;
        private string _desc = null;
        private string _value = null;
        private string _sourceId = null;
        private string _valType = null;
        private string _valTypeEnum = null;
        private static readonly IConfigServer ConfigServer = ConfigServiceLocator.GetService();
        protected void Page_Load(object sender, EventArgs e)
        {
            GetParams();
            if (!ValidateParams())
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('参数存在异常！')", true);
                return;
            }
            if(!InitVariables())
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('初始化变量失败！')", true);
                return;
            }
            if (!IsPostBack)
            {
                Initialize();
            }
        }
        private void GetParams()
        {
            _appName = Request.QueryString["appName"];
            _parentPathItemNames = Request.QueryString["parentPathItemNames"];
            _id = Request.QueryString["id"];
            _parentId = Request.QueryString["parentId"];
            _actionType = Request.QueryString["type"];
        }
        private bool ValidateParams()
        {
            if (_actionType == "edit")
            {
                if (string.IsNullOrEmpty(_id))
                    return false;
            }
            return true;
        }
        private bool InitVariables()
        {
            if (_actionType == "edit")
            {
                _configItem = ConfigServer.GetConfigItem(_id);
                if (_configItem == null)
                    return false;
            }
            if (!string.IsNullOrEmpty(_parentId))
            {
                _parentConfigItem = ConfigServer.GetConfigItem(_parentId);
                if (_parentConfigItem == null)
                    return false;
            }
            return true;
        }
        private void Initialize()
        {

            if (_actionType == "edit")
            {
                txtName.Text = _configItem.Name;
                txtName.Enabled = false;
                txtFriendlyName.Text = _configItem.FriendlyName;
                txtDesc.Text = _configItem.Description;
                txtValue.Text = _configItem.Value;
                _sourceId = _configItem.SourceId;
                if (_sourceId != null)
                {
                    DataBindAssembly(_sourceId);
                    txtValue.Visible = false;
                    DataBindSource(ddlAssembly.SelectedValue,_sourceId);
                    if (!IsCompositeValue(_sourceId))
                    {
                        DataBindSourceItem(_sourceId, _configItem.Value);
                    }
                    else
                    {
                        DataBindCompositeSourceItem(_sourceId, _configItem.Value);
                    }
                }
                else
                {
                    DataBindAssembly(null);
                    DataBindSource(ddlAssembly.SelectedValue,null);
                }
            }
            else
            {
                DataBindAssembly(null);
                DataBindSource(ddlAssembly.SelectedValue,null);
            }
        }
        private void AddConfigItem()
        {
            try
            {
                if (string.IsNullOrEmpty(_parentId))
                {
                    ConfigServer.AddConfigItem(string.IsNullOrEmpty(_appName) ? null : _appName, string.IsNullOrEmpty(_parentPathItemNames) ? null : _parentPathItemNames.Split(','), _name, _friendlyName, _desc, _value, _sourceId, null, null,IsCompositeValue(_sourceId));
                    _backUrl = string.Format("ViewTopLevelConfigItemList.aspx?appName={0}", _appName);
                }
                else
                {
                    bool isEntityItem = false;
                    _backUrl = string.Format("ViewConfigItemList.aspx?appName={0}&parentPathItemNames={1}&prevParentId={2}&parentId={3}", _appName, _parentPathItemNames, _parentConfigItem.ParentId, _parentId);
                    //当上一级配置项为列表或字典类型，并且item为自定义实体类型时，当添加一个item时，若存在至少一个item，就复制第一个item。
                    if (_parentConfigItem.ValueTypeEnum != null && (_parentConfigItem.ValueTypeEnum == ValueTypeEnum.List.ToString() || _parentConfigItem.ValueTypeEnum == ValueTypeEnum.Dictionary.ToString()))
                    {
                        ConfigItem[] childConfigItems = ConfigServer.GetChildConfigItems(_parentId);
                        if (childConfigItems != null && childConfigItems.Length > 0)
                        {
                            if (childConfigItems[0].ValueTypeEnum == ValueTypeEnum.Entity.ToString())
                            {
                                _valTypeEnum = ValueTypeEnum.Entity.ToString();
                                _valType = childConfigItems[0].ValueType;
                                ConfigServer.AddConfigItem(string.IsNullOrEmpty(_appName) ? null : _appName, string.IsNullOrEmpty(_parentPathItemNames) ? null : _parentPathItemNames.Split(','), _name, _friendlyName, _desc, _value, _sourceId, _valType, _valTypeEnum, IsCompositeValue(_sourceId));
                                CopyConfigItemFromBrother(_appName, new List<string>(_parentPathItemNames.Split(',')) { _name }, new List<string>(_parentPathItemNames.Split(',')) { childConfigItems[0].Name });
                                isEntityItem = true;
                            }
                        }
                    }
                    if (!isEntityItem)
                    {
                        ConfigServer.AddConfigItem(string.IsNullOrEmpty(_appName) ? null : _appName, string.IsNullOrEmpty(_parentPathItemNames) ? null : _parentPathItemNames.Split(','), _name, _friendlyName, _desc, _value, _sourceId, _valType, _valTypeEnum, IsCompositeValue(_sourceId));
                    }
                }


            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('添加失败，请稍后再试！')", true);
                LocalLoggingService.Error(string.Format("添加配置项失败！异常：{0}", ex));
                return;
            }
            Response.Redirect(_backUrl);
        }
        private void CopyConfigItemFromBrother(string appName, List<string> pathItemNames, List<string> brotherPathItemNames)
        {
            string brotherId = ConfigHelper.GenerateConfigItemId(appName, brotherPathItemNames.ToArray());
            //获取兄弟节点下的所有节点
            ConfigItem[] childConfigItems = ConfigServer.GetChildConfigItems(brotherId);
            if (childConfigItems == null || childConfigItems.Length == 0) return;
            foreach (ConfigItem configItem in childConfigItems)
            {
                ConfigItem parentConfigItem = ConfigServer.AddConfigItem(string.IsNullOrEmpty(_appName) ? null : _appName, pathItemNames.ToArray(), configItem.Name, configItem.FriendlyName, configItem.Description, configItem.Value, configItem.SourceId, configItem.ValueType, configItem.ValueTypeEnum,configItem.IsCompositeValue);
                if (parentConfigItem != null)
                {
                    CopyConfigItemFromBrother(appName, new List<string>(pathItemNames) { configItem.Name }, new List<string>(brotherPathItemNames) { configItem.Name });
                }
            }
        }
        private void SaveConfigItem()
        {
            try
            {
               
                ConfigServer.SaveConfigItem(_id, _friendlyName, _desc, _value);
                if (string.IsNullOrEmpty(_parentId))
                {
                    _backUrl = string.Format("ViewTopLevelConfigItemList.aspx?appName={0}", _appName);
                }
                else
                {
                    ConfigItem parentConfigItem = ConfigServer.GetConfigItem(_parentId);
                    _backUrl = string.Format("ViewConfigItemList.aspx?appName={0}&parentPathItemNames={1}&prevParentId={2}&parentId={3}", _appName, _parentPathItemNames, parentConfigItem.ParentId, _parentId);
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('保存失败，请稍后再试！')", true);
                LocalLoggingService.Error(string.Format("保存配置项失败！异常：{0}", ex));
                return;
            }
            Response.Redirect(_backUrl);
        }
        private void GetInput()
        {
            _name = txtName.Text.Trim() == string.Empty ? null : txtName.Text.Trim();
            _friendlyName = txtFriendlyName.Text.Trim() == string.Empty ? null : txtFriendlyName.Text.Trim();
            _desc = txtDesc.Text.Trim() == string.Empty ? null : txtDesc.Text.Trim();
            _sourceId = ddlSource.SelectedValue;
            if (!string.IsNullOrEmpty(_sourceId))
            {
                ConfigItem sourceConfigItem = ConfigServer.GetConfigItem(_sourceId);
                if (sourceConfigItem != null)
                {
                    if (sourceConfigItem.ValueType == null && sourceConfigItem.ValueTypeEnum == null)
                    {
                        if (ddlSourceItem.Visible && !string.IsNullOrEmpty(ddlSourceItem.SelectedValue))
                        {
                            ConfigItem sourceItemConfigItem = ConfigServer.GetConfigItem(ddlSourceItem.SelectedValue);
                            _valType = sourceItemConfigItem.ValueType;
                            _valTypeEnum = sourceItemConfigItem.ValueTypeEnum;
                        }
                        if (chklSourceItem.Visible && chklSourceItem.Items.Count > 0)
                        {
                            ConfigItem sourceItemConfigItem = ConfigServer.GetConfigItem(chklSourceItem.Items[0].Value);
                            _valType = sourceItemConfigItem.ValueType;
                            _valTypeEnum = sourceItemConfigItem.ValueTypeEnum;
                        }
                    }
                    else
                    {
                        _valType = sourceConfigItem.ValueType;
                        _valTypeEnum = sourceConfigItem.ValueTypeEnum;
                    }
                }

                if (!IsCompositeValue(_sourceId))
                {
                    if (ddlSourceItem.Visible && !string.IsNullOrEmpty(ddlSourceItem.SelectedValue))
                    {
                        ConfigItem sourceItem = ConfigServer.GetConfigItem(ddlSourceItem.SelectedValue);
                        if (sourceItem != null)
                            _value = sourceItem.Value;
                    }
                    else
                        _value = txtValue.Text;
                }
                else
                {
                    List<string> valueList = new List<string>();
                    if (chklSourceItem.Visible)
                    {
                        if (chklSourceItem.Items.Count > 0)
                        {
                            foreach (ListItem item in chklSourceItem.Items)
                            {
                                if (item.Selected)
                                {
                                    ConfigItem configItem = ConfigServer.GetConfigItem(item.Value);
                                    if (configItem != null)
                                        valueList.Add(configItem.Value);
                                }
                            }
                        }
                        if (valueList.Count > 0)
                            _value = string.Join(",", valueList);
                        else
                            _value = txtValue.Text;
                    }
                    else
                    {
                        _value = txtValue.Text;
                    }
                }
            }
            else
            {
                _value = txtValue.Text;
            }

        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(_name))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('配置项名称不允许为空！')", true);
                return false;
            }
            string[] invalidNameTokens = { ",", "&" };
            foreach (string invalidNameToken in invalidNameTokens)
            {
                if (_name.Contains(invalidNameToken))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "error", string.Format("alert('配置项名称包含非法字符（{0}）！')", string.Join(" ", invalidNameTokens)), true);
                    return false;
                }
            }
            if (_actionType != "edit")
            {
                if (ConfigServer.IsConfigItemExists(_appName, string.Format("{0},{1}", _parentPathItemNames, _name).Split(',')))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('当前层级下存在同名配置项！')", true);
                    return false;
                }
                if (IsObjectItem() && string.IsNullOrEmpty(ddlSource.SelectedValue))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('配置项为Object类型，请指定一个具体的类型！')", true);
                    return false;
                }
            }
            return true;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            GetInput();
            if (!ValidateInput())
                return;
            if (_actionType == "edit")
            {
                SaveConfigItem();
            }
            else
            {
                AddConfigItem();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigServer.RemoveConfigItem(_id);
                if (string.IsNullOrEmpty(_parentId))
                {
                    _backUrl = string.Format("ViewTopLevelConfigItemList.aspx?appName={0}", _appName);
                }
                else
                {
                    ConfigItem parentConfigItem = ConfigServer.GetConfigItem(_parentId);
                    _backUrl = string.Format("ViewConfigItemList.aspx?appName={0}&parentPathItemNames={1}&prevParentId={2}&parentId={3}", _appName, _parentPathItemNames, parentConfigItem.ParentId, _parentId);
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('删除失败，请稍后再试！')", true);
                LocalLoggingService.Error(string.Format("删除配置项失败！异常：{0}", ex));
                return;
            }
            Response.Redirect(_backUrl);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_parentId))
            {
                _backUrl = string.Format("ViewTopLevelConfigItemList.aspx?appName={0}", _appName);
            }
            else
            {
                ConfigItem parentConfigItem = ConfigServer.GetConfigItem(_parentId);
                _backUrl = string.Format("ViewConfigItemList.aspx?appName={0}&parentPathItemNames={1}&prevParentId={2}&parentId={3}", _appName, _parentPathItemNames, parentConfigItem.ParentId, _parentId);
            }
            Response.Redirect(_backUrl);
        }
        private void DataBindAssembly(string sourceId)
        {
            string selectedAssemblyName = null;
            if (!string.IsNullOrEmpty(sourceId))
            {
                ConfigItem sourceConfigItem = ConfigServer.GetConfigItem(sourceId);
                if (sourceConfigItem != null)
                {
                    selectedAssemblyName = sourceConfigItem.Name.Contains(",") ? sourceConfigItem.Name.Split(',')[1] : "";
                }
            }
            ddlAssembly.Items.Clear();
            ddlAssembly.Items.Add(new ListItem("Global", ""));
            ConfigItem[] configItems = ConfigServer.GetChildConfigItems(null, Constants.TypeRepositoryItemName);
            Dictionary<string,string> dic = new Dictionary<string,string>();
            foreach (ConfigItem configItem in configItems)
            {
                if (configItem.Name.Contains(","))
                {
                    string assemblyName = configItem.Name.Split(',')[1];
                    if (!dic.ContainsKey(assemblyName))
                        dic.Add(assemblyName, assemblyName);
                }
            }
                       
            foreach (var  de in dic)
            {
                ListItem listItem = new ListItem(string.Format("{0}", de.Key), de.Value);
                ddlAssembly.Items.Add(listItem);
            }
            if (!string.IsNullOrEmpty(selectedAssemblyName))
            {
                ddlAssembly.SelectedIndex = ddlAssembly.Items.IndexOf(ddlAssembly.Items.FindByValue(selectedAssemblyName));
            }
            if (_actionType == "edit")
                ddlAssembly.Enabled = false;
        }
        private void DataBindSource(string assemblyName,string sourceId)
        {
            ddlSource.Items.Clear();
                ddlSource.Items.Add(new ListItem("", ""));
            ConfigItem[] configItems = ConfigServer.GetChildConfigItems(null, Constants.TypeRepositoryItemName);
            foreach (ConfigItem configItem in configItems)
            {
                if (string.IsNullOrEmpty(assemblyName))
                {
                    if (configItem.Name.Contains(","))
                        continue;
                }
                else if (configItem.Name.LastIndexOf(assemblyName) == -1)
                    continue;
                ListItem listItem = new ListItem(string.Format("{0}", configItem.FriendlyName), configItem.Id);
                ddlSource.Items.Add(listItem);
            }
            if (!string.IsNullOrEmpty(sourceId))
            {
                ddlSource.SelectedIndex = ddlSource.Items.IndexOf(ddlSource.Items.FindByValue(sourceId));
            }
            if (_actionType == "edit")
                ddlSource.Enabled = false;
        }
        private void DataBindSourceItem(string sourceId,string value)
        {
            ddlSourceItem.Visible = true;
            chklSourceItem.Visible = false;
            ddlSourceItem.Items.Clear();
            ConfigItem[] configItems = ConfigServer.GetChildConfigItems(sourceId);
            if (configItems == null || configItems.Length == 0)
            {
                ddlSourceItem.Visible = false;
                txtValue.Visible = true;
            }
            int i = 0;
            int selectedIndex = -1;
            foreach (ConfigItem configItem in configItems)
            {
                if (configItem.Value == null)
                    continue;
                ListItem listItem = new ListItem(string.Format("{0}", configItem.Name), configItem.Id);
                ddlSourceItem.Items.Add(listItem);
                if (configItem.Value == value)
                {
                    selectedIndex = i;
                }
                i++;
            }
            if (!string.IsNullOrEmpty(value) && selectedIndex != -1)
            {
                ddlSourceItem.SelectedIndex = selectedIndex;
            }
        }

        private void DataBindCompositeSourceItem(string sourceId,string value)
        {
            chklSourceItem.Visible = true;
            ddlSourceItem.Visible = false;
            List<string> valueList = new List<string>();
            if (!string.IsNullOrEmpty(value))
                valueList = value.Replace(" ","").Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).ToList();
            chklSourceItem.Items.Clear();
            ConfigItem[] configItems = ConfigServer.GetChildConfigItems(sourceId);
            if (configItems == null || configItems.Length == 0)
            {
                chklSourceItem.Visible = false;
                txtValue.Visible = true;
            }
            int i = 0;
            foreach (ConfigItem configItem in configItems)
            {
                if (configItem.Value == null)
                    continue;
                ListItem listItem = new ListItem(string.Format("{0}", configItem.Name), configItem.Id);
                chklSourceItem.Items.Add(listItem);
                if (valueList.Contains(configItem.Value))
                    chklSourceItem.Items[i].Selected = true;
                i++;
            }

        }

        protected void ddlAssembly_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBindSource(ddlAssembly.SelectedValue,null);
            ddlSource_SelectedIndexChanged(null,null);
        }

        protected void ddlSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isCompositeValue = false;
            string sourceId = null;
            if (!string.IsNullOrEmpty(ddlSource.SelectedValue))
            {
                sourceId = ddlSource.SelectedValue;
                isCompositeValue = IsCompositeValue(sourceId);
                if (!isCompositeValue)
                {
                    DataBindSourceItem(sourceId,null);
                    if (ddlSourceItem.Items.Count == 0)
                        ddlSourceItem.Visible = false;
                }
                else
                {
                    
                    DataBindCompositeSourceItem(sourceId,null);
                    if (chklSourceItem.Items.Count == 0)
                        chklSourceItem.Visible = false;
                }
                
            }
            else
            {
                ddlSourceItem.Visible = false;
                chklSourceItem.Visible = false;
            }
            if (!isCompositeValue)
                txtValue.Visible = !ddlSourceItem.Visible;
            else
                txtValue.Visible = !chklSourceItem.Visible;
        }
        /// <summary>
        /// 配置项的类型是否为object类型，只允许列表或字典的item为object类型，并且只能为基础数据类型。
        /// </summary>
        /// <returns></returns>
        private bool IsObjectItem()
        {
            if (string.IsNullOrEmpty(_parentId))
                return false;
            if (_parentConfigItem == null)
                return false;
            if (_parentConfigItem.ValueTypeEnum != null && (_parentConfigItem.ValueTypeEnum == ValueTypeEnum.ObjectItemList.ToString() || _parentConfigItem.ValueTypeEnum == ValueTypeEnum.ObjectItemDictionary.ToString()))
            {
                return true;
            }
            return false;
        }
        private bool IsCompositeValue(string sourceId)
        {
            if (string.IsNullOrEmpty(sourceId))
                return false;
            ConfigItem configItem = ConfigServer.GetConfigItem(sourceId);
            if (configItem != null && configItem.IsCompositeValue)
                return true;
            return false;
        }
    }
}