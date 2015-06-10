using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using Adhesive.Mongodb.Silverlight.Service;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class Alarm : Page
    {
        private bool firstbind = false;
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        public Alarm()
        {
            InitializeComponent();
        }

        // 当用户导航到此页面时执行。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Data.AdminConfigurationItem == null) return;
            this.Busy.IsBusy = true;

            service.GetAlarmItemsAsync(AlarmStatus.Open, Data.AdminConfigurationItem, OpenPager.PageSize, 0);
            firstbind = true;
            service.GetAlarmItemsCompleted += new EventHandler<GetAlarmItemsCompletedEventArgs>(service_GetAlarmItemsCompleted);
            service.GetAlarmGroupCompleted += new EventHandler<GetAlarmGroupCompletedEventArgs>(service_GetAlarmGroupCompleted);
            service.GetAlarmReceiversCompleted += new EventHandler<GetAlarmReceiversCompletedEventArgs>(service_GetAlarmReceiversCompleted);
            service.SendMobileCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(service_SendMobileCompleted);
            service.SendEmailCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(service_SendEmailCompleted);

        }

        void service_SendEmailCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Notice.Text = "邮件发送成功!";
        }

        void service_SendMobileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Notice.Text = "短信发送成功!";
        }

        void service_GetAlarmReceiversCompleted(object sender, GetAlarmReceiversCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }
            if (e.Result != null)
            {
                ReceiversTitle.Text = Group.Tag.ToString();
                ReceiversGrid.ItemsSource = e.Result.ToList();
                ReceiversGrid.Tag = e.Result;
                ReceiversGrid.Visibility = Visibility.Visible;
            }
            if (membercheck == true)
            {
                Receivers r = new Receivers() { UserName = new List<string>() };
                r.GropuName = Group.Tag.ToString();
                foreach (var a in e.Result.ToList())
                {
                    r.UserName.Add(a.UserName);
                }
                var data = receiver.FirstOrDefault(s => s.GropuName == Group.Tag.ToString());
                if (data != null)
                {
                    receiver.Remove(data);
                }
                else
                {
                    receiver.Add(r);
                }
            }
            if (membercheck == false)
            {
                var data = receiver.FirstOrDefault(s => s.GropuName == Group.Tag.ToString());
                if (data != null)
                {
                    receiver.Remove(data);
                }
            }
        }

        void service_GetAlarmGroupCompleted(object sender, GetAlarmGroupCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }
            if (e.Result != null)
            {
                Group.Children.Clear();
                TextBlock title = new TextBlock
                {
                    Text = "分   组",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 20,
                };
                Group.Children.Add(title);

                foreach (var a in e.Result)
                {
                    CheckBox member = new CheckBox
                    {
                        Content = a,
                        FontSize = 14,
                        Margin = new Thickness(0, 5, 0, 0),
                    };
                    member.Click += new RoutedEventHandler(member_Click);
                    if (member != null)
                    {
                        Group.Children.Add(member);
                    }
                }
            }
        }

        public bool membercheck;
        void member_Click(object sender, RoutedEventArgs e)
        {
            var member = sender as CheckBox;
            if (member != null)
            {
                if (member.IsChecked == true)
                {
                    membercheck = true;
                }
                if (member.IsChecked == false)
                {
                    membercheck = false;
                }
                service.GetAlarmReceiversAsync(member.Content.ToString());
                Group.Tag = member.Content;
            }
        }

        List<Receivers> receiver = new List<Receivers>();
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Receivers r = new Receivers() { UserName = new List<string>() };
            var count = 0;
            var counttrue = 0;
            foreach (object ovj in ReceiversGrid.ItemsSource)
            {
                CheckBox cb = ReceiversGrid.Columns[4].GetCellContent(ovj).FindName("ck") as CheckBox;
                count++;
                if (cb.IsChecked.Value)
                {
                    counttrue++;
                    r.GropuName = Group.Tag.ToString();
                    r.UserName.Add(cb.Tag.ToString());
                }
            }
            var data = receiver.FirstOrDefault(s => s.GropuName == Group.Tag.ToString());
            if (data != null)
            {
                receiver.Remove(data);
            }
            if (counttrue != 0)
            {
                receiver.Add(r);
            }
            Group.Children.OfType<CheckBox>().ToList().ForEach(member =>
                {
                    if (member.Content.ToString() == Group.Tag.ToString())
                    {
                        if (counttrue == count)
                        {
                            member.IsChecked = true;
                        }
                        else if (counttrue == 0)
                        {
                            member.IsChecked = false;
                        }
                        else
                        {
                            member.IsChecked = null;
                        }
                    }
                });
        }

        private void ReceiversGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            CheckBox cb = ReceiversGrid.Columns[4].GetCellContent(e.Row).FindName("ck") as CheckBox;
            if (membercheck == true)
            {
                cb.IsChecked = true;
            }
            if (membercheck == false)
            {
                cb.IsChecked = false;
            }
        }

        void service_GetAlarmItemsCompleted(object sender, GetAlarmItemsCompletedEventArgs e)
        {
            this.Busy.IsBusy = false;
            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }
            if (e.Result != null)
            {
                if (tabControl1 != null)
                {
                    if (tabControl1.SelectedIndex == 0)
                    {
                        var count = e.Result.Count;
                        if (count == 0)
                        {
                            OpenGrid.Width = 1050;
                        }
                        else
                        {
                            OpenGrid.Width = double.NaN;
                        }
                        if (firstbind == true)
                        {
                            PagedCollectionView pcv = new PagedCollectionView(Enumerable.Range(0, count));
                            OpenPager.Source = pcv;
                            OpenPager2.Source = pcv;
                            firstbind = false;
                        }
                        OpenGrid.ItemsSource = e.Result.Data.ToList();
                        OpenGrid.Tag = e.Result.Data;
                        OpenCount.Text = string.Format("总页数：{0}    总记录数：{1}", OpenPager.PageCount, count);
                        OpenCount2.Text = string.Format("总页数：{0}    总记录数：{1}", OpenPager.PageCount, count);
                    }
                    if (tabControl1.SelectedIndex == 1)
                    {
                        var count = e.Result.Count;
                        if (count == 0)
                        {
                            HandleGrid.Width = 1050;
                        }
                        else
                        {
                            HandleGrid.Width = double.NaN;
                        }
                        if (firstbind == true)
                        {
                            PagedCollectionView pcv = new PagedCollectionView(Enumerable.Range(0, count));
                            HandlePager.Source = pcv;
                            HandlePager2.Source = pcv;
                            firstbind = false;
                        }
                        HandleGrid.ItemsSource = e.Result.Data.ToList();
                        HandleGrid.Tag = e.Result.Data;
                        HandleCount.Text = string.Format("总页数：{0}    总记录数：{1}", HandlePager.PageCount, count);
                        HandleCount2.Text = string.Format("总页数：{0}    总记录数：{1}", HandlePager.PageCount, count);

                    }
                    if (tabControl1.SelectedIndex == 2)
                    {
                        var count = e.Result.Count; if (count == 0)
                        {
                            ClosedGrid.Width = 1050;
                        }
                        else
                        {
                            ClosedGrid.Width = double.NaN;
                        }
                        if (firstbind == true)
                        {
                            PagedCollectionView pcv = new PagedCollectionView(Enumerable.Range(0, count));
                            ClosedPager.Source = pcv;
                            ClosedPager2.Source = pcv;
                            firstbind = false;
                        }
                        ClosedGrid.ItemsSource = e.Result.Data.ToList();
                        ClosedGrid.Tag = e.Result.Data;
                        ClosedCount.Text = string.Format("总页数：{0}    总记录数：{1}", ClosedPager.PageCount, count);
                        ClosedCount2.Text = string.Format("总页数：{0}    总记录数：{1}", ClosedPager.PageCount, count);
                    }
                }
            }
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bind();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var abc = OpenGrid.Tag as List<Table>;
            var obj = OpenGrid.SelectedItem;
            if (tabControl1 != null)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    abc = OpenGrid.Tag as List<Table>;
                    obj = OpenGrid.SelectedItem;
                }
                if (tabControl1.SelectedIndex == 1)
                {
                    abc = HandleGrid.Tag as List<Table>;
                    obj = HandleGrid.SelectedItem;
                }
                if (tabControl1.SelectedIndex == 2)
                {
                    abc = ClosedGrid.Tag as List<Table>;
                    obj = ClosedGrid.SelectedItem;
                }
            }
            var root = Application.Current.RootVisual as FrameworkElement;
            AlarmDetail c = new AlarmDetail(obj);
            //设置子窗口的宽度和长度
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            //子窗口全屏居中
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Closed += new EventHandler(c_Closed);
            c.finish.Click += new RoutedEventHandler(finish_Click);
            c.change.Click += new RoutedEventHandler(change_Click);
            c.Show();
        }

        void change_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        void finish_Click(object sender, RoutedEventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }
        int pageindex;
        public void bind()
        {
            if (tabControl1 != null)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    if (OpenPager.PageIndex < 0)
                    {
                        pageindex = 0;
                        firstbind = true;
                    }
                    else
                    {
                        pageindex = OpenPager.PageIndex;
                    }
                    service.GetAlarmItemsAsync(AlarmStatus.Open, Data.AdminConfigurationItem, OpenPager.PageSize, pageindex);
                }
                if (tabControl1.SelectedIndex == 1)
                {
                    if (HandlePager.PageIndex < 0)
                    {
                        pageindex = 0;
                        firstbind = true;

                    }
                    else
                    {
                        pageindex = HandlePager.PageIndex;

                    }
                    service.GetAlarmItemsAsync(AlarmStatus.Handling, Data.AdminConfigurationItem, HandlePager.PageSize, pageindex);

                }
                if (tabControl1.SelectedIndex == 2)
                {
                    if (ClosedPager.PageIndex < 0)
                    {
                        pageindex = 0;
                        firstbind = true;
                    }
                    else
                    {
                        pageindex = ClosedPager.PageIndex;
                    }
                    service.GetAlarmItemsAsync(AlarmStatus.Closed, Data.AdminConfigurationItem, ClosedPager.PageSize, pageindex);
                }
                if (tabControl1.SelectedIndex == 3)
                {
                    service.GetAlarmGroupAsync();
                }
            }
        }

        private void c_Closed(object sender, EventArgs e)
        {
            bind();
        }

        private void OpenPager_PageIndexChanged(object sender, EventArgs e)
        {
            if (OpenPager.PageIndex < 0)
                return;
            if (firstbind == true && OpenPager.PageIndex == 0)
                return;
            bind();
        }
        private void HandlePager_PageIndexChanged(object sender, EventArgs e)
        {
            if (HandlePager.PageIndex < 0)
                return;
            if (firstbind == true && HandlePager.PageIndex == 0)
                return;
            bind();
        }

        private void ClosedPager_PageIndexChanged(object sender, EventArgs e)
        {
            if (ClosedPager.PageIndex < 0)
                return;
            if (firstbind == true && ClosedPager.PageIndex == 0)
                return;
            bind();
        }

        private void SendMobile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageContent.Text))
            {
                MessageBox.Show("消息内容不能为空");
            }
            else
            {
                if (MessageContent.Text.Length > 50)
                {
                    MessageBox.Show("短信内容最多为50字");
                }
                else
                {
                    if (receiver.Count != 0)
                    {
                        List<string> username = new List<string>();
                        foreach (var r in receiver)
                        {
                            foreach (var u in r.UserName)
                            {
                                if (!username.Contains(u))
                                {
                                    username.Add(u);
                                }
                            }
                        }
                        service.SendMobileAsync(username, MessageContent.Text);
                    }
                    if (receiver.Count == 0)
                    {
                        MessageBox.Show("请至少选择一组");
                    }
                }
            }
        }

        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageContent.Text))
            {
                MessageBox.Show("消息内容不能为空");
            }
            else
            {
                if (receiver.Count != 0)
                {
                    List<string> username = new List<string>();
                    foreach (var r in receiver)
                    {
                        foreach (var u in r.UserName)
                        {
                            if (username.Contains(u))
                            {
                                username.Add(u);
                            }
                        }
                    }
                    service.SendEmailAsync(username, MessageContent.Text);
                }
                if (receiver.Count == 0)
                {
                    MessageBox.Show("请至少选择一组");
                }
            }
        }
    }
    class Receivers
    {
        public string GropuName { get; set; }

        public List<string> UserName { get; set; }

    }
}
