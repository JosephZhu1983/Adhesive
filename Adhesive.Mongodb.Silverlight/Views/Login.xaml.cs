using System;
using System.IO.IsolatedStorage;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using Adhesive.Mongodb.Silverlight.Service;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class Login : ChildWindow
    {       
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

        public Login()
        {
            InitializeComponent();
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            service.GetAdminConfigurationCompleted += new EventHandler<GetAdminConfigurationCompletedEventArgs>(service_GetAdminConfigurationCompleted);
           
        }

        private void service_GetAdminConfigurationCompleted(object sender, GetAdminConfigurationCompletedEventArgs e)
        {
            LoginButton.IsEnabled = true;

            if (e.Result == null || e.Result.MongodbAdminDatabaseConfigurationItems == null
                || string.IsNullOrEmpty(e.Result.UserName))
            {
                //Ϊ�˷�ֹ�����ε��´������Close�¼�
                MessageBox.Show("��֤ʧ�ܣ�");
                return;
            }
            else
            {
                service.LogAsync(new OperationLog
                {
                    AccountName = e.Result.UserName,
                    AccountRealName = e.Result.RealName,
                    CategoryName = "",
                    DatabaseName = "",
                    TableName = "",
                    Action = "��½",
                    ActionMemo = ""
                });
                this.Tag = e.Result;
                this.Close();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;
            service.GetAdminConfigurationAsync(Username.Text, Password.Password);
        }

        private void ChildWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                LoginButton_Click(this, null);
        }
    }
}

