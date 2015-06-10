using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Adhesive.Mongodb.Silverlight.Service;

namespace Adhesive.Mongodb.Silverlight
{
    public partial class Manage : Page
    {
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

        public Manage()
        {
            InitializeComponent();

        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Data.AdminConfigurationItem == null) return;
            this.Busy.IsBusy = true;
            service.GetServerInfoCompleted += new System.EventHandler<GetServerInfoCompletedEventArgs>(service_GetServerInfoCompleted);
            service.GetServerInfoAsync();           
            ObjectTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(ObjectTree_SelectedItemChanged);
            
        }

        private void Size()
        {
            var root = Application.Current.RootVisual as FrameworkElement;
            ObjectTree.Height = root.ActualHeight - 200;
            ObjectTree.Width = (root.ActualWidth - 100) * 0.3;
            ObjectDetailPanel.Height = root.ActualHeight - 200;
            ObjectDetailPanel.Width = (root.ActualWidth - 100) * 0.6;
        }

        private void ObjectTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ObjectDetailPanel.Children.Clear();

            if ((ObjectTree.SelectedValue as TreeViewItem).Tag == null) return;

            if ((ObjectTree.SelectedValue as TreeViewItem).Tag.GetType().Name == "ServerInfo")
            {
                var serverInfo = (ObjectTree.SelectedValue as TreeViewItem).Tag as ServerInfo;

                var serverBasicInfoExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "������Ϣ",
                    IsExpanded = true,
                };
                var serverBasicInfoPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                var serverBasicInfoPanel1 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };
                serverBasicInfoPanel1.Children.Add(new TextBlock { Text = "��������Ⱥ����", FontWeight = FontWeights.Bold });
                serverBasicInfoPanel1.Children.Add(new TextBlock { Text = serverInfo.Url.Name });
                serverBasicInfoPanel1.Children.Add(new TextBlock { Text = "Master��ַ��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel1.Children.Add(new TextBlock { Text = serverInfo.Url.Master });
                serverBasicInfoPanel1.Children.Add(new TextBlock { Text = "Slave��ַ��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel1.Children.Add(new TextBlock { Text = serverInfo.Url.Slave });

                var serverBasicInfoPanel2 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = "�����ݿ�������", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = serverInfo.Databases.Count.ToString() });
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = "�ܱ�������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = serverInfo.Databases.Select(db => db.DatabaseStatus).Sum(db => db.CollectionCount).ToString() });
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = "������������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = serverInfo.Databases.Select(db => db.DatabaseStatus).Sum(db => db.IndexCount).ToString() });
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = "�ܶ���������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel2.Children.Add(new TextBlock { Text = serverInfo.Databases.Select(db => db.DatabaseStatus).Sum(db => db.ObjectCount).ToString("N0") });

                var serverBasicInfoPanel3 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };
                var fileSize = (serverInfo.Databases.Select(db => db.DatabaseStatus).Sum(db => db.FileSize) / 1024 / 1024).ToString("N0");
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = "���ļ���С��", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", fileSize) });

                var storageSize = (serverInfo.Databases.Select(db => db.DatabaseStatus).Sum(db => db.StorageSize) / 1024 / 1024).ToString("N0");
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = "�ܴ洢��С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", storageSize) });

                var dataSize = (serverInfo.Databases.Select(db => db.DatabaseStatus).Sum(db => db.DataSize) / 1024 / 1024).ToString("N0");
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = "�����ݴ�С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", dataSize) });

                var indexSize = (serverInfo.Databases.Select(db => db.DatabaseStatus).Sum(db => db.IndexSize) / 1024 / 1024).ToString("N0");
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = "��������С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", indexSize) });

                var objectSize = (serverInfo.Databases.Select(db => db.DatabaseStatus).Average(db => db.IndexSize)).ToString("N0");
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = "ƽ�������С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                serverBasicInfoPanel3.Children.Add(new TextBlock { Text = objectSize });

                serverBasicInfoPanel.Children.Add(serverBasicInfoPanel1);
                serverBasicInfoPanel.Children.Add(serverBasicInfoPanel2);
                serverBasicInfoPanel.Children.Add(serverBasicInfoPanel3);
                serverBasicInfoExpander.Content = serverBasicInfoPanel;
                ObjectDetailPanel.Children.Add(serverBasicInfoExpander);

                var metadataPanelExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "���ݿ���Ϣ",
                    IsExpanded = true,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                var metadataPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                var metadataGrid = new DataGrid()
                {
                    Width = ObjectDetailPanel.Width,
                    Tag = serverInfo.Descriptions,
                    MaxWidth = ObjectDetailPanel.Width - 50,
                    MaxHeight = ObjectDetailPanel.Height - 150,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    SelectionMode = DataGridSelectionMode.Single,
                };
                metadataGrid.ItemsSource = serverInfo.Descriptions.Select(desc =>
                new 
                {
                    ������ = desc.CategoryName,
                    ���� = desc.Name,
                    ǰ׺ = desc.DatabasePrefix,
                    ��ʾ�� = desc.DisplayName,
                    ������ = desc.ExpireDays,
                    ����ȫ�� = desc.TypeFullName,
                }).ToList();
                metadataPanel.Children.Add(metadataGrid);
                metadataPanelExpander.Content = metadataPanel;
                ObjectDetailPanel.Children.Add(metadataPanelExpander);
            }

            if ((ObjectTree.SelectedValue as TreeViewItem).Tag.GetType().Name == "MongodbDatabaseDescription")
            {
                var databaseDescription = (ObjectTree.SelectedValue as TreeViewItem).Tag as MongodbDatabaseDescription;

                var metadataBasicInfoExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "������Ϣ",
                    IsExpanded = true,
                };
                var metadataBasicInfoPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                var metadataBasicInfoPanel1 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = "��������", FontWeight = FontWeights.Bold });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseDescription.CategoryName });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = "���ݿ�ǰ׺��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseDescription.DatabasePrefix });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = "��ʾ����", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseDescription.DisplayName });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = "����ȫ����", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseDescription.TypeFullName });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = "����������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                metadataBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseDescription.ExpireDays.ToString() });

                metadataBasicInfoPanel.Children.Add(metadataBasicInfoPanel1);
                metadataBasicInfoExpander.Content = metadataBasicInfoPanel;
                ObjectDetailPanel.Children.Add(metadataBasicInfoExpander);

                var metadataColumnDescPanelExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "��Ԫ����",
                    IsExpanded = true,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                var metadataColumnDescPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                var metadataColumnDescGrid = new DataGrid()
                {
                    Width = ObjectDetailPanel.Width,
                    Tag = databaseDescription.MongodbColumnDescriptionList,
                    SelectionMode = DataGridSelectionMode.Single,
                    MaxWidth = ObjectDetailPanel.Width - 50,
                    MaxHeight = ObjectDetailPanel.Height - 150,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                metadataColumnDescGrid.ItemsSource = databaseDescription.MongodbColumnDescriptionList.Select(desc =>
                new
                {
                    �������� = desc.Name,
                    �洢���� = desc.ColumnName,
                    ���� = desc.Description,
                    ��ʾ�� = desc.DisplayName,
                    ������ = desc.IsArrayColumn,
                    �������� = desc.IsContextIdentityColumn,
                    ʵ���� = desc.IsEntityColumn,
                    ������ = desc.IsPrimaryKey,
                    ������ = desc.IsTableColumn,
                    ʱ���� = desc.IsTimeColumn,
                    ��ʾ���б���ͼ = desc.ShowInTableView,
                }).OrderByDescending(desc => desc.��ʾ���б���ͼ).ToList();
                metadataColumnDescPanel.Children.Add(metadataColumnDescGrid);
                metadataColumnDescPanelExpander.Content = metadataColumnDescPanel;
                ObjectDetailPanel.Children.Add(metadataColumnDescPanelExpander);
            }

            if ((ObjectTree.SelectedValue as TreeViewItem).Tag.GetType().Name == "DatabaseInfo")
            {
                var databaseInfo = (ObjectTree.SelectedValue as TreeViewItem).Tag as DatabaseInfo;

                var databaseBasicInfoExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "������Ϣ",
                    IsExpanded = true,
                };
                var databaseBasicInfoPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                var databaseBasicInfoPanel1 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };

                databaseBasicInfoPanel1.Children.Add(new TextBlock { Text = "���ݿ�ǰ׺��", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                databaseBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseInfo.DatabasePrefix });
                databaseBasicInfoPanel1.Children.Add(new TextBlock { Text = "���ݿ����ڣ�", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseInfo.DatabaseDate.ToString("yyyy/MM") });
                databaseBasicInfoPanel1.Children.Add(new TextBlock { Text = "���ݿ�����", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel1.Children.Add(new TextBlock { Text = databaseInfo.DatabaseName });

                var databaseBasicInfoPanel2 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };
                databaseBasicInfoPanel2.Children.Add(new TextBlock { Text = "�ܱ�������", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                databaseBasicInfoPanel2.Children.Add(new TextBlock { Text = databaseInfo.DatabaseStatus.CollectionCount.ToString() });
                databaseBasicInfoPanel2.Children.Add(new TextBlock { Text = "������������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel2.Children.Add(new TextBlock { Text = databaseInfo.DatabaseStatus.IndexCount.ToString() });
                databaseBasicInfoPanel2.Children.Add(new TextBlock { Text = "�ܶ���������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel2.Children.Add(new TextBlock { Text = databaseInfo.DatabaseStatus.ObjectCount.ToString("N0") });

                var databaseBasicInfoPanel3 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };
                var fileSize = (databaseInfo.DatabaseStatus.FileSize / 1024 / 1024).ToString("N0");
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = "���ļ���С��", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", fileSize) });

                var storageSize = (databaseInfo.DatabaseStatus.StorageSize / 1024 / 1024).ToString("N0");
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = "�ܴ洢��С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", storageSize) });

                var dataSize = (databaseInfo.DatabaseStatus.DataSize / 1024 / 1024).ToString("N0");
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = "�����ݴ�С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", dataSize) });

                var indexSize = (databaseInfo.DatabaseStatus.IndexSize / 1024 / 1024).ToString("N0");
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = "��������С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", indexSize) });

                var objectSize = (databaseInfo.DatabaseStatus.AverageObjectSize).ToString("N0");
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = "ƽ�������С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                databaseBasicInfoPanel3.Children.Add(new TextBlock { Text = objectSize });

                databaseBasicInfoPanel.Children.Add(databaseBasicInfoPanel1);
                databaseBasicInfoPanel.Children.Add(databaseBasicInfoPanel2);
                databaseBasicInfoPanel.Children.Add(databaseBasicInfoPanel3);
                databaseBasicInfoExpander.Content = databaseBasicInfoPanel;
                ObjectDetailPanel.Children.Add(databaseBasicInfoExpander);

                var collectionStatusExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "����Ϣ",
                    IsExpanded = true,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                var collectionStatusPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                var collectionStatusGrid = new DataGrid()
                {
                    Width = ObjectDetailPanel.Width,
                    Tag = databaseInfo.Collections,
                    SelectionMode = DataGridSelectionMode.Single,
                    MaxWidth = ObjectDetailPanel.Width - 50,
                    MaxHeight = ObjectDetailPanel.Height - 150,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                collectionStatusGrid.ItemsSource = databaseInfo.Collections.Select(collection =>
                new
                {
                    ���� = collection.CollectionName,
                    �����ռ� = collection.CollectionStatus.Namespace,
                    ƽ�������С = collection.CollectionStatus.AverageObjectSize,
                    ���ݴ�С = collection.CollectionStatus.DataSize,
                    ��չ����= collection.CollectionStatus.ExtentCount,
                    ��� = collection.CollectionStatus.Flags,
                    �������� = collection.CollectionStatus.IndexCount,
                    �������ʱ�� = collection.CollectionStatus.LastEnsureIndexTime,
                    �����չ��С = collection.CollectionStatus.LastExtentSize,
                    �������� = collection.CollectionStatus.ObjectCount,
                    ������� = collection.CollectionStatus.PaddingFactor,
                    �洢��С = collection.CollectionStatus.StorageSize,
                    ������С = collection.CollectionStatus.TotalIndexSize,
                }).ToList();
                collectionStatusPanel.Children.Add(collectionStatusGrid);
                collectionStatusExpander.Content = collectionStatusPanel;
                ObjectDetailPanel.Children.Add(collectionStatusExpander);
            }

            if ((ObjectTree.SelectedValue as TreeViewItem).Tag.GetType().Name == "CollectionInfo")
            {
                var collectionInfo = (ObjectTree.SelectedValue as TreeViewItem).Tag as CollectionInfo;
                
                var collectionBasicInfoExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "������Ϣ",
                    IsExpanded = true,
                };
                var collectionBasicInfoPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };

                var collectionBasicInfoPanel1 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };

                collectionBasicInfoPanel1.Children.Add(new TextBlock { Text = "������", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                collectionBasicInfoPanel1.Children.Add(new TextBlock { Text = collectionInfo.CollectionName });
                collectionBasicInfoPanel1.Children.Add(new TextBlock { Text = "��ȫ����", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                collectionBasicInfoPanel1.Children.Add(new TextBlock { Text = collectionInfo.CollectionStatus.Namespace });
                collectionBasicInfoPanel1.Children.Add(new TextBlock { Text = "�ϴ�����ʱ�䣺", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                collectionBasicInfoPanel1.Children.Add(new TextBlock { Text = collectionInfo.CollectionStatus.LastEnsureIndexTime.ToString() });

                var collectionBasicInfoPanel2 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };
                collectionBasicInfoPanel2.Children.Add(new TextBlock { Text = "Extent������", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                collectionBasicInfoPanel2.Children.Add(new TextBlock { Text = collectionInfo.CollectionStatus.ExtentCount.ToString() });
                collectionBasicInfoPanel2.Children.Add(new TextBlock { Text = "����������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                collectionBasicInfoPanel2.Children.Add(new TextBlock { Text = collectionInfo.CollectionStatus.IndexCount.ToString() });
                collectionBasicInfoPanel2.Children.Add(new TextBlock { Text = "����������", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                collectionBasicInfoPanel2.Children.Add(new TextBlock { Text = collectionInfo.CollectionStatus.ObjectCount.ToString("N0") });

                var collectionBasicInfoPanel3 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                };

                var storageSize = (collectionInfo.CollectionStatus.StorageSize / 1024 / 1024).ToString("N0");
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = "�ܴ洢��С��", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 0) });
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", storageSize) });

                var dataSize = (collectionInfo.CollectionStatus.DataSize / 1024 / 1024).ToString("N0");
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = "�����ݴ�С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", dataSize) });

                var indexSize = (collectionInfo.CollectionStatus.TotalIndexSize / 1024 / 1024).ToString("N0");
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = "��������С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = string.Format("{0} M", indexSize) });

                var objectSize = (collectionInfo.CollectionStatus.AverageObjectSize).ToString("N0");
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = "ƽ�������С��", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 0) });
                collectionBasicInfoPanel3.Children.Add(new TextBlock { Text = objectSize });

                collectionBasicInfoPanel.Children.Add(collectionBasicInfoPanel1);
                collectionBasicInfoPanel.Children.Add(collectionBasicInfoPanel2);
                collectionBasicInfoPanel.Children.Add(collectionBasicInfoPanel3);
                collectionBasicInfoExpander.Content = collectionBasicInfoPanel;
                ObjectDetailPanel.Children.Add(collectionBasicInfoExpander);

                var indexStatusExpander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    Header = "������Ϣ",
                    IsExpanded = true,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                var indexStatusPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                };
                var indexStatusGrid = new DataGrid()
                {
                    Width = ObjectDetailPanel.Width,
                    Tag = collectionInfo.CollectionStatus.IndexStatusList,
                    SelectionMode = DataGridSelectionMode.Single,
                    MaxWidth = ObjectDetailPanel.Width - 50,
                    MaxHeight = ObjectDetailPanel.Height - 150,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };
                indexStatusGrid.ItemsSource = collectionInfo.CollectionStatus.IndexStatusList.Select(index =>
                new
                {
                    ������ = index.Name,
                    �����ռ� = index.Namespace,
                    ������С = index.Size,
                    �Ƿ�Ψһ = index.Unique
                }).ToList();
                indexStatusPanel.Children.Add(indexStatusGrid);
                indexStatusExpander.Content = indexStatusPanel;
                ObjectDetailPanel.Children.Add(indexStatusExpander);
            }
        }

        private void service_GetServerInfoCompleted(object sender, GetServerInfoCompletedEventArgs e)
        {
            this.Busy.IsBusy = false;

            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }

            if (e.Result != null)
            {
                var rootNode = new TreeViewItem();
                rootNode.Header = new TextBlock { Text = "��������Ⱥ", FontWeight = FontWeights.Bold };
                foreach (var item in e.Result)
                {
                    var serverNode = new TreeViewItem();
                    serverNode.Header = item.Key.Name;
                    serverNode.Tag = item.Value;
                    var serverInfo = item.Value;
                    var descriptionsNode = new TreeViewItem();
                    descriptionsNode.Header = new TextBlock { Text = "���ݿ���", FontWeight = FontWeights.Bold };
                    foreach (var description in serverInfo.Descriptions.OrderBy(d => d.DatabasePrefix))
                    {
                        var dbaccess = Data.AdminConfigurationItem.MongodbAdminDatabaseConfigurationItems.Values.FirstOrDefault(_ => _.DatabasePrefix == "*"
                        || description.DatabasePrefix.Contains(_.DatabasePrefix));
                        if (dbaccess == null) continue;

                        var descriptionNode = new TreeViewItem();
                        descriptionNode.Header = description.DatabasePrefix;
                        descriptionNode.Tag = description;
                        var databasesNode = new TreeViewItem();
                        databasesNode.Header = new TextBlock { Text = "���ݿ�", FontWeight = FontWeights.Bold };
                        foreach (var database in serverInfo.Databases.Where(db => db.DatabasePrefix == description.DatabasePrefix).ToList())
                        {
                            var databaseNode = new TreeViewItem();
                            databaseNode.Header = database.DatabaseName;
                            databaseNode.Tag = database;
                            var collectionsNode = new TreeViewItem();
                            collectionsNode.Header = new TextBlock { Text = "��", FontWeight = FontWeights.Bold };
                            foreach (var collection in database.Collections)
                            {
                                var tableAccess = dbaccess.MongodbAdminTableConfigurationItems.Values.FirstOrDefault(_ => _.TableName == "*"
                                        || _.TableName.Contains(collection.CollectionName));
                                if (tableAccess == null) continue;

                                var collectionNode = new TreeViewItem();
                                collectionNode.Header = collection.CollectionName;
                                collectionNode.Tag = collection;
                                var indexesNode = new TreeViewItem();
                                indexesNode.Header = new TextBlock { Text = "����", FontWeight = FontWeights.Bold };
                                foreach (var index in collection.CollectionStatus.IndexStatusList)
                                {
                                    var indexNode = new TreeViewItem();
                                    indexNode.Header = index.Name;
                                    indexNode.Tag = index;
                                    indexesNode.Items.Add(indexNode);
                                }
                                collectionNode.Items.Add(indexesNode);
                                collectionsNode.Items.Add(collectionNode);
                                collectionNode.IsExpanded = false;
                                indexesNode.IsExpanded = false;
                            }
                            databaseNode.Items.Add(collectionsNode);
                            databasesNode.Items.Add(databaseNode);
                            collectionsNode.IsExpanded = false;
                            databaseNode.IsExpanded = true;
                        }
                        descriptionNode.Items.Add(databasesNode);
                        descriptionsNode.Items.Add(descriptionNode);
                        descriptionNode.IsExpanded = true;
                        databasesNode.IsExpanded = true;
                    }
                    serverNode.Items.Add(descriptionsNode);
                    rootNode.Items.Add(serverNode);
                    descriptionsNode.IsExpanded = true;
                    serverNode.IsExpanded = true;
                }
                rootNode.IsExpanded = true;
                ObjectTree.Items.Add(rootNode);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.Busy.IsBusy = true;
            ObjectTree.Items.Clear();
            service.GetServerInfoAsync();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size();
        }
    }
}
