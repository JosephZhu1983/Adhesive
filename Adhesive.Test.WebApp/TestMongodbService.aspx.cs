using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Adhesive.Mongodb;

namespace Adhesive.Test.WebApp
{
    public partial class TestMongodbService : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                showdata();
            }
        }

        protected void showdata()
        {
            List<Category> ls = MongodbService.MongodbQueryService.GetCategoryData();
            DropDownList1.DataSource = ls;
            DropDownList1.DataTextField = "Name";
            DropDownList1.DataValueField = "Name";
            DropDownList1.DataBind();
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList2.Items.Clear();
            DropDownList2.Items.Add(new ListItem("请选择", "-1"));
            List<Category> ls = MongodbService.MongodbQueryService.GetCategoryData();
            foreach (Category cg in ls)
            {
                if (DropDownList1.SelectedValue == cg.Name)
                {
                    DropDownList2.DataSource = cg.SubCategoryList;
                    DropDownList2.DataTextField = "DisplayName";
                    DropDownList2.DataValueField = "Name";
                    DropDownList2.DataBind();
                }
            }
        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioButtonList1.Items.Clear();
            List<Category> ls = MongodbService.MongodbQueryService.GetCategoryData();
            foreach (Category cg in ls)
            {
                if (DropDownList1.SelectedValue == cg.Name)
                {
                    foreach (SubCategory sc in cg.SubCategoryList)
                    {
                        if (DropDownList2.SelectedValue == sc.Name)
                        {
                            foreach (string tablename in sc.TableNames)
                            {
                                RadioButtonList1.Items.Add(new ListItem(tablename));
                            }
                            if (RadioButtonList1.Items.Count > 0)
                            {
                                RadioButtonList1.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder("");
            List<TableData> collections = MongodbService.MongodbQueryService.GetTableData(string.Format("{0}__{1}", DropDownList1.SelectedValue, DropDownList2.SelectedValue), new List<string>() { RadioButtonList1.SelectedValue }, DateTime.Now.AddDays(-int.Parse(TextBox1.Text)), DateTime.Now.AddMinutes(10), 0, 10, null);
            sb.Append(string.Format("databasePrifix:{0} tablename:{1} beginTime:{2} endTime:{3} ", DropDownList1.SelectedValue + "__" + DropDownList2.SelectedValue, RadioButtonList1.SelectedValue, DateTime.Now.AddDays(-int.Parse(TextBox1.Text)), DateTime.Now.AddMinutes(10)));
            if (collections != null)
            {
                foreach (TableData td in collections)
                {
                    sb.Append(string.Format("TableName:{0} PkColumnDisplayName:{1} PkColumnName:{2}<br/>", td.TableName, td.PkColumnDisplayName, td.PkColumnName));
                    foreach (var t in td.Tables)
                    {
                        int line = 0;
                        foreach (var b in t.Data)
                        {
                            line += 1;
                            sb.Append("----第" + line + "行------------------------------------------------------------<br/>");
                            foreach (KeyValuePair<string, string> kvp in b)
                            {
                                sb.Append(string.Format("key={0},value={1}<br/>", kvp.Key, kvp.Value));
                            }
                        }
                    }
                }
            }
            Literal2.Text = sb.ToString();
        }
    }
}