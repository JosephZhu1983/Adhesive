using System.Windows;
using System.Windows.Controls;

namespace Adhesive.Mongodb.Client.UI
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var a = (Application.Current.RootVisual as FrameworkElement);
            var c = new ChildWindow1();
            c.Width = a.ActualWidth * 0.8;
            c.Height = a.ActualHeight * 0.8;
            c.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            c.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            c.Show();
        }
    }
}
