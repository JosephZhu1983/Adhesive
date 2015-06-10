using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using Adhesive.Mongodb.Silverlight.Service;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class AlarmDetail : ChildWindow
    {
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        object obj;
        public AlarmDetail(object obj)
        {
            InitializeComponent();
            this.obj = obj;
        }
        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Data.AdminConfigurationItem == null) return;
            this.Busy.IsBusy = true;
            //自动选择
            message.MouseEnter += (s, ee) => { message.Focus();};
            mail.MouseEnter += (s, ee) => { mail.Focus();};

            Errormessage.Text = "*";
            Errormail.Text = "*";
            ContentStackPanel.Width = this.Width - 140;
            Welcome.Text = string.Format("{0}/{1}", Data.AdminConfigurationItem.UserName, Data.AdminConfigurationItem.RealName);
            var obj_id = obj as AlarmItem;
            if (obj_id.AlarmStatus.ToString() == "Open")
            {
                DetailTable.Visibility = Visibility.Collapsed;
            }
            if (obj_id.AlarmStatus.ToString() == "Closed")
            {
                DetailChange.Visibility = Visibility.Collapsed;
            }
            else
            {
                DetailTable.Visibility = Visibility.Visible;
                DetailChange.Visibility = Visibility.Visible;
            }
            service.GetAlarmProcessItemAsync(obj_id.Id);
            service.GetAlarmProcessItemCompleted += new EventHandler<GetAlarmProcessItemCompletedEventArgs>(service_GetAlarmProcessItemCompleted);
            service.HandleAlarmEventCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(service_HandleAlarmEventCompleted);
            service.CloseAlarmEventCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(service_CloseAlarmEventCompleted);
        }

        void service_CloseAlarmEventCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("处理完成");
            this.Close();
        }

        void service_HandleAlarmEventCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("接手成功");
            this.Close();
        }

        void service_GetAlarmProcessItemCompleted(object sender, GetAlarmProcessItemCompletedEventArgs e)
        {
            this.Busy.IsBusy = false;
            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }
            DetailGrid.ItemsSource = e.Result.ToList();
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            if (message.Text == "")
            {
                Errormessage.Text = "短信内容不能为空";
                return;
            }
            if (mail.Text == "")
            {
                Errormessage.Text = "*";
                Errormail.Text = "邮件内容不能为空";
                return;
            }
            Errormessage.Text = "*";
            Errormail.Text = "*";
            var obj_id = obj as AlarmItem;
            service.HandleAlarmEventAsync(obj_id.Id, message.Text, mail.Text, Data.AdminConfigurationItem.UserName, Data.AdminConfigurationItem.RealName);
        }

        private void finish_Click(object sender, RoutedEventArgs e)
        {
            if (message.Text == "")
            {
                Errormessage.Text = "*";
                Errormessage.Text = "短信内容不能为空";
                return;
            }
            if (mail.Text == "")
            {
                Errormail.Text = "邮件内容不能为空";
                return;
            }
            Errormessage.Text = "*";
            Errormail.Text = "*";
            var obj_id = obj as AlarmItem;
            service.CloseAlarmEventAsync(obj_id.Id, message.Text, mail.Text, Data.AdminConfigurationItem.UserName, Data.AdminConfigurationItem.RealName);
        }
    }
}

