using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Adhesive.Mongodb.Silverlight.Service;
using System.ServiceModel;
using System;
using System.Linq;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class MainPage : UserControl
    {
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));

        public MainPage()
        {
            InitializeComponent();

        }

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (ContentFrame.UriMapper.MapUri(e.Uri).ToString().Equals(ContentFrame.UriMapper.MapUri(hb.NavigateUri).ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.HorizontalAlignment = HorizontalAlignment.Center;
            login.VerticalAlignment = VerticalAlignment.Center;
            login.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(login_Closing);
            login.Show();
        }

        private void login_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var login = sender as Login;
            if (login != null)
            {
                var data = login.Tag as MongodbAdminConfigurationItem;
                if (data != null)
                {
                    Data.AdminConfigurationItem = data;
                    Welcome.Text = string.Format("»¶Ó­Äú£º{0} ({1})", data.RealName, data.IP);
                    if (this.ContentFrame.Content is Home)
                        (this.ContentFrame.Content as Home).Load(null);
                }
            }
        }
    }
}