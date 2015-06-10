using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Adhesive.Common;

namespace Adhesive.Mongodb.Client.UI.Web
{
    public partial class Demo : System.Web.UI.Page
    {
        private static IMongodbQueryService service = LocalServiceLocator.GetService<IMongodbQueryService>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var categoryData = service.GetCategoryData();
                categoryData.ForEach(category =>
                {
                    Category.Items.Add(new ListItem(category.Name));
                });
                ViewState["Category"] = categoryData;
                Category.Items.Insert(0, new ListItem(""));
                EndTime.Text = DateTime.Now.ToShortDateString();
                StartTime.Text = DateTime.Now.AddMonths(-1).AddDays(1).ToShortDateString();
            }
        }

        protected void Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Category.SelectedValue)) return;

            var categoryData = ViewState["Category"] as List<Category>;
            Database.Items.Clear();
            categoryData.Single(c => c.Name == Category.SelectedValue).SubCategoryList.ForEach(sub =>
            {
                Database.Items.Add(new ListItem(sub.DisplayName, sub.Name));
            });

            Database.Items.Insert(0, new ListItem(""));
        }

        protected void Database_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Database.SelectedValue)) return;
            var categoryData = ViewState["Category"] as List<Category>;
            var sub = categoryData.SelectMany(c => c.SubCategoryList).Where(sc => sc.Name == Database.SelectedValue).SingleOrDefault();
            if (sub == null) return;

            TableNames.Items.Clear();
            sub.TableNames.ForEach(name =>
            {
                TableNames.Items.Add(new ListItem(name) { Selected = false });
            });
            TableNames.Items.Insert(0, new ListItem(""));
        }

        protected void Query_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Database.SelectedValue)) return;

            if (string.IsNullOrEmpty(TableNames.SelectedValue)) return;

            var categoryData = ViewState["Category"] as List<Category>;
            var sub = categoryData.SelectMany(c => c.SubCategoryList).Where(sc => sc.Name == Database.SelectedValue).SingleOrDefault();
            if (sub == null) return;

            var data = service.GetTableData(sub.DatabasePrefix, new List<string> {  TableNames.SelectedValue }, DateTime.Parse(StartTime.Text), DateTime.Parse(EndTime.Text), 0, 10, null).First();

            var pkColumn = "";
            var dataTable = new DataTable(data.TableName);
            if (data.Data.Count > 0)
            {
                foreach (var column in data.Data.OrderByDescending(item => item.Values.Count).First())
                {
                    if (column.Key.StartsWith("#"))
                        pkColumn = column.Key;
                    dataTable.Columns.Add(column.Key);
                }

                foreach (var row in data.Data)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var column in row)
                    {
                        dataRow[column.Key] = column.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            GridView1.Columns.Clear();
            GridView1.DataSource = dataTable;

            HyperLinkField objHC = new HyperLinkField();
            objHC.DataNavigateUrlFields = new string[] { pkColumn };
            objHC.DataTextField = pkColumn;
            objHC.DataNavigateUrlFormatString = "Detail.aspx?Id={0}" + "&DatabaseName=" + data.DatabaseName + "&TableName=" + data.TableName + "&pkColumnName=" + pkColumn.TrimStart('#') + "&databasePrefix=" + sub.DatabasePrefix;
            GridView1.Columns.Add(objHC);

            GridView1.DataBind();

        }

        protected void TableNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TableNames.SelectedValue)) return;
            var categoryData = ViewState["Category"] as List<Category>;
            var sub = categoryData.SelectMany(c => c.SubCategoryList).Where(sc => sc.Name == Database.SelectedValue).SingleOrDefault();
            if (sub == null) return;


            var filters = service.GetFilterData(sub.DatabasePrefix, new List<string> { TableNames.SelectedValue } , DateTime.Parse(StartTime.Text), DateTime.Parse(EndTime.Text));
            ViewState["Filter"] = filters;
            filters.ForEach(filter =>
            {
                var normal = filter as ListFilter;
                if (normal != null)
                {
                    PlaceHolder1.Controls.Add(new Label
                    {
                        Text = normal.DisplayName,
                    });

                    PlaceHolder1.Controls.Add(new Label
                    {
                        Text = "<br/>",
                    });

                    switch (normal.FilterType)
                    {
                        case ListFilterType.TextBoxFilter:
                            {
                                PlaceHolder1.Controls.Add(new TextBox
                                {
                                    ID = normal.ColumnName,
                                });
                                break;
                            }
                        case ListFilterType.DropDownListFilter:
                            {
                                var ddl = new DropDownList();
                                ddl.ID = normal.ColumnName;
                                normal.Items.ForEach(a =>
                                {
                                    ddl.Items.Add(new ListItem(a.Name, a.Value.ToString()));
                                });
                                ddl.Items.Insert(0, new ListItem(""));
                                PlaceHolder1.Controls.Add(ddl);
                                break;
                            }
                        case ListFilterType.CheckBoxListFilter:
                            {
                                var cbl = new CheckBoxList() { RepeatLayout = RepeatLayout.Flow };
                                cbl.ID = normal.ColumnName;
                                normal.Items.ForEach(a =>
                                {
                                    cbl.Items.Add(new ListItem(a.Name, a.Value.ToString()) { Selected = true });
                                });
                                PlaceHolder1.Controls.Add(cbl);
                                break;
                            }
                    }
                    if (!string.IsNullOrEmpty(normal.Description))
                    {
                        PlaceHolder1.Controls.Add(new Label
                        {
                            Text = string.Format("({0})", normal.Description),
                        });
                    }

                    PlaceHolder1.Controls.Add(new Label
                    {
                        Text = "<br/>",
                    });
                }

                var cascade = filter as CascadeFilter;
                if (cascade != null)
                {
                    PlaceHolder2.Controls.Add(new Label
                    {
                        Text = cascade.DisplayName,
                    });
                    PlaceHolder2.Controls.Add(new Label
                    {
                        Text = "<br/>",
                    });
                    switch (cascade.FilterType)
                    {
                        case CascadeFilterType.LevelOne:
                            {
                                var ddl = new DropDownList();
                                ddl.ID = cascade.ColumnName;
                                foreach (var a in cascade.Items)
                                {
                                    ddl.Items.Add(a.Key);
                                }
                                ddl.Items.Insert(0, new ListItem(""));
                                PlaceHolder2.Controls.Add(ddl);
                                break;
                            }
                        case CascadeFilterType.LevelTwo:
                            {
                                var ddl = new DropDownList();
                                ddl.ID = cascade.ColumnName;
                                foreach (var a in cascade.Items)
                                {
                                    ddl.Items.Add("--" + a.Key);
                                    cascade.Items[a.Key].ForEach(b => ddl.Items.Add(b));
                                }
                                ddl.Items.Insert(0, new ListItem(""));
                                PlaceHolder2.Controls.Add(ddl);
                                break;
                            }
                        case CascadeFilterType.LevelThree:
                            {
                                var ddl = new DropDownList();
                                ddl.ID = cascade.ColumnName;
                                foreach (var a in cascade.Items)
                                {
                                    ddl.Items.Add("--" + a.Key);
                                    cascade.Items[a.Key].ForEach(b => ddl.Items.Add(b));
                                }
                                ddl.Items.Insert(0, new ListItem(""));
                                PlaceHolder2.Controls.Add(ddl);
                                break;
                            }
                    }
                    if (!string.IsNullOrEmpty(cascade.Description))
                    {
                        PlaceHolder2.Controls.Add(new Label
                        {
                            Text = string.Format("({0})", cascade.Description),
                        });
                    }

                    PlaceHolder2.Controls.Add(new Label
                    {
                        Text = "<br/>",
                    });
                }
            });
        }

    }
}