using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using Adhesive.Mongodb.Silverlight.Service;
using System.Windows.Browser;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Polenter.Serialization;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class ViewGroupData : ChildWindow
    {
        public string chartkey;
        public object chartvalue;
        public SearchCondition SearchCondition { get; set; }
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

        public ViewGroupData()
        {
            InitializeComponent();
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Size();
            if (SearchCondition != null)
            {
                RangeDetail.Text = string.Format("数据库: {0}  当前时间：{1} 查询时间段：{2} - {3}", SearchCondition.DatabasePrefix, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                   SearchCondition.BeginTime.ToString("yyyy/MM/dd HH:mm:ss"), SearchCondition.EndTime.ToString("yyyy/MM/dd HH:mm:ss"));
            }

            service.GetGroupDataCompleted += new EventHandler<GetGroupDataCompletedEventArgs>(service_GetGroupDataCompleted);
            ShowData();
        }

        private void Size()
        {
            this.ListChartTab.Width = this.Width - 100;
            this.ListChartTab.Height = this.Height - 100;
        }

        private void ps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chartkey != null)
            {
                SearchCondition.Filters.Remove(chartkey);
            }
            if (e.AddedItems.Count > 0)
            {
                //把点击的饼图区域的文字说明设置为粗体
                KeyValuePair<string, int> a = (KeyValuePair<string, int>)e.AddedItems[0];
                PieSeries ps = sender as PieSeries;

                var tab = ListChartTab.SelectedItem as TabItem;
                if (tab != null)
                {
                    var scroll = tab.Content as ScrollViewer;
                    if (scroll != null)
                    {
                        var grid = scroll.Content as Grid;
                        if (grid != null)
                        {
                            grid.Children.OfType<Chart>().ToList().ForEach(chart =>
                            {
                                var pieSeries = chart.Series.OfType<PieSeries>().FirstOrDefault();
                                if (pieSeries != null && pieSeries != ps)
                                {
                                    pieSeries.SelectedItem = null;
                                    pieSeries.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.FontWeight = FontWeights.Normal);
                                }
                                if (pieSeries == ps)
                                {
                                    PSselected.Text = string.Format("{0}: {1}", chart.Title, a.Key);
                                    Cancel.Visibility = System.Windows.Visibility.Visible;
                                    var data = ListChartTab.Tag as List<Group>;
                                    foreach (var group in data)
                                    {
                                        var groupItems = group.GroupItems.Where(i => i.Values.Count > 0).ToList();
                                        foreach (var groupItem in groupItems)
                                        {
                                            if (groupItem.DisplayName == chart.Title.ToString())
                                            {
                                                foreach (var c in groupItem.Values)
                                                {
                                                    if (c.Key.DisplayName == a.Key)
                                                    {
                                                        chartkey = groupItem.Name;
                                                        chartvalue = c.Key.Name;
                                                        if (!SearchCondition.Filters.ContainsKey(chartkey))
                                                        {
                                                            SearchCondition.Filters.Add(chartkey, chartvalue);
                                                        }
                                                        else
                                                        {
                                                            SearchCondition.Filters.Remove(chartkey);
                                                            SearchCondition.Filters.Add(chartkey, chartvalue);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    ps.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.FontWeight = FontWeights.Normal);
                                    ps.LegendItems.OfType<LegendItem>().Where(l => l.Content.ToString() == a.Key).ToList().ForEach(l => l.FontWeight = FontWeights.Bold);
                                }
                            });
                        }
                    }
                }
            }
        }
        private void service_GetGroupDataCompleted(object sender, GetGroupDataCompletedEventArgs e)
        {
            this.Busy.IsBusy = false;
            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }

            if (e.Result != null)
            {
                ListChartTab.Tag = e.Result.ToList();
                var data = e.Result.ToList();
                foreach (var group in data)
                {
                    //没数据的就不用显示了
                    var groupItems = group.GroupItems.Where(i => i.Values.Count > 0).ToList();

                    var scroll = new ScrollViewer()
                    {
                    };
                    var tab = new TabItem
                    {
                        Header = group.TableName,
                        Tag = group.TableName,
                    };
                    var grid = new Grid();
                    var chartWidth = (this.ListChartTab.Width - 50) / 2;
                    var chartHeight = 300;
                    //最大列索引
                    var maxColumnIndex = Math.Floor(ListChartTab.Width / chartWidth);
                    //最大行索引
                    var maxRowIndex = ((groupItems.Count % maxColumnIndex == 0 ? 0 : (maxColumnIndex - groupItems.Count % maxColumnIndex))
                        + groupItems.Count) / maxColumnIndex;
                    //初始化表格布局控件
                    for (int j = 0; j < maxRowIndex; j++)
                    {
                        RowDefinition rd = new RowDefinition();
                        rd.Height = new GridLength(chartHeight);
                        grid.RowDefinitions.Add(rd);
                    }
                    for (int j = 0; j < maxColumnIndex; j++)
                    {
                        ColumnDefinition cd = new ColumnDefinition();
                        cd.Width = new GridLength(chartWidth);
                        grid.ColumnDefinitions.Add(cd);
                    }
                    var rowIndex = 0;
                    var columnIndex = 0;

                    foreach (var groupItem in groupItems)
                    {
                        var chart = new Chart()
                        {
                            Margin = new Thickness(10)
                        };
                        //把图表放置到相应的单元格中
                        chart.SetValue(Grid.ColumnProperty, columnIndex);
                        chart.SetValue(Grid.RowProperty, rowIndex);

                        if (columnIndex < maxColumnIndex - 1)
                        {
                            columnIndex++;
                        }
                        else
                        {
                            columnIndex = 0;
                            rowIndex++;
                        }
                        var ps = new PieSeries();


                        ps.ItemsSource = CombineData(groupItem.Values, 10);
                        ps.IsSelectionEnabled = true;
                        ps.IndependentValuePath = "Key";
                        ps.DependentValuePath = "Value";
                        ps.AnimationSequence = AnimationSequence.Simultaneous;
                        ps.SelectionChanged += new SelectionChangedEventHandler(ps_SelectionChanged);
                        chart.Title = groupItem.DisplayName;
                        if (!string.IsNullOrEmpty(groupItem.Description))
                            chart.Title += string.Format(" ({0})", groupItem.Description);
                        chart.Series.Add(ps);
                        grid.Children.Add(chart);
                    }
                    //滚动控件中包含表格布局
                    scroll.Content = grid;
                    //tab页中包含滚动控件
                    tab.Content = scroll;
                    //tab页容器中包含tab页
                    ListChartTab.Items.Add(tab);
                    if (!string.IsNullOrEmpty(SearchCondition.SelectedTableName) && group.TableName == SearchCondition.SelectedTableName)
                        tab.IsSelected = true;
                }
            }
        }

        private Dictionary<string, int> CombineData(Dictionary<GroupItemValuePair, int> data, int count)
        {
            if (data.Count > count)
            {
                var combined = data.OrderByDescending(d => d.Value).Take(count - 1).ToDictionary(a => a.Key, a => a.Value);
                combined.Add(new GroupItemValuePair()
                {
                    DisplayName = "其它",
                    Name = "",
                }, data.Sum(d => d.Value) - combined.Sum(d => d.Value));
                return Format(combined);
            }
            return Format(data);
        }

        private Dictionary<string, int> Format(Dictionary<GroupItemValuePair, int> data)
        {
            var r = new Dictionary<string, int>();
            foreach (var item in data)
                r.Add(item.Key.DisplayName, item.Value);
            return r;
        }

        private void ShowData()
        {
            if (SearchCondition != null)
            {
                this.Busy.IsBusy = true;
                service.GetGroupDataAsync(SearchCondition.DatabasePrefix, SearchCondition.TableNames, SearchCondition.BeginTime, SearchCondition.EndTime, SearchCondition.Filters);
            }
            else
            {
                MessageBox.Show("错误！没有获取到查询参数！");
                this.Close();
            }
        }

        private void List_Click(object sender, RoutedEventArgs e)
        {
            var count = 0;
            var tab = ListChartTab.SelectedItem as TabItem;
            if (tab != null)
            {
                var scroll = tab.Content as ScrollViewer;
                if (scroll != null)
                {
                    var grid = scroll.Content as Grid;
                    if (grid != null)
                    {
                        grid.Children.OfType<Chart>().ToList().ForEach(chart =>
                        {
                            var pieSeries = chart.Series.OfType<PieSeries>().FirstOrDefault();
                            if (pieSeries != null)
                            {
                                if (pieSeries.SelectedItem != null)
                                {
                                    count++;
                                }
                            }
                        });
                    }
                }
            }

            if (count == 1)
            {
                //弹出列表视图
                ViewListData c = new ViewListData();
                var root = Application.Current.RootVisual as FrameworkElement;
                var tableName = (ListChartTab.SelectedItem as TabItem).Tag.ToString();

                c.SearchCondition = new SearchCondition
                {
                    DatabasePrefix = SearchCondition.DatabasePrefix,
                    TableNames = new List<string> { tableName },
                    Filters = SearchCondition.Filters,
                    BeginTime = SearchCondition.BeginTime,
                    EndTime = SearchCondition.EndTime,
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
                    TableName = ListChartTab.SelectedItem.ToString(),
                    Action = "查看列表视图",
                    ActionMemo = string.Format("开始时间：{0} 结束时间：{1} 过滤条件：{2}",
                      SearchCondition.BeginTime,
                      SearchCondition.EndTime,
                      SearchCondition.Filters.GetFilterText())
                });
            }

            if (count == 0)
            {
                this.Close();
                ViewListData c = new ViewListData();
                var root = Application.Current.RootVisual as FrameworkElement;
                var tableName = (ListChartTab.SelectedItem as TabItem).Tag.ToString();
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
                    Action = "查看列表视图",
                    ActionMemo = string.Format("开始时间：{0} 结束时间：{1} 过滤条件：{2}",
                      SearchCondition.BeginTime,
                      SearchCondition.EndTime,
                      SearchCondition.Filters)
                });
            }
        }


        private void Stat_Click(object sender, RoutedEventArgs e)
        {
            if (SearchCondition.TableNames.Count > 10)
            {
                MessageBox.Show("对于数据量统计方式呈现最多只能选择10个表");
                return;
            }
            var count = 0;
            var tab = ListChartTab.SelectedItem as TabItem;
            if (tab != null)
            {
                var scroll = tab.Content as ScrollViewer;
                if (scroll != null)
                {
                    var grid = scroll.Content as Grid;
                    if (grid != null)
                    {
                        grid.Children.OfType<Chart>().ToList().ForEach(chart =>
                        {
                            var pieSeries = chart.Series.OfType<PieSeries>().FirstOrDefault();
                            if (pieSeries != null)
                            {
                                if (pieSeries.SelectedItem != null)
                                {
                                    count++;
                                }
                            }
                        });
                    }
                }
            }
            if (count == 1)
            {
                if (!SearchCondition.Filters.ContainsKey(chartkey))
                {
                    SearchCondition.Filters.Add(chartkey, chartvalue);
                }
                else
                {
                    SearchCondition.Filters.Remove(chartkey);
                    SearchCondition.Filters.Add(chartkey, chartvalue);
                }
                ViewStatData c = new ViewStatData();
                var root = Application.Current.RootVisual as FrameworkElement;
                var tableName = (ListChartTab.SelectedItem as TabItem).Tag.ToString();

                c.SearchCondition = new SearchCondition
                {
                    DatabasePrefix = SearchCondition.DatabasePrefix,
                    TableNames = new List<string> { tableName },
                    Filters = SearchCondition.Filters,
                    BeginTime = SearchCondition.BeginTime,
                    EndTime = SearchCondition.EndTime,
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
                    TableName = ListChartTab.SelectedItem.ToString(),
                    Action = "查看统计视图",
                    ActionMemo = string.Format("开始时间：{0} 结束时间：{1} 过滤条件：{2}",
                      SearchCondition.BeginTime,
                      SearchCondition.EndTime,
                      SearchCondition.Filters.GetFilterText())
                });
            }


            if (count == 0)
            {
                this.Close();
                ViewStatData c = new ViewStatData();
                var root = Application.Current.RootVisual as FrameworkElement;
                var tableName = (ListChartTab.SelectedItem as TabItem).Tag.ToString();
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
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SearchCondition.Filters.Remove(chartkey);
            PSselected.Text = "";
            Cancel.Visibility = System.Windows.Visibility.Collapsed;
            var tab = ListChartTab.SelectedItem as TabItem;
            if (tab != null)
            {
                var scroll = tab.Content as ScrollViewer;
                if (scroll != null)
                {
                    var grid = scroll.Content as Grid;
                    if (grid != null)
                    {
                        grid.Children.OfType<Chart>().ToList().ForEach(chart =>
                        {
                            var pieSeries = chart.Series.OfType<PieSeries>().FirstOrDefault();
                            if (pieSeries != null)
                            {
                                pieSeries.SelectedItem = null;
                                pieSeries.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.FontWeight = FontWeights.Normal);
                            }

                        });
                    }
                }
            }
        }

        private void CopyUrlAddress_Click(object sender, RoutedEventArgs e)
        {
            var ud = new UrlData
            {
                Condition = this.SearchCondition,
                Type = "Group",
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
