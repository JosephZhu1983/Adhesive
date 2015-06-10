using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Adhesive.Mongodb.Silverlight.Service;
using System.IO;
using Polenter.Serialization;
using System.Windows.Browser;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class ViewListData : ChildWindow
    {
        public SearchCondition SearchCondition { get; set; }
        private static readonly string PageSizeKey = "PAGESIZE";
        //选择的分页索引号，根据表名分开存放
        private Dictionary<string, int> pageIndex = new Dictionary<string, int>();
        //选择的tab页索引号
        private int tabIndex = 0;
        //判断是否是在加载数据
        private bool working = false;

        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

        public ViewListData()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 获取分页大小
        /// </summary>
        /// <returns></returns>
        private int GetPageSize()
        {
            var pageSize = 0;
            if (int.TryParse(PageSize.Text, out pageSize))
            {
                if (pageSize > 0 && pageSize < 100)
                    return pageSize;
                else
                {
                    MessageBox.Show("每页显示记录数必须介于0到100之间！");
                    PageSize.Focus();
                    return -1;
                }
            }
            else
            {
                MessageBox.Show("每页显示记录数必须是数字！");
                PageSize.Focus();
                return -1;
            }
        }

        private void service_GetTableDataCompleted(object sender, GetTableDataCompletedEventArgs e)
        {
            Refresh.IsEnabled = true;
            this.Busy.IsBusy = false;

            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }
                

            if (e.Result != null)
            {
                working = true;

                var tableData = e.Result.ToList();

                var tableName = e.UserState as string;
                if (string.IsNullOrEmpty(tableName))
                    ListGridTab.Items.Clear();

                foreach (var table in tableData.OrderBy(t => t.TableName))
                {
                    //数据
                    var data = new List<Dictionary<string, string>>();
                    //数据量
                    //var count = 0;
                    //把所有数据库的数据组合在一起，如果跨库可能就会有多个数据
                    foreach (var t in table.Tables)
                    {
                        data.AddRange(t.Data);
                        //count += t.TotalCount;
                    }
                    //字典变为动态类型集合
                    var enties = (data as IEnumerable<Dictionary<string, string>>).ToDataSource();
                    if (enties != null)
                    {
                        //是否传入了表名，在异步调用方法的时候作为状态信息传入的参数
                        if (string.IsNullOrEmpty(tableName))
                        {
                            var tab = new TabItem
                            {
                                Header = table.TableName,
                                Tag = table.TableName,
                            };
                            if (data.Count > 0)
                            {
                                var panel = new StackPanel
                                {
                                    Orientation = Orientation.Vertical,
                                };
                                var pager2Panel = new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                                };
                                var pagershow = new TextBlock
                                {
                                    Text = "第 1 页",
                                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                };
                                var pagertop = new Button
                                {
                                    Content = "首页",
                                    Tag = table.TableName,
                                    IsEnabled = false,
                                    Margin = new Thickness(20, 0, 0, 0),
                                };
                                var pagerup = new Button
                                {
                                    Content = "上一页",
                                    Tag = table.TableName,
                                    Margin = new Thickness(20, 0, 0, 0),
                                    IsEnabled = false,
                                };
                                var pagerdown = new Button
                                {
                                    Content = "下一页",
                                    Tag = table.TableName,
                                    Margin = new Thickness(20, 0, 0, 0),
                                };

                                pager2Panel.Children.Add(pagershow);
                                pager2Panel.Children.Add(pagertop);
                                pager2Panel.Children.Add(pagerup);
                                pager2Panel.Children.Add(pagerdown);

                                pagerdown.Click += new RoutedEventHandler(pagerdown_Click);
                                pagerup.Click += new RoutedEventHandler(pagerup_Click);
                                pagertop.Click += new RoutedEventHandler(pagertop_Click);

                                var grid = new DataGrid()
                                {
                                    AutoGenerateColumns = true,
                                    ItemsSource = enties,
                                    SelectionMode = DataGridSelectionMode.Single,
                                    Tag = table,
                                    IsReadOnly = false,
                                    MaxColumnWidth = 600,
                                    MaxHeight = 600,
                                };
                                grid.BeginEdit();

                                //再是表格
                                panel.Children.Add(grid);
                                //最后是下面的分页
                                panel.Children.Add(pager2Panel);
                                tab.Content = panel;
                            }
                            ListGridTab.Items.Add(tab);
                        }
                        //查看到最后一页时,提示并使按键灰掉
                        else
                        {
                            if (data.Count == 0)
                            {
                                MessageBox.Show("已是最后一页");
                                var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == tableName.ToString());
                                var panel = tab.Content as StackPanel;
                                panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                                {
                                    member.Children.OfType<Button>().ToList().ForEach(a =>
                                    {
                                        if (a.Content.ToString() == "下一页")
                                        {
                                            a.IsEnabled = false;
                                        }
                                        if (pageIndex[tableName] == 1)
                                        {
                                            if (a.Content.ToString() == "首页")
                                            {
                                                a.IsEnabled = false;
                                            }
                                            if (a.Content.ToString() == "上一页")
                                            {
                                                a.IsEnabled = false;
                                            }
                                        }
                                    });
                                });
                                pageIndex[tableName] = pageIndex[tableName] - 1;
                            }
                            else
                            {    
                                //找到标签页
                                var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == table.TableName);
                                //找到原先的翻页索引号
                                var index = 0;
                                if (!string.IsNullOrEmpty(table.TableName) && pageIndex.ContainsKey(table.TableName))
                                    index = pageIndex[table.TableName];
                                var panel = tab.Content as StackPanel;
                                if (panel != null)
                                {
                                    //分页控件的状态重置为原先选择的
                                    panel.Children.OfType<StackPanel>().SelectMany(p => p.Children).OfType<DataPager>()
                                        .ToList().ForEach(p => p.PageIndex = index);
                                    //数据重新绑定
                                    var grid = panel.Children.OfType<DataGrid>().FirstOrDefault();
                                    if (grid != null)
                                    {
                                        grid.ItemsSource = enties;
                                        grid.Tag = table;
                                    }

                                    //显示页码
                                    panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                                    {
                                        member.Children.OfType<TextBlock>().ToList().ForEach(a =>
                                        {
                                            a.Text = string.Format("第 {0} 页", pageIndex[tableName] + 1);
                                        });
                                    });
                                }

                            }
                        }
                    }
                }

                ListGridTab.SelectedIndex = tabIndex;
                working = false;
            }

            if (SearchCondition.SelectedTableName != null)
            {
                var tab = ListGridTab.Items.OfType<TabItem>().FirstOrDefault(t => t.Tag.ToString() == SearchCondition.SelectedTableName);
                if (tab != null) ListGridTab.SelectedItem = tab;
            }
        }

        //选择首页
        void pagertop_Click(object sender, RoutedEventArgs e)
        {
            if (working) return;

            var pager = sender as Button;
            if (pager != null)
            {
                //var index = pager.PageIndex;
                if (!string.IsNullOrEmpty(pager.Tag as string))
                {
                    var tableName = pager.Tag.ToString();
                    //把翻页的数据保存起来
                    if (pageIndex[tableName] != 0)
                    {
                        if (!pageIndex.ContainsKey(tableName))
                        {
                            pageIndex.Add(tableName, 0);
                        }
                        pageIndex[tableName] = 0;
                        var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == pager.Tag.ToString());
                        var panel = tab.Content as StackPanel;
                        panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                        {
                            member.Children.OfType<Button>().ToList().ForEach(a =>
                            {
                                if (a.Content.ToString() == "首页")
                                {
                                    a.IsEnabled = false;
                                }
                                if (a.Content.ToString() == "上一页")
                                {
                                    a.IsEnabled = false;
                                }
                                if (a.Content.ToString() == "下一页")
                                {
                                    a.IsEnabled = true;
                                }
                            });
                        });
                        ShowData(tableName);
                    }
                }
            }
        }

        //选择上页
        void pagerup_Click(object sender, RoutedEventArgs e)
        {
            if (working) return;

            var pager = sender as Button;
            if (pager != null)
            {
                //var index = pager.PageIndex;
                if (!string.IsNullOrEmpty(pager.Tag as string))
                {
                    var tableName = pager.Tag.ToString();
                    //把翻页的数据保存起来                   

                    pageIndex[tableName] = pageIndex[tableName] - 1;
                    var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == pager.Tag.ToString());
                    var panel = tab.Content as StackPanel;
                    panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                    {
                        member.Children.OfType<Button>().ToList().ForEach(a =>
                        {
                            if (pageIndex[tableName] == 0)
                            {
                                if (a.Content.ToString() == "首页")
                                {
                                    a.IsEnabled = false;
                                }
                                if (a.Content.ToString() == "上一页")
                                {
                                    a.IsEnabled = false;
                                }
                            }
                            if (a.Content.ToString() == "下一页")
                            {
                                a.IsEnabled = true;
                            }
                        });
                    });
                    if (!pageIndex.ContainsKey(tableName))
                    {
                        pageIndex.Add(tableName, 0);
                    }
                    ShowData(tableName);
                }
            }
        }

        //选择下页
        void pagerdown_Click(object sender, RoutedEventArgs e)
        {
            if (working) return;

            var pager = sender as Button;
            if (pager != null)
            {
                //var index = pager.PageIndex;
                if (!string.IsNullOrEmpty(pager.Tag as string))
                {
                    var tableName = pager.Tag.ToString();
                    //把翻页的数据保存起来
                    if (!pageIndex.ContainsKey(tableName))
                    {
                        pageIndex.Add(tableName, 0);
                    }
                    pageIndex[tableName] = pageIndex[tableName] + 1;
                    var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == pager.Tag.ToString());
                    var panel = tab.Content as StackPanel;
                    panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                        {
                            member.Children.OfType<Button>().ToList().ForEach(a =>
                                {
                                    if (a.Content.ToString() == "首页")
                                    {
                                        a.IsEnabled = true;
                                    }
                                    if (a.Content.ToString() == "上一页")
                                    {
                                        a.IsEnabled = true;
                                    }
                                    if (a.Content.ToString() == "下一页")
                                    {
                                        a.IsEnabled = true;
                                    }
                                });
                        });
                    ShowData(tableName);
                }
            }
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Size();
            //注册事件
            service.GetTableDataCompleted += new EventHandler<GetTableDataCompletedEventArgs>(service_GetTableDataCompleted);
            service.GetTableDataByContextIdCompleted += new EventHandler<GetTableDataByContextIdCompletedEventArgs>(service_GetTableDataByContextIdCompleted);
            //如果设置过分页大小的话，就用这个数据
            if (setting.Contains(PageSizeKey))
                PageSize.Text = setting[PageSizeKey].ToString();
            //第一次加载数据
            if (SearchCondition != null)
            {
                RangeDetail.Text = string.Format("数据库: {0}  当前时间：{1} 查询时间段：{2} - {3}", SearchCondition.DatabasePrefix, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                   SearchCondition.BeginTime.ToString("yyyy/MM/dd HH:mm:ss"), SearchCondition.EndTime.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            ShowData("");
        }

        private void Size()
        {
            ContentStackPanel.Width = this.Width - 140;
        }

        private void service_GetTableDataByContextIdCompleted(object sender, GetTableDataByContextIdCompletedEventArgs e)
        {
            this.Busy.IsBusy = false;

            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }

            if (e.Result != null)
            {
                var tableData = e.Result.Where(t => t.Tables.Count > 0).OrderBy(t => t.Tables.First().DatabaseName).ToList();

                foreach (var table in tableData)
                {
                    var data = table.Tables.First();
                    if (data.Data.Count == 0) continue;
                    var enties = (data.Data as IEnumerable<Dictionary<string, string>>).ToDataSource();
                    if (enties != null)
                    {
                        var tab = new TabItem
                        {
                            Header = data.DatabasePrefix,
                            Tag = data.DatabaseName,
                        };
                        if (data.Data.Count > 0)
                        {
                            var panel = new StackPanel
                            {
                                Orientation = Orientation.Vertical,
                            };

                            var grid = new DataGrid()
                            {
                                AutoGenerateColumns = true,
                                ItemsSource = enties,
                                SelectionMode = DataGridSelectionMode.Single,
                                Tag = table,
                                IsReadOnly = false,
                                MaxColumnWidth = 600,
                                MaxHeight = 600,
                            };
                            grid.BeginEdit();
                            panel.Children.Add(grid);
                            tab.Content = panel;
                        }
                        ListGridTab.Items.Add(tab);
                    }

                }

            }
        }


        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="tableName">是否仅限于一个表名</param>
        private void ShowData(string tableName)
        {
            if (SearchCondition != null)
            {
                var pageSize = GetPageSize();
                if (pageSize > 0)
                {
                    var index = 0;
                    //如果以前翻页过，那么还是加载这页的数据
                    if (!string.IsNullOrEmpty(tableName) && pageIndex.ContainsKey(tableName))
                        index = pageIndex[tableName];
                    this.Busy.IsBusy = true;
                    if (string.IsNullOrEmpty(SearchCondition.ContextId))
                    {
                        //如果传了表名表示不是第一次加载或是刷新的加载，而是翻页的加载，最后一个参数是状态信息
                        if (!string.IsNullOrEmpty(tableName))
                        {
                            service.GetTableDataAsync(SearchCondition.DatabasePrefix, new List<string> { tableName },
                                SearchCondition.BeginTime, SearchCondition.EndTime, index, pageSize, SearchCondition.Filters, tableName);
                        }
                        else
                        {
                            service.GetTableDataAsync(SearchCondition.DatabasePrefix, SearchCondition.TableNames,
                              SearchCondition.BeginTime, SearchCondition.EndTime, index, pageSize, SearchCondition.Filters);
                        }
                    }
                    else
                    {
                        //根据上下文Id查看
                        Refresh.IsEnabled = false;
                        Group.IsEnabled = false;
                        service.GetTableDataByContextIdAsync(SearchCondition.ContextId);
                    }
                }
            }
            else
            {
                MessageBox.Show("错误！没有获取到查询参数！");
                this.Close();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh.IsEnabled = false;
            setting[PageSizeKey] = GetPageSize();
            //因为更改了分页大小，所以刷新加载不传表名，加载所有的
            ShowData("");
        }

        private void ListGridTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabIndex = ListGridTab.SelectedIndex;
        }

        private void Detail_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = (ListGridTab.SelectedItem as TabItem);
            if (tabItem != null)
            {
                var panel = tabItem.Content as StackPanel;
                if (panel != null)
                {
                    var grid = panel.Children.OfType<DataGrid>().FirstOrDefault();
                    if (grid != null)
                    {
                        if (grid.SelectedItem == null)
                        {
                            MessageBox.Show("请先选择一行！");
                            return;
                        }
                        else
                        {
                            //数据行
                            var obj = grid.SelectedItem;
                            //原始数据
                            var table = grid.Tag as TableData;
                            //先找到ID列
                            var idProperty = obj.GetType().GetProperty(table.PkColumnDisplayName);
                            if (idProperty != null)
                            {
                                //取值
                                var id = idProperty.GetValue(obj, null);
                                if (id as string != null)
                                {
                                    //尝试根据列显示名找到数据库名
                                    var databaseName = "";
                                    Table t = null;
                                    foreach (var tt in table.Tables)
                                    {
                                        var data = tt.Data;
                                        foreach (var d in data)
                                        {
                                            if (d.ContainsKey(table.PkColumnDisplayName) &&
                                                d[table.PkColumnDisplayName].ToString() == id.ToString())
                                            {
                                                databaseName = tt.DatabaseName;
                                                t = tt;
                                                break;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(databaseName))
                                            break;
                                    }
                                    ViewDetailData c = new ViewDetailData();
                                    var root = Application.Current.RootVisual as FrameworkElement;
                                    c.DetailCondition = new DetailCondition
                                    {
                                        DatabasePrefix = t.DatabasePrefix,
                                        ID = id.ToString(),
                                        TableName = table.TableName,
                                        PkColumnName = table.PkColumnName,
                                        DatabaseName = databaseName,
                                    };
                                    c.Width = root.ActualWidth * 0.98;
                                    c.Height = root.ActualHeight * 0.98;
                                    c.HorizontalAlignment = HorizontalAlignment.Center;
                                    c.VerticalAlignment = VerticalAlignment.Center;
                                    c.Show();
                                    service.LogAsync(new OperationLog
                                    {
                                        AccountName = Data.AdminConfigurationItem.UserName,
                                        AccountRealName = Data.AdminConfigurationItem.RealName,
                                        CategoryName = t.DatabasePrefix.Substring(0, t.DatabasePrefix.IndexOf("__")),
                                        DatabaseName = t.DatabasePrefix.Substring(t.DatabasePrefix.IndexOf("__") + 2),
                                        TableName = table.TableName,
                                        Action = "查看详细视图",
                                        ActionMemo = string.Format("主键列名：{0} 主键：{0}", table.PkColumnName, id.ToString()),
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Group_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ViewGroupData c = new ViewGroupData();
            var root = Application.Current.RootVisual as FrameworkElement;
            var tableName = (ListGridTab.SelectedItem as TabItem).Tag.ToString();
            c.SearchCondition = new SearchCondition
            {
                DatabasePrefix = SearchCondition.DatabasePrefix,
                BeginTime = SearchCondition.BeginTime,
                EndTime = SearchCondition.EndTime,
                Filters = SearchCondition.Filters,
                TableNames = SearchCondition.TableNames,
                SelectedTableName = tableName,
            };
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            service.LogAsync(new OperationLog
            {
                AccountName = Data.AdminConfigurationItem.UserName,
                AccountRealName = Data.AdminConfigurationItem.RealName,
                CategoryName = SearchCondition.DatabasePrefix.Substring(0, SearchCondition.DatabasePrefix.IndexOf("__")),
                DatabaseName = SearchCondition.DatabasePrefix.Substring(SearchCondition.DatabasePrefix.IndexOf("__") + 2),
                TableName = tableName,
                Action = "查看分组视图",
                ActionMemo = string.Format("开始时间：{0} 结束时间：{1} 过滤条件：{2}",
                  SearchCondition.BeginTime,
                  SearchCondition.EndTime,
                  SearchCondition.Filters)
            });
        }

        private void Stat_Click(object sender, RoutedEventArgs e)
        {
            if (SearchCondition.TableNames.Count > 10)
            {
                MessageBox.Show("对于数据量统计方式呈现最多只能选择10个表");
                return;
            }
            this.Close();
            ViewStatData c = new ViewStatData();
            var root = Application.Current.RootVisual as FrameworkElement;
            var tableName = (ListGridTab.SelectedItem as TabItem).Tag.ToString();
            c.SearchCondition = new SearchCondition
            {
                DatabasePrefix = SearchCondition.DatabasePrefix,
                BeginTime = SearchCondition.BeginTime,
                EndTime = SearchCondition.EndTime,
                Filters = SearchCondition.Filters,
                TableNames = SearchCondition.TableNames,
                SelectedTableName = tableName,
            };
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            service.LogAsync(new OperationLog
            {
                AccountName = Data.AdminConfigurationItem.UserName,
                AccountRealName = Data.AdminConfigurationItem.RealName,
                CategoryName = SearchCondition.DatabasePrefix.Substring(0, SearchCondition.DatabasePrefix.IndexOf("__")),
                DatabaseName = SearchCondition.DatabasePrefix.Substring(SearchCondition.DatabasePrefix.IndexOf("__") + 2),
                TableName = tableName,
                Action = "查看统计视图",
                ActionMemo = string.Format("开始时间：{0} 结束时间：{1} 过滤条件：{2}",
                  SearchCondition.BeginTime,
                  SearchCondition.EndTime,
                  SearchCondition.Filters)
            });
        }

        private void CopyUrlAddress_Click(object sender, RoutedEventArgs e)
        {
            var tableName = (ListGridTab.SelectedItem as TabItem).Tag.ToString();
            this.SearchCondition.SelectedTableName = tableName;
            var ud = new UrlData
            {
                Condition = this.SearchCondition,
                Type = "List",
            };
            MemoryStream ms = new MemoryStream();
            var serializer = new SharpSerializer(true);
            serializer.Serialize(ud, ms);
            byte[] array = ms.ToArray();
            ms.Close();
            string data = Convert.ToBase64String(array);
            var url = HtmlPage.Document.DocumentUri;
            var urlstring = url.ToString();
            if (url.Query.Length > 0)
                urlstring = urlstring.Replace(url.Query, "");
            Clipboard.SetText(string.Format("{0}?url={1}", urlstring, data));
        }

        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size();
        }
    }
}

