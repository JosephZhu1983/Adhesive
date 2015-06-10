using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using Adhesive.Mongodb.Silverlight.Service;
using System.Threading;
using System.IO;
using Polenter.Serialization;
using System.Windows.Browser;
using System.Windows.Media;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class ViewStateData : ChildWindow
    {
        public StateCondition StateCondition { get; set; }
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
        private int treeDepth = 1;
        private bool autoRefresh = false;
        private bool enable = true;
        private Thread autoRefreshThread;

        public ViewStateData()
        {
            InitializeComponent();
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Size();

            if (StateCondition != null)
            {
                RangeDetail.Text = string.Format("数据库: {0}  当前时间：{1} ", StateCondition.DatabasePrefix, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            service.GetStateDataCompleted += new EventHandler<GetStateDataCompletedEventArgs>(service_GetStateDataCompleted);
            ShowData();
        }

        private void Size()
        {
            ContentStackPanel.Width = this.Width - 100;
            DataTree.Height = this.Height - 100;
        }

        private void service_GetStateDataCompleted(object sender, GetStateDataCompletedEventArgs e)
        {
            this.Busy.IsBusy = false;

            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }

            if (e.Result != null && e.Result.Data != null)
            {
                BindData(e.Result);
            }

            if (autoRefreshThread == null)
            {
                autoRefreshThread = new Thread(this.AutoRefreshAction)
                {
                    IsBackground = true,
                };
                autoRefreshThread.Start();
            }
        }

        private void ShowData()
        {
            if (StateCondition != null)
            {
                this.Busy.IsBusy = true;
                service.GetStateDataAsync(StateCondition.DatabasePrefix, StateCondition.TableName, DateTime.Now.AddMonths(-1), DateTime.Now, StateCondition.Filters);
            }
            else
            {
                MessageBox.Show("错误！没有获取到查询参数！");
                this.Close();
            }
        }

        private void AutoRefreshAction(object state)
        {
            while (enable)
            {
                Thread.Sleep(100);

                while (autoRefresh)
                {
                    Dispatcher.BeginInvoke(() => ShowData());
                    Thread.Sleep(GetRefreshInterval());
                }
            }
        }

        private TimeSpan GetRefreshInterval()
        {
            int i = 10;
            Dispatcher.BeginInvoke(() => int.TryParse(RefreshInterval.Text, out i));
            Thread.Sleep(500);
            if (i < 1) i = 1;
            return TimeSpan.FromSeconds(i);
        }


        private void BindData(DetailData result)
        {
            var data = new List<Detail>();
            //把没有下级的排在前面
            data.AddRange(result.Data.Where(d => d.SubDetails == null).ToList());
            data.AddRange(result.Data.Where(d => d.SubDetails != null).ToList());
            var root = new TreeViewItem();
            var panel = new WrapPanel();
            panel.Children.Add(new TextBlock { Text = "数据库名：", FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBox { Text = result.DatabaseName, Margin = new Thickness(5, 0, 0, 0), TextWrapping = TextWrapping.Wrap, BorderBrush = new SolidColorBrush(Colors.White), Padding = new Thickness(0) });
            panel.Children.Add(new TextBlock { Text = "表名：", FontWeight = FontWeights.Bold, Margin = new Thickness(5, 0, 0, 0) });
            panel.Children.Add(new TextBox { Text = result.TableName, Margin = new Thickness(5, 0, 0, 0), TextWrapping = TextWrapping.Wrap, BorderBrush = new SolidColorBrush(Colors.White), Padding = new Thickness(0) });
            root.Header = panel;
            root.IsExpanded = true;
            BindTree(root, data);
            DataTree.Items.Clear();
            DataTree.Items.Add(root);
        }

        private void BindTree(TreeViewItem item, List<Detail> data)
        {
            foreach (var detail in data)
            {
                var panel = new WrapPanel();
                panel.Children.Add(new TextBlock { Text = detail.DisplayName, FontWeight = FontWeights.Bold });
                panel.Children.Add(new TextBox { Text = detail.Value, Margin = new Thickness(5, 0, 0, 0), TextWrapping = TextWrapping.Wrap, Width = 1050,BorderBrush = new SolidColorBrush(Colors.White), Padding = new Thickness(0)});
                if (!string.IsNullOrEmpty(detail.Description))
                    panel.Children.Add(new TextBox { Text = detail.Description, Margin = new Thickness(5, 0, 0, 0), FontStyle = FontStyles.Italic, TextWrapping = TextWrapping.Wrap, Width = 1050,BorderBrush = new SolidColorBrush(Colors.White), Padding = new Thickness(0)});
                var subItem = new TreeViewItem
                {
                    Header = panel,
                };
                if (detail.SubDetails != null)
                {
                    BindTree(subItem, detail.SubDetails);
                }
                item.Items.Add(subItem);
            }
        }

        private void Expand_Click(object sender, RoutedEventArgs e)
        {
            if (Expand.IsChecked.HasValue && Expand.IsChecked.Value)
            {
                DataTree.ExpandAll();
                Expand.Content = "收缩所有";
            }
            else
            {
                DataTree.CollapseAll();
                Expand.Content = "展开所有";
            }
        }

        private void ExpandNextLevel_Click(object sender, RoutedEventArgs e)
        {
            treeDepth++;
            DataTree.ExpandToDepth(treeDepth);
        }

        private void CollapseNextLevel_Click(object sender, RoutedEventArgs e)
        {
            if (treeDepth >= 0)
            {
                treeDepth--;
                DataTree.CollapseAll();
                DataTree.ExpandToDepth(treeDepth);
            }
        }

        private void AutoRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (AutoRefresh.IsChecked.HasValue)
            {
                if (AutoRefresh.IsChecked.Value)
                {
                    autoRefresh = true;
                    AutoRefresh.Content = "关闭自动刷新";
                }
                else
                {
                    autoRefresh = false;
                    AutoRefresh.Content = "开启自动刷新";
                }
            }
        }

        private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            enable = false; //结束这个线程，否则会一直存在
            autoRefresh = false;
        }

        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size();
        }

        private void CopyUrlAddress_Click(object sender, RoutedEventArgs e)
        {
            var ud = new UrlData
            {
                Condition = this.StateCondition,
                Type = "State",
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
    }
}

