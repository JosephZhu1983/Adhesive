using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Adhesive.Mongodb.Silverlight.Service;
using System.Threading;
using Polenter.Serialization;
using System.IO;
using System.Windows.Browser;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class ViewStatData : ChildWindow
    {
        public SearchCondition SearchCondition { get; set; }
        private List<Statistics> statData;
        private readonly string TimePoint = "ʱ���";
        private readonly string DataVolume = "������";
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
        private bool autoRefresh = false;
        private bool enable = true;
        private Thread autoRefreshThread;

        private List<Color> colorPool = new List<Color>()
        {
            0xFF3F94E9.ToColor(),
            Colors.Red,
            Colors.Blue,
            Colors.Orange,
            Colors.Magenta,
            Colors.Green,
            Colors.Brown,
            Colors.Yellow,  
            Colors.Purple,
            Colors.DarkGray,
            Colors.Cyan,
            Colors.Black,
        };

        public ViewStatData()
        {
            InitializeComponent();
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Size();
            service.GetStatisticsDataCompleted += new EventHandler<GetStatisticsDataCompletedEventArgs>(service_GetStatisticsDataCompleted);

            //������ʼʱ��ͽ���ʱ���Զ�����ʱ������
            SetTimeSpan();

            //��ʼ��x��
            DateTimeAxis dateAxis = new DateTimeAxis()
            {
                Orientation = AxisOrientation.X,
                Title = TimePoint,
                FontStyle = FontStyles.Normal,
                FontSize = 10f,
                ShowGridLines = true,
            };
            dateAxis.GridLineStyle = new Style(typeof(Line));
            dateAxis.GridLineStyle.Setters.Add(new Setter(Line.StrokeThicknessProperty, 1));
            dateAxis.GridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, "LightGray"));

            //��ʼ��y��
            LinearAxis valueAxis = new LinearAxis()
            {
                Orientation = AxisOrientation.Y,
                Title = DataVolume,
                FontStyle = FontStyles.Normal,
                FontSize = 12f,
                ShowGridLines = true,
            };
            valueAxis.GridLineStyle = new Style(typeof(Line));
            valueAxis.GridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, "LightGray"));
            valueAxis.GridLineStyle.Setters.Add(new Setter(Line.StrokeThicknessProperty, 1));
            Chart.Axes.Clear();
            Chart.Axes.Add(dateAxis);
            Chart.Axes.Add(valueAxis);

            //��������
            ShowData();
        }


        private void Size()
        {
            Chart.Height = this.Height - 100;
            ContentStackPanel.Width = this.Width - 50;
        }

        private void ShowData()
        {
            //��ȡʱ������
            var timeSpan = GetTimeSpan();
            if (timeSpan.HasValue)
            {
                this.Busy.IsBusy = true;
                var beginTime = DateTime.Now.Subtract(SearchCondition.EndTime - SearchCondition.BeginTime);
                if (timeSpan.Value.TotalSeconds < TimeSpan.FromMinutes(1).TotalSeconds)
                {
                    beginTime -= TimeSpan.FromSeconds(beginTime.Second % 10);
                }
                else if (timeSpan.Value.TotalSeconds >= TimeSpan.FromMinutes(1).TotalSeconds && timeSpan.Value.TotalSeconds < TimeSpan.FromHours(1).TotalSeconds)
                {
                    beginTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, beginTime.Hour, beginTime.Minute, 0);
                    beginTime -= TimeSpan.FromMinutes(beginTime.Minute % 10);
                }
                else if (timeSpan.Value.TotalSeconds >= TimeSpan.FromHours(1).TotalSeconds && timeSpan.Value.TotalSeconds < TimeSpan.FromDays(1).TotalSeconds)
                {
                    beginTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day, beginTime.Hour, 0, 0);
                    beginTime -= TimeSpan.FromHours(beginTime.Hour % 10);
                }
                else if (timeSpan.Value.TotalSeconds >= TimeSpan.FromDays(1).TotalSeconds)
                {
                    beginTime = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day);
                    beginTime -= TimeSpan.FromDays(beginTime.Day % 10);
                }

                if (autoRefresh)
                {
                    service.GetStatisticsDataAsync(SearchCondition.DatabasePrefix, SearchCondition.TableNames.Take(12).ToList(),
                                beginTime, DateTime.Now, timeSpan.Value, SearchCondition.Filters);
                }
                else
                {
                    service.GetStatisticsDataAsync(SearchCondition.DatabasePrefix, SearchCondition.TableNames.Take(12).ToList(),
                                beginTime, SearchCondition.EndTime, timeSpan.Value, SearchCondition.Filters);
                }
            }
        }

        private void SetTimeSpan()
        {
            //ʱ����
            var span = SearchCondition.EndTime - SearchCondition.BeginTime;
            //����������һ��ͼ����100����
            var count = Convert.ToInt32(span.TotalSeconds / 100);

            //��λΪ��
            if (count >= 0 && count < TimeSpan.FromMinutes(1).TotalSeconds)
            {
                var item = TimeSpanType.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (string)c.Tag == "Second");
                if (item != null) TimeSpanType.SelectedItem = item;
                //10��20�����������������������
                if (count > 10)
                {
                    count = count - count % 10;
                }
            }
            else if (count > TimeSpan.FromMinutes(1).TotalSeconds && count < TimeSpan.FromHours(1).TotalSeconds)
            {
                var item = TimeSpanType.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (string)c.Tag == "Minute");
                if (item != null) TimeSpanType.SelectedItem = item;
                count /= Convert.ToInt32(TimeSpan.FromMinutes(1).TotalSeconds);
                if (count > 10)
                {
                    count = count - count % 10;
                }
            }
            else if (count > TimeSpan.FromHours(1).TotalSeconds && count < TimeSpan.FromDays(1).TotalSeconds)
            {
                var item = TimeSpanType.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (string)c.Tag == "Hour");
                if (item != null) TimeSpanType.SelectedItem = item;
                count /= Convert.ToInt32(TimeSpan.FromHours(1).TotalSeconds);
                if (count > 10)
                {
                    count = count - count % 10;
                }
            }
            else if (count > TimeSpan.FromDays(1).TotalSeconds && count < TimeSpan.FromDays(7).TotalSeconds)
            {
                var item = TimeSpanType.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (string)c.Tag == "Day");
                if (item != null) TimeSpanType.SelectedItem = item;
                count /= Convert.ToInt32(TimeSpan.FromDays(1).TotalSeconds);
                if (count > 10)
                {
                    count = count - count % 10;
                }
            }
            else if (count > TimeSpan.FromDays(7).TotalSeconds && count < TimeSpan.FromDays(30).TotalSeconds)
            {
                var item = TimeSpanType.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (string)c.Tag == "Week");
                if (item != null) TimeSpanType.SelectedItem = item;
                count /= Convert.ToInt32(TimeSpan.FromDays(7).TotalSeconds);
                if (count > 10)
                {
                    count = count - count % 10;
                }
            }
            else if (count > TimeSpan.FromDays(30).TotalSeconds)
            {
                var item = TimeSpanType.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (string)c.Tag == "Month");
                if (item != null) TimeSpanType.SelectedItem = item;
                count /= Convert.ToInt32(TimeSpan.FromDays(30).TotalSeconds);
                if (count > 10)
                {
                    count = count - count % 10;
                }
            }
            //��λ����Ҳ��1
            if (count == 0) count = 1;
            TimeSpanNumber.Value = count;
        }

        private TimeSpan? GetTimeSpan()
        {
            double timeSpanSeconds = 0;
            var selectedItem = TimeSpanType.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                var value = selectedItem.Tag;
                if (value != null)
                {
                    switch (value.ToString())
                    {
                        case "Second":
                            {
                                timeSpanSeconds = 1;
                                break;
                            }
                        case "Minute":
                            {
                                timeSpanSeconds = TimeSpan.FromMinutes(1).TotalSeconds;
                                break;
                            }
                        case "Hour":
                            {
                                timeSpanSeconds = TimeSpan.FromHours(1).TotalSeconds;
                                break;
                            }
                        case "Day":
                            {
                                timeSpanSeconds = TimeSpan.FromDays(1).TotalSeconds;
                                break;
                            }
                        case "Week":
                            {
                                timeSpanSeconds = TimeSpan.FromDays(7).TotalSeconds;
                                break;
                            }
                        case "Month":
                            {
                                timeSpanSeconds = TimeSpan.FromDays(30).TotalSeconds;
                                break;
                            }
                    }
                    //��λ��������
                    timeSpanSeconds *= TimeSpanNumber.Value;
                    var totalPointCount = (SearchCondition.EndTime - SearchCondition.BeginTime).TotalSeconds / timeSpanSeconds;
                    if (totalPointCount > 200)
                    {
                        MessageBox.Show(string.Format("ѡ��������Ȼ���� {0} ���㣬����̫ϸ�����������ȣ�", totalPointCount));
                        return null;
                    }
                    return TimeSpan.FromSeconds(timeSpanSeconds);
                }
            }
            return null;
        }

        /// <summary>
        /// ����˵�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ls = sender as LineSeries;
            if (ls != null)
            {
                var si = ls.SelectedItem as StatisticsItem;
                if (si != null)
                {
                    //�����б���ͼ
                    ViewListData c = new ViewListData();
                    var root = Application.Current.RootVisual as FrameworkElement;
                    c.SearchCondition = new SearchCondition
                    {
                        DatabasePrefix = SearchCondition.DatabasePrefix,
                        TableNames = new List<string> { ls.Tag.ToString() },
                        Filters = SearchCondition.Filters,
                        BeginTime = si.BeginTime,
                        EndTime = si.EndTime,
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
                        TableName = ls.Tag.ToString(),
                        Action = "�鿴�б���ͼ",
                        ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
                          si.BeginTime,
                          si.EndTime,
                          SearchCondition.Filters.GetFilterText())
                    });
                }
            }
            ls.SelectedItem = null;
        }

        private void AutoRefreshAction(object state)
        {
            while (enable)
            {
                Thread.Sleep(100);

                while (autoRefresh)
                {
                    TimeSpan? span = null;
                    Dispatcher.BeginInvoke(() => span = GetTimeSpan());
                    Thread.Sleep(500);
                    if (span != null)
                    {
                        Dispatcher.BeginInvoke(() => ShowData());
                        Thread.Sleep(span.Value);
                    }
                    else
                    {
                        autoRefresh = false;

                        Dispatcher.BeginInvoke(() =>
                            {
                                AutoRefresh.IsChecked = false; AutoRefresh.Content = "�����Զ�ˢ��";
                            });
                    }
                }
            }
        }

        private void service_GetStatisticsDataCompleted(object sender, GetStatisticsDataCompletedEventArgs e)
        {
            if (autoRefreshThread == null)
            {
                autoRefreshThread = new Thread(this.AutoRefreshAction)
                {
                    IsBackground = true,
                };
                autoRefreshThread.Start();
            }
            Refresh.IsEnabled = true;
            this.Busy.IsBusy = false;

            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }

            if (e.Result != null)
            {
                statData = e.Result.ToList();

                Chart.Title = string.Format("���ݿ�: {0}  ��ѯʱ��Σ�{1} - {2} ���ݵ�������{3} ������������{4}", SearchCondition.DatabasePrefix, 
                    SearchCondition.BeginTime.ToString("yyyy/MM/dd HH:mm:ss"), SearchCondition.EndTime.ToString("yyyy/MM/dd HH:mm:ss"), statData.SelectMany(s=>s.StatisticsItems).Count(), statData.SelectMany(s=>s.StatisticsItems).Sum(s=>s.Value));
                //��ʼ��ʱ����ĸ�ʽ����ͬ������ѡ��ͬ�ĸ�ʽ
                DateTimeAxis dta = Chart.Axes.OfType<DateTimeAxis>().FirstOrDefault();
                var span = (TimeSpanType.SelectedItem as ComboBoxItem).Tag.ToString();
                Style dtaLabelStyle = new Style(typeof(DateTimeAxisLabel));
                switch (span)
                {
                    case "Second":
                    case "Minute":
                        {
                            dtaLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.StringFormatProperty, "{0:HH:mm}"));
                            break;
                        }
                    case "Hour":
                        {
                            dtaLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.StringFormatProperty, "{0:MM/dd HH.}"));
                            break;
                        }
                    case "Day":
                    case "Week":
                        {
                            dtaLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.StringFormatProperty, "{0:MM/dd}"));
                            break;
                        }
                    case "Month":
                        {
                            dtaLabelStyle.Setters.Add(new Setter(DateTimeAxisLabel.StringFormatProperty, "{0:yyyy/MM/dd}"));
                            break;
                        }
                }
                dta.AxisLabelStyle = dtaLabelStyle;


                //��һ����Ҫ��ʼ��ͼ��
                if (Chart.Series.Count == 0)
                {
                    for (int i = 0; i < statData.Count; i++)
                    {
                        var item = statData[i];
                        LineSeries ls = new LineSeries
                        {
                            Title = item.TableName,
                            Tag = item.TableName,
                            ItemsSource = item.StatisticsItems,
                            IndependentValuePath = "EndTime",
                            DependentValuePath = "Value",
                            IsSelectionEnabled = true,
                        };

                        ls.SelectionChanged += new SelectionChangedEventHandler(ls_SelectionChanged);
                        ls.MouseMove += new MouseEventHandler(ls_MouseMove);

                        //�����ʽ
                        Style style = new Style(typeof(LineDataPoint));
                        style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(colorPool[i])));
                        style.Setters.Add(new Setter(LineDataPoint.BorderThicknessProperty, new Thickness(0)));
                        style.Setters.Add(new Setter(LineDataPoint.CursorProperty, Cursors.Hand));
                        style.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                        style.Setters.Add(new Setter(LineDataPoint.TemplateProperty, Application.Current.Resources["LineNoTransition"] as ControlTemplate));
                        ls.DataPointStyle = style;

                        //��������ʽ
                        Style style2 = new Style(typeof(Polyline));
                        style2.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 1));
                        style2.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                        ls.PolylineStyle = style2;
                        Chart.Series.Add(ls);
                    }
                }
                else //������������Դ�Ϳ�����
                {
                    for (int i = 0; i < e.Result.Count; i++)
                    {
                        var item = e.Result[i];
                        var ls = Chart.Series.OfType<LineSeries>().FirstOrDefault(s => s.Title.ToString() == item.TableName);
                        if (ls != null)
                            ls.ItemsSource = item.StatisticsItems;
                    }
                }

                Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(legendItem =>
                {
                    legendItem.MouseLeftButtonDown += new MouseButtonEventHandler(legendItem_MouseLeftButtonDown);
                });

                //����y������߶�
                var max = statData.SelectMany(r => r.StatisticsItems).Select(p => p.Value).Max();
                var axes = Chart.Axes.ToList().OfType<LinearAxis>().Where(a => a.Orientation == AxisOrientation.Y).FirstOrDefault();
                axes.Maximum = max + max * 0.1;
            }
        }

        private bool bridge = true;
        private void legendItem_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (Chart.Series.OfType<LineSeries>().Count() > 1)
            {
                LegendItem li = sender as LegendItem;
                if (li != null)
                {
                    string tableName = li.Content as string;
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        //ȡ����������
                        Chart.Series.OfType<LineSeries>().ToList().ForEach(ls =>
                            {
                                Style style2 = new Style(typeof(Polyline));
                                style2.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 1));
                                style2.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                                ls.PolylineStyle = style2;
                            });
                        //������괥����˵�����ֵ�����Ϊ����
                        Chart.Series.OfType<LineSeries>().Where(ls => ls.Tag.ToString().TrimEnd() == tableName.TrimEnd()).ToList().ForEach(ls =>
                            {
                                Style style2 = new Style(typeof(Polyline));
                                style2.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 3));
                                style2.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                                ls.PolylineStyle = style2;
                            });
                        if (bridge)
                        {
                            Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.Content = l.Content + " ");
                        }
                        else
                        {
                            Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.Content = l.Content.ToString().TrimEnd());
                        }
                        bridge = !bridge;

                        //������괥�����������ߵ�˵������Ϊ����
                        Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.FontWeight = FontWeights.Normal);
                        Chart.LegendItems.OfType<LegendItem>().Where(l => l.Content.ToString().TrimEnd() == tableName.TrimEnd()).ToList().ForEach(l => l.FontWeight = FontWeights.Bold);
                    }
                }
            }
        }

        private void ls_MouseMove(object sender, MouseEventArgs e)
        {
            //���ֻ��һ���Ļ��Ͳ�Ҫ�Ӵ���
            if (Chart.Series.OfType<LineSeries>().Count() > 1)
            {
                Polyline p = e.OriginalSource as Polyline;
                if (p != null)
                {
                    //ȡ����������
                    Chart.Series.OfType<LineSeries>().ToList().ForEach(ls =>
                    {
                        Style style2 = new Style(typeof(Polyline));
                        style2.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 1));
                        style2.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                        ls.PolylineStyle = style2;
                    });
                    //������괥����˵�����ֵ�����Ϊ����
                    Chart.Series.OfType<LineSeries>().Where(ls => ls.Tag.ToString() == p.Tag.ToString()).ToList().ForEach(ls =>
                    {
                        Style style2 = new Style(typeof(Polyline));
                        style2.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 3));
                        style2.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                        ls.PolylineStyle = style2;
                    });
                    if (bridge)
                    {
                        Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.Content = l.Content + " ");
                    }
                    else
                    {
                        Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.Content = l.Content.ToString().TrimEnd());
                    }
                    bridge = !bridge;
                    //ȡ�������Ĵ���
                    Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(l => l.FontWeight = FontWeights.Normal);

                    //������괥�����������ߵ�˵������Ϊ����
                    Chart.LegendItems.OfType<LegendItem>().Where(l => l.Content.ToString().TrimEnd() == p.Tag.ToString()).ToList().ForEach(l => l.FontWeight = FontWeights.Bold);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh.IsEnabled = false;
            AutoRefresh.IsChecked = false;
            AutoRefresh_Click(AutoRefresh, null);
            ShowData();
        }

        private void AutoRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (AutoRefresh.IsChecked.HasValue)
            {
                if (AutoRefresh.IsChecked.Value)
                {
                    autoRefresh = true;
                    AutoRefresh.Content = "�ر��Զ�ˢ��";
                }
                else
                {
                    autoRefresh = false;
                    AutoRefresh.Content = "�����Զ�ˢ��";
                }
            }
        }

        private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            enable = false; //��������̣߳������һֱ����
            autoRefresh = false;
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            SetTimeSpan();
        }

        private void TimeSpanNumber_MouseEnter(object sender, MouseEventArgs e)
        {
            TimeSpanNumber.Focus();
        }

        private void List_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ViewListData c = new ViewListData();
            var root = Application.Current.RootVisual as FrameworkElement;
            c.SearchCondition = new SearchCondition
            {
                DatabasePrefix = SearchCondition.DatabasePrefix,
                BeginTime = SearchCondition.BeginTime,
                EndTime = SearchCondition.EndTime,
                Filters = SearchCondition.Filters,
                TableNames = SearchCondition.TableNames,
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
                Action = "�鿴�б���ͼ",
                ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
                  SearchCondition.BeginTime,
                  SearchCondition.EndTime,
                  SearchCondition.Filters)
            });
        }

        private void Group_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ViewGroupData c = new ViewGroupData();
            var root = Application.Current.RootVisual as FrameworkElement;
            //var tableName = (SearchCondition.TableNames.Take(12) as TabItem).Tag.ToString();
            c.SearchCondition = new SearchCondition
            {
                DatabasePrefix = SearchCondition.DatabasePrefix,
                BeginTime = SearchCondition.BeginTime,
                EndTime = SearchCondition.EndTime,
                Filters = SearchCondition.Filters,
                TableNames = SearchCondition.TableNames,
                //SelectedTableName = tableName,
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
                Action = "�鿴������ͼ",
                ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
                  SearchCondition.BeginTime,
                  SearchCondition.EndTime,
                  SearchCondition.Filters)
            });
        }

        private void Sum_Click(object sender, RoutedEventArgs e)
        {
            var sum = new Statistics
            {
                TableName = "Sum_Data",
                StatisticsItems = new List<StatisticsItem>(),
            };
            foreach (var table in statData)
            {
                if (table.TableName != "Avg_Data")
                {
                    foreach (var point in table.StatisticsItems)
                    {
                        if (sum.StatisticsItems.Count(a => a.EndTime == point.EndTime) == 0)
                        {
                            sum.StatisticsItems.Add(new StatisticsItem
                            {
                                BeginTime = point.BeginTime,
                                EndTime = point.EndTime,
                                Value = point.Value,
                            });
                        }
                        else
                        {
                            var old = sum.StatisticsItems.SingleOrDefault(a => a.EndTime == point.EndTime);
                            old.Value += point.Value;
                        }
                    }
                }
            }
            var data = statData.FirstOrDefault(s => s.TableName == "Sum_Data");
            if (data != null)
            {
                statData.Remove(data);
            }
            else
            {
                statData.Add(sum);
            }
            Chart.Series.Clear();
            for (int i = 0; i < statData.Count; i++)
            {
                var item = statData[i];
                LineSeries ls = new LineSeries
                {
                    Title = item.TableName,
                    Tag = item.TableName,
                    ItemsSource = item.StatisticsItems,
                    IndependentValuePath = "EndTime",
                    DependentValuePath = "Value",
                    IsSelectionEnabled = true,
                };
                //ls.SelectionChanged += new SelectionChangedEventHandler(ls_SelectionChanged);
                ls.MouseMove += new MouseEventHandler(ls_MouseMove);

                //�����ʽ
                Style style = new Style(typeof(LineDataPoint));
                style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(colorPool[i])));
                style.Setters.Add(new Setter(LineDataPoint.BorderThicknessProperty, new Thickness(0)));
                style.Setters.Add(new Setter(LineDataPoint.CursorProperty, Cursors.Hand));
                style.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                style.Setters.Add(new Setter(LineDataPoint.TemplateProperty, Application.Current.Resources["LineNoTransitionAdd"] as ControlTemplate));
                ls.DataPointStyle = style;

                //��������ʽ
                Style style2 = new Style(typeof(Polyline));
                style2.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 1));
                style2.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                ls.PolylineStyle = style2;
                Chart.Series.Add(ls);
                Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(legendItem =>
                    {
                        legendItem.MouseLeftButtonDown += new MouseButtonEventHandler(legendItem_MouseLeftButtonDown);
                    });
                //����y������߶�
                var max = statData.SelectMany(r => r.StatisticsItems).Select(p => p.Value).Max();
                var axes = Chart.Axes.ToList().OfType<LinearAxis>().Where(a => a.Orientation == AxisOrientation.Y).FirstOrDefault();
                axes.Maximum = max + max * 0.1;
            }
        }

        private void AVG_Click(object sender, RoutedEventArgs e)
        {
            var avg = new Statistics
            {
                TableName = "Avg_Data",
                StatisticsItems = new List<StatisticsItem>(),
            };
            foreach (var table in statData)
            {
                if (table.TableName != "Sum_Data")
                foreach (var point in table.StatisticsItems)
                { 
                    if (avg.StatisticsItems.Count(a => a.EndTime == point.EndTime) == 0)
                    {
                        avg.StatisticsItems.Add(new StatisticsItem
                        {
                            BeginTime = point.BeginTime,
                            EndTime = point.EndTime,
                            Value = point.Value / statData.Count,
                        });
                    }
                    else
                    {
                        var old = avg.StatisticsItems.SingleOrDefault(a => a.EndTime == point.EndTime);
                        old.Value += point.Value;
                    }
                }
                
            }
            foreach (var item in avg.StatisticsItems)
            {
                item.Value = item.Value / statData.Count;
            }
            var data = statData.FirstOrDefault(s => s.TableName == "Avg_Data");
            if (data != null)
            {
                statData.Remove(data);
            }
            else
            {
                statData.Add(avg);
            }
            Chart.Series.Clear();
            for (int i = 0; i < statData.Count; i++)
            {
                var item = statData[i];
                LineSeries ls = new LineSeries
                {
                    Title = item.TableName,
                    Tag = item.TableName,
                    ItemsSource = item.StatisticsItems,
                    IndependentValuePath = "EndTime",
                    DependentValuePath = "Value",
                    IsSelectionEnabled = true,
                };
                //ls.SelectionChanged += new SelectionChangedEventHandler(ls_SelectionChanged);
                ls.MouseMove += new MouseEventHandler(ls_MouseMove);

                //�����ʽ
                Style style = new Style(typeof(LineDataPoint));
                style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(colorPool[i])));
                style.Setters.Add(new Setter(LineDataPoint.BorderThicknessProperty, new Thickness(0)));
                style.Setters.Add(new Setter(LineDataPoint.CursorProperty, Cursors.Hand));
                style.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                style.Setters.Add(new Setter(LineDataPoint.TemplateProperty, Application.Current.Resources["LineNoTransitionAdd"] as ControlTemplate));
                ls.DataPointStyle = style;

                //��������ʽ
                Style style2 = new Style(typeof(Polyline));
                style2.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 1));
                style2.Setters.Add(new Setter(Polyline.TagProperty, ls.Tag));
                ls.PolylineStyle = style2;
                Chart.Series.Add(ls);
            }
            Chart.LegendItems.OfType<LegendItem>().ToList().ForEach(legendItem =>
                {
                    legendItem.MouseLeftButtonDown += new MouseButtonEventHandler(legendItem_MouseLeftButtonDown);
                });
            //����y������߶�
            var max = statData.SelectMany(r => r.StatisticsItems).Select(p => p.Value).Max();
            var axes = Chart.Axes.ToList().OfType<LinearAxis>().Where(a => a.Orientation == AxisOrientation.Y).FirstOrDefault();
            axes.Maximum = max + max * 0.1;
        }

        private void CopyUrlAddress_Click(object sender, RoutedEventArgs e)
        {
            var ud = new UrlData
            {
                Condition = this.SearchCondition,
                Type = "Stat",
            };
            MemoryStream ms = new MemoryStream();
            var serializer = new SharpSerializer(true);
            serializer.Serialize(ud, ms);
            byte[] array = ms.ToArray();
            ms.Close();
            string data = Convert.ToBase64String(array);
            var url = HtmlPage.Document.DocumentUri;
            var urlstring = url.ToString();
            if (url.Query.Length >0)
                urlstring = urlstring.Replace(url.Query, "");
            Clipboard.SetText(string.Format("{0}?url={1}", urlstring, data));
        }

        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size();
        }
    }
}