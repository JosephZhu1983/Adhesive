using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Adhesive.Mongodb.Silverlight.Service;
using System.IO;
using Polenter.Serialization;
using System.Windows.Browser;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class ViewDetailData : ChildWindow
    {
        public DetailCondition DetailCondition { get; set; }
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
        private int treeDepth = 1;

        public ViewDetailData()
        {
            InitializeComponent();
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Size();

            service.GetDetailDataCompleted += new EventHandler<GetDetailDataCompletedEventArgs>(service_GetDetailDataCompleted);
            service.GetDetailDataOnlyByIdCompleted += new EventHandler<GetDetailDataOnlyByIdCompletedEventArgs>(service_GetDetailDataOnlyByIdCompleted);

            ShowData();

            
        }

        private void Size()
        {
            ContentStackPanel.Width = this.Width - 100;
            DataTree.Height = this.Height - 100;
        }

        private void service_GetDetailDataOnlyByIdCompleted(object sender, GetDetailDataOnlyByIdCompletedEventArgs e)
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
        }

        private void ShowData()
        {
            if (DetailCondition != null)
            {
                this.Busy.IsBusy = true;
                if (string.IsNullOrEmpty(DetailCondition.TableName))
                {
                    service.GetDetailDataOnlyByIdAsync(DetailCondition.DatabasePrefix, DetailCondition.ID);
                }
                else
                {
                    service.GetDetailDataAsync(DetailCondition.DatabasePrefix, DetailCondition.DatabaseName, DetailCondition.TableName, DetailCondition.PkColumnName, DetailCondition.ID);
                }
            }
            else
            {
                MessageBox.Show("错误！没有获取到查询参数！");
                this.Close();
            }
        }

        private void service_GetDetailDataCompleted(object sender, GetDetailDataCompletedEventArgs e)
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
        }

        private void BindData(DetailData result)
        {
            var data = new List<Detail>();
            //把没有下级的排在前面
            data.AddRange(result.Data.Where(d => d.SubDetails == null).ToList());
            data.AddRange(result.Data.Where(d => d.SubDetails != null).ToList());
            var root = new TreeViewItem();
            var panel = new WrapPanel();
            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            panel.Children.Add(new TextBlock { Text = "数据库名：", FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBlock { Text = result.DatabaseName });
            panel.Children.Add(new TextBlock { Text = "表名：", FontWeight = FontWeights.Bold, Margin = new Thickness(5, 0, 0, 0) });
            panel.Children.Add(new TextBlock { Text = result.TableName });
            panel.Children.Add(new TextBlock { Text = "主键：", FontWeight = FontWeights.Bold, Margin = new Thickness(5, 0, 0, 0) });
            panel.Children.Add(new TextBlock { Text = DetailCondition.ID });
            root.Header = panel;
            root.IsExpanded = true;
            BindTree(root, data);
            DataTree.Items.Add(root);
        }

        private void BindTree(TreeViewItem item, List<Detail> data)
        {
            foreach (var detail in data)
            {
                var panel = new WrapPanel();
                panel.Children.Add(new TextBlock { Text = detail.DisplayName, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center });
                var t = new TextBox
                {
                    Text = detail.Value,
                    Margin = new Thickness(5, 0, 0, 0),
                    TextWrapping = TextWrapping.Wrap,
                    //Width = this.Width - 300,
                    BorderBrush = new SolidColorBrush(Colors.White),
                    Padding = new Thickness(0)
                };
                t.MouseEnter += (s, ee) => { t.Focus(); t.SelectAll(); };
                panel.Children.Add(t);
                if (!string.IsNullOrEmpty(detail.Description))
                    panel.Children.Add(new TextBlock { Text = detail.Description, Margin = new Thickness(5, 0, 0, 0), FontStyle = FontStyles.Italic, TextWrapping = TextWrapping.Wrap, Width = 1050 });
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

        private void ChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size();
        }

        private void CopyUrlAddress_Click(object sender, RoutedEventArgs e)
        {
            var ud = new UrlData
            {
                Condition = this.DetailCondition,
                Type = "Detail",
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

