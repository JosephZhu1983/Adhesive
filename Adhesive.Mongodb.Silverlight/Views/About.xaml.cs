using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();

        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                tv.Items.Add(new TreeViewItem
                {
                    Header = new string('a', 100),
                });
            }
        }
    }
}