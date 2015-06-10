using System.Windows;
using System.Windows.Controls;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class App : Application
    {
        public App()
        {
            this.Startup += this.Application_Startup;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.RootVisual = new MainPage();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
            var root = Application.Current.RootVisual as FrameworkElement;
            errorWin.Width = root.ActualWidth * 0.95;
            errorWin.Height = root.ActualHeight * 0.95;
            errorWin.HorizontalAlignment = HorizontalAlignment.Center;
            errorWin.VerticalAlignment = VerticalAlignment.Center;
            errorWin.Show();
        }
    }
}