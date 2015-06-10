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
        //ѡ��ķ�ҳ�����ţ����ݱ����ֿ����
        private Dictionary<string, int> pageIndex = new Dictionary<string, int>();
        //ѡ���tabҳ������
        private int tabIndex = 0;
        //�ж��Ƿ����ڼ�������
        private bool working = false;

        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

        public ViewListData()
        {
            InitializeComponent();

        }

        /// <summary>
        /// ��ȡ��ҳ��С
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
                    MessageBox.Show("ÿҳ��ʾ��¼���������0��100֮�䣡");
                    PageSize.Focus();
                    return -1;
                }
            }
            else
            {
                MessageBox.Show("ÿҳ��ʾ��¼�����������֣�");
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
                    //����
                    var data = new List<Dictionary<string, string>>();
                    //������
                    //var count = 0;
                    //���������ݿ�����������һ����������ܾͻ��ж������
                    foreach (var t in table.Tables)
                    {
                        data.AddRange(t.Data);
                        //count += t.TotalCount;
                    }
                    //�ֵ��Ϊ��̬���ͼ���
                    var enties = (data as IEnumerable<Dictionary<string, string>>).ToDataSource();
                    if (enties != null)
                    {
                        //�Ƿ����˱��������첽���÷�����ʱ����Ϊ״̬��Ϣ����Ĳ���
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
                                    Text = "�� 1 ҳ",
                                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                };
                                var pagertop = new Button
                                {
                                    Content = "��ҳ",
                                    Tag = table.TableName,
                                    IsEnabled = false,
                                    Margin = new Thickness(20, 0, 0, 0),
                                };
                                var pagerup = new Button
                                {
                                    Content = "��һҳ",
                                    Tag = table.TableName,
                                    Margin = new Thickness(20, 0, 0, 0),
                                    IsEnabled = false,
                                };
                                var pagerdown = new Button
                                {
                                    Content = "��һҳ",
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

                                //���Ǳ��
                                panel.Children.Add(grid);
                                //���������ķ�ҳ
                                panel.Children.Add(pager2Panel);
                                tab.Content = panel;
                            }
                            ListGridTab.Items.Add(tab);
                        }
                        //�鿴�����һҳʱ,��ʾ��ʹ�����ҵ�
                        else
                        {
                            if (data.Count == 0)
                            {
                                MessageBox.Show("�������һҳ");
                                var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == tableName.ToString());
                                var panel = tab.Content as StackPanel;
                                panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                                {
                                    member.Children.OfType<Button>().ToList().ForEach(a =>
                                    {
                                        if (a.Content.ToString() == "��һҳ")
                                        {
                                            a.IsEnabled = false;
                                        }
                                        if (pageIndex[tableName] == 1)
                                        {
                                            if (a.Content.ToString() == "��ҳ")
                                            {
                                                a.IsEnabled = false;
                                            }
                                            if (a.Content.ToString() == "��һҳ")
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
                                //�ҵ���ǩҳ
                                var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == table.TableName);
                                //�ҵ�ԭ�ȵķ�ҳ������
                                var index = 0;
                                if (!string.IsNullOrEmpty(table.TableName) && pageIndex.ContainsKey(table.TableName))
                                    index = pageIndex[table.TableName];
                                var panel = tab.Content as StackPanel;
                                if (panel != null)
                                {
                                    //��ҳ�ؼ���״̬����Ϊԭ��ѡ���
                                    panel.Children.OfType<StackPanel>().SelectMany(p => p.Children).OfType<DataPager>()
                                        .ToList().ForEach(p => p.PageIndex = index);
                                    //�������°�
                                    var grid = panel.Children.OfType<DataGrid>().FirstOrDefault();
                                    if (grid != null)
                                    {
                                        grid.ItemsSource = enties;
                                        grid.Tag = table;
                                    }

                                    //��ʾҳ��
                                    panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                                    {
                                        member.Children.OfType<TextBlock>().ToList().ForEach(a =>
                                        {
                                            a.Text = string.Format("�� {0} ҳ", pageIndex[tableName] + 1);
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

        //ѡ����ҳ
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
                    //�ѷ�ҳ�����ݱ�������
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
                                if (a.Content.ToString() == "��ҳ")
                                {
                                    a.IsEnabled = false;
                                }
                                if (a.Content.ToString() == "��һҳ")
                                {
                                    a.IsEnabled = false;
                                }
                                if (a.Content.ToString() == "��һҳ")
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

        //ѡ����ҳ
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
                    //�ѷ�ҳ�����ݱ�������                   

                    pageIndex[tableName] = pageIndex[tableName] - 1;
                    var tab = ListGridTab.Items.Cast<TabItem>().FirstOrDefault(t => (string)t.Tag == pager.Tag.ToString());
                    var panel = tab.Content as StackPanel;
                    panel.Children.OfType<StackPanel>().ToList().ForEach(member =>
                    {
                        member.Children.OfType<Button>().ToList().ForEach(a =>
                        {
                            if (pageIndex[tableName] == 0)
                            {
                                if (a.Content.ToString() == "��ҳ")
                                {
                                    a.IsEnabled = false;
                                }
                                if (a.Content.ToString() == "��һҳ")
                                {
                                    a.IsEnabled = false;
                                }
                            }
                            if (a.Content.ToString() == "��һҳ")
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

        //ѡ����ҳ
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
                    //�ѷ�ҳ�����ݱ�������
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
                                    if (a.Content.ToString() == "��ҳ")
                                    {
                                        a.IsEnabled = true;
                                    }
                                    if (a.Content.ToString() == "��һҳ")
                                    {
                                        a.IsEnabled = true;
                                    }
                                    if (a.Content.ToString() == "��һҳ")
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
            //ע���¼�
            service.GetTableDataCompleted += new EventHandler<GetTableDataCompletedEventArgs>(service_GetTableDataCompleted);
            service.GetTableDataByContextIdCompleted += new EventHandler<GetTableDataByContextIdCompletedEventArgs>(service_GetTableDataByContextIdCompleted);
            //������ù���ҳ��С�Ļ��������������
            if (setting.Contains(PageSizeKey))
                PageSize.Text = setting[PageSizeKey].ToString();
            //��һ�μ�������
            if (SearchCondition != null)
            {
                RangeDetail.Text = string.Format("���ݿ�: {0}  ��ǰʱ�䣺{1} ��ѯʱ��Σ�{2} - {3}", SearchCondition.DatabasePrefix, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
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
        /// ��������
        /// </summary>
        /// <param name="tableName">�Ƿ������һ������</param>
        private void ShowData(string tableName)
        {
            if (SearchCondition != null)
            {
                var pageSize = GetPageSize();
                if (pageSize > 0)
                {
                    var index = 0;
                    //�����ǰ��ҳ������ô���Ǽ�����ҳ������
                    if (!string.IsNullOrEmpty(tableName) && pageIndex.ContainsKey(tableName))
                        index = pageIndex[tableName];
                    this.Busy.IsBusy = true;
                    if (string.IsNullOrEmpty(SearchCondition.ContextId))
                    {
                        //������˱�����ʾ���ǵ�һ�μ��ػ���ˢ�µļ��أ����Ƿ�ҳ�ļ��أ����һ��������״̬��Ϣ
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
                        //����������Id�鿴
                        Refresh.IsEnabled = false;
                        Group.IsEnabled = false;
                        service.GetTableDataByContextIdAsync(SearchCondition.ContextId);
                    }
                }
            }
            else
            {
                MessageBox.Show("����û�л�ȡ����ѯ������");
                this.Close();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh.IsEnabled = false;
            setting[PageSizeKey] = GetPageSize();
            //��Ϊ�����˷�ҳ��С������ˢ�¼��ز����������������е�
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
                            MessageBox.Show("����ѡ��һ�У�");
                            return;
                        }
                        else
                        {
                            //������
                            var obj = grid.SelectedItem;
                            //ԭʼ����
                            var table = grid.Tag as TableData;
                            //���ҵ�ID��
                            var idProperty = obj.GetType().GetProperty(table.PkColumnDisplayName);
                            if (idProperty != null)
                            {
                                //ȡֵ
                                var id = idProperty.GetValue(obj, null);
                                if (id as string != null)
                                {
                                    //���Ը�������ʾ���ҵ����ݿ���
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
                                        Action = "�鿴��ϸ��ͼ",
                                        ActionMemo = string.Format("����������{0} ������{0}", table.PkColumnName, id.ToString()),
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
                Action = "�鿴������ͼ",
                ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
                  SearchCondition.BeginTime,
                  SearchCondition.EndTime,
                  SearchCondition.Filters)
            });
        }

        private void Stat_Click(object sender, RoutedEventArgs e)
        {
            if (SearchCondition.TableNames.Count > 10)
            {
                MessageBox.Show("����������ͳ�Ʒ�ʽ�������ֻ��ѡ��10����");
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
                Action = "�鿴ͳ����ͼ",
                ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
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

