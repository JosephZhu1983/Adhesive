using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Adhesive.Mongodb.Silverlight.Service;
using System.Windows.Browser;
using System.IO;
using System.Text;
using Polenter.Serialization;

namespace Adhesive.Mongodb.Silverlight
{


    public partial class Home : Page
    {
        private DataServiceClient service = new DataServiceClient("CustomBinding_DataService", new EndpointAddress(new Uri(Application.Current.Host.Source, "../DataService.svc")));
        private bool loaded = false; //�Ƿ��һ�μ������
        private FilterData filterData; //��������
        private double timeRange; //����Ϊ��λ��ʱ����
        private int timeOffset; //ʱ���ȵ�ƫ����
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings; //���ڱ�����Ի�����
        private static Thread autoRefreshThread;
        private bool autoRefreshTime = true;
        private static readonly string SelectionKey = "SELECTION";
        private Selection selectionFromUrl = null;

        public Home()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //var selection = GetLastSelection();
            //selection.CategoryNameApplied = selection.DatabaseNameApplied = selection.TableNamesApplied = selection.TimeRangeApplied = selection.ViewModelApplied = false;
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Size();
            //�����¼�ע��
            service.GetCategoryDataCompleted += new EventHandler<GetCategoryDataCompletedEventArgs>(service_GetCategoryDataCompleted);
            service.GetFilterDataCompleted += new EventHandler<GetFilterDataCompletedEventArgs>(service_GetFilterDataCompleted);

            //�߼�����Ĭ�ϲ�չ��
            AdvancedSearchPanel.IsExpanded = false;

            //ʱ����Ĭ��ѡ����
            var item = TimeRange.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (string)c.Tag == "Hour");
            if (item != null) TimeRange.SelectedItem = item;

            //�Զ�ѡ��
            DetailId.MouseEnter += (s, ee) => { DetailId.Focus(); DetailId.SelectAll(); };
            ContextId.MouseEnter += (s, ee) => { ContextId.Focus(); ContextId.SelectAll(); };
            UrlAddress.MouseEnter += (s, ee) => { UrlAddress.Focus(); UrlAddress.SelectAll(); };

            BeginTime.MouseEnter += (s, ee) => { BeginTime.Focus(); };
            EndTime.MouseEnter += (s, ee) => { EndTime.Focus(); };

            Load(null);
        }

        private void Size()
        {
            var root = Application.Current.RootVisual as FrameworkElement;
            ContentStackPanel.MaxWidth = root.ActualWidth - 120;
            PageScrollViewer.MaxHeight = root.ActualHeight - 120;

        }

        private Action CheckUrl(string url)
        {
            var qs = HtmlPage.Document.DocumentUri.Query;
            string d = "";
            if (qs.Contains("url"))
            {
                d = qs.Substring(5);
                UrlAddress.Text = d;
            }
            else if (url != null)
                d = url;
            if (d == "") return null;

            var serializer = new SharpSerializer(true);

            UrlData ud = null;

            try
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(d)))
                {
                    ud = serializer.Deserialize(ms) as UrlData;
                }
            }
            catch
            {
                MessageBox.Show("��url��ȡ����ʧ�ܣ���������Ϊ����̫�࣬�볢��ʹ��'ֱ��ճ����ݷ��ʱ�ʶ����ѯ����'���ܣ���url��ѯ�ַ����������ı����У�");
            }

            if (ud != null)
            {
                RemberLastSelection.IsChecked = autoRefreshTime = false;
                AutoRefresh.IsChecked = false;

                if (ud.Type == "List" || ud.Type == "Stat" || ud.Type == "Group")
                {
                    var sc = ud.Condition as SearchCondition;

                    if (sc != null)
                    {
                        selectionFromUrl = new Selection
                        {
                            BeginTime = sc.BeginTime,
                            EndTime = sc.EndTime,
                            CategoryName = sc.DatabasePrefix.Substring(0, sc.DatabasePrefix.IndexOf("__")),
                            DatabaseName = sc.DatabasePrefix.Substring(sc.DatabasePrefix.IndexOf("__") + 2),
                            TableNames = sc.TableNames,
                            ViewModel = ud.Type,
                        };

                        SetFilters(sc.Filters);
                        SetTime(sc.BeginTime, sc.EndTime);

                        if (ud.Type == "Stat")
                            return () =>
                        {
                            ShowStatView(sc, selectionFromUrl.CategoryName, selectionFromUrl.DatabaseName);
                          
                        };
                        if (ud.Type == "List")
                            return () =>
                            {
                                ShowListView(sc, selectionFromUrl.CategoryName, selectionFromUrl.DatabaseName);
                            };
                        if (ud.Type == "Group")
                            return () =>
                            {
                                ShowGroupView(sc, selectionFromUrl.CategoryName, selectionFromUrl.DatabaseName);
                            };
                    }
                }
                else if (ud.Type == "State")
                {
                    var sc = ud.Condition as StateCondition;
                    if (sc != null)
                    {
                        selectionFromUrl = new Selection
                        {
                            CategoryName = sc.DatabasePrefix.Substring(0, sc.DatabasePrefix.IndexOf("__")),
                            DatabaseName = sc.DatabasePrefix.Substring(sc.DatabasePrefix.IndexOf("__") + 2),
                            TableNames = new List<string> { sc.TableName },
                            ViewModel = ud.Type,
                        };

                        return () =>
                        {
                            ShowStateView(sc, selectionFromUrl.CategoryName, selectionFromUrl.DatabaseName);
                        };
                    }
                }
                else if (ud.Type == "Detail")
                {
                    var sc = ud.Condition as DetailCondition;
                    if (sc != null)
                    {
                        selectionFromUrl = new Selection
                        {
                            CategoryName = sc.DatabasePrefix.Substring(0, sc.DatabasePrefix.IndexOf("__")),
                            DatabaseName = sc.DatabasePrefix.Substring(sc.DatabasePrefix.IndexOf("__") + 2),
                            TableNames = new List<string> { sc.TableName },
                            ViewModel = ud.Type,
                        };

                        return () =>
                        {
                            ShowDetailView(sc, selectionFromUrl.CategoryName, selectionFromUrl.DatabaseName);
                        };
                    }
                }
            }

            return null;
        }

        private void AutoRefreshAction(object state)
        {
            while (true)
            {
                Thread.Sleep(1000);

                if (autoRefreshTime)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SetTime();
                    });
                }
            }
        }

        public void Load(string url)
        {
            if (Data.AdminConfigurationItem == null) return;

            var action = CheckUrl(url);

            if (Data.CategoryData == null)
            {
                this.Busy.IsBusy = true;
                service.GetCategoryDataAsync();
            }
            else
            {
                ShowCategory();
            }
            loaded = true;

            var selection = GetLastSelection();
            if (selection != null && !selection.TimeRangeApplied && selection.TimeRange != null)
            {
                var item = TimeRange.Items.OfType<ComboBoxItem>().FirstOrDefault(c => c.Tag.ToString() == selection.TimeRange);
                if (item != null) TimeRange.SelectedItem = item;
                selection.TimeRangeApplied = true;
            }

            if (selection != null && !selection.ViewModelApplied && selection.ViewModel != null)
            {
                var item = ViewModelPanel.Children.OfType<RadioButton>().FirstOrDefault(c => c.Tag.ToString() == selection.ViewModel);
                if (item != null) item.IsChecked = true;
                selection.ViewModelApplied = true;
            }

            if (action != null)
            {
                action();
            }
        }

        #region LoadDataCompleteEvents

        private void service_GetFilterDataCompleted(object sender, GetFilterDataCompletedEventArgs e)
        {
            this.Busy.IsBusy = false;

            //������еĿؼ�
            TextboxFilterListPanel.Children.Clear();
            SelectFilterListPanel.Children.Clear();
            MultipleSelectFilterListPanel.Children.Clear();
            CascadeFilterListPanel.Children.Clear();

            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }

            if (e.Result != null)
            {
                filterData = e.Result;
                //�������ؼ�
                var cascadeFilters = filterData.CascadeFilters;
                if (cascadeFilters.Count > 0)
                {
                    CascadeFilterList.Visibility = System.Windows.Visibility.Visible;
                    foreach (var cascadeFilter in cascadeFilters.OrderBy(c => c.CascadeFilterType))
                    {
                        var cascadeFilterControl = new ComboBox()
                        {
                            Margin = new Thickness(5, 0, 0, 0),
                            Tag = cascadeFilter.ColumnName,
                        };

                        cascadeFilterControl.SelectionChanged += new SelectionChangedEventHandler(cascadeFilterControl_SelectionChanged);

                        //ֻ�е�һ����ʱ��Ĭ�ϲ��������
                        if (cascadeFilter.CascadeFilterType == CascadeFilterType.LevelOne)
                        {
                            foreach (var item in cascadeFilter.Items.Where(item => !string.IsNullOrWhiteSpace(item.Key)).ToList())
                            {
                                cascadeFilterControl.Items.Add(new ComboBoxItem { Content = item.Key, Tag = item.Key });
                            }
                            if (cascadeFilterControl.Items.Count > 0)
                            {
                                //����Ĭ����Ϊ�հ��ַ������ַ��������൱���������ַ�������
                                cascadeFilterControl.Items.Insert(0, new ComboBoxItem { Content = new string(' ', cascadeFilterControl.Items.OfType<ComboBoxItem>().Max(c => c.Content.ToString().Length)), Tag = null });
                                cascadeFilterControl.SelectedIndex = 0;
                            }
                        }
                        //����
                        var panel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 5, 0, 0),
                        };
                        //������ʾ����
                        panel.Children.Add(new TextBlock { Text = cascadeFilter.DisplayName, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center });
                        //Ȼ���ǿؼ�����
                        panel.Children.Add(cascadeFilterControl);
                        //����������Ļ���������ı�
                        if (!string.IsNullOrWhiteSpace(cascadeFilter.Description))
                            panel.Children.Add(new TextBlock { Text = cascadeFilter.Description, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center, FontStyle = FontStyles.Italic });
                        CascadeFilterListPanel.Children.Add(panel);
                    }
                }
                if (cascadeFilters.Count == 0)
                {
                    CascadeFilterList.Visibility = System.Windows.Visibility.Collapsed;
                }

                //��ѡ����
                var selectFilters = filterData.ListFilters.Where(f => f.ListFilterType == ListFilterType.Select).ToList();
                if (selectFilters.Count > 0)
                {
                    SelectFilterList.Visibility = System.Windows.Visibility.Visible;
                    foreach (var selectFilter in selectFilters)
                    {
                        var selectFilterControl = new ComboBox()
                        {
                            Margin = new Thickness(5, 0, 0, 0),
                            Tag = selectFilter.ColumnName,
                        };

                        foreach (var item in selectFilter.Items.Where(item => !string.IsNullOrWhiteSpace(item.Name)).ToList())
                        {
                            selectFilterControl.Items.Add(new ComboBoxItem { Content = item.Name, Tag = item.Value });
                        }
                        if (selectFilterControl.Items.Count != 0)
                        {
                            selectFilterControl.Items.Insert(0, new ComboBoxItem { Content = new string(' ', selectFilterControl.Items.OfType<ComboBoxItem>().Max(c => c.Content.ToString().Length)), Tag = null });
                        }
                        else
                        {
                            selectFilterControl.Items.Insert(0, new ComboBoxItem { Content = ' ', Tag = null });
                        }
                        selectFilterControl.SelectedIndex = 0;
                        var panel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 5, 0, 0),
                        };
                        panel.Children.Add(new TextBlock { Text = selectFilter.DisplayName, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center });
                        panel.Children.Add(selectFilterControl);
                        if (!string.IsNullOrWhiteSpace(selectFilter.Description))
                            panel.Children.Add(new TextBlock { Text = selectFilter.Description, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center, FontStyle = FontStyles.Italic });
                        SelectFilterListPanel.Children.Add(panel);
                    }
                    if (selectFilters.Count == 0)
                    {
                        SelectFilterList.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }

                //��ѡ����
                var multipleselectFilters = filterData.ListFilters.Where(f => f.ListFilterType == ListFilterType.MultipleSelect).ToList();
                if (multipleselectFilters.Count > 0)
                {
                    MultipleSelectFilterList.Visibility = System.Windows.Visibility.Visible;
                    foreach (var multipleselectFilter in multipleselectFilters)
                    {
                        var panel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 5, 0, 0),
                        };

                        //ÿһ�����һ�������ڣ����г���300
                        var groupPanel = new WrapPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 5, 0, 0),
                            Tag = multipleselectFilter.ColumnName,
                            MaxWidth = 300,

                        };
                        foreach (var item in multipleselectFilter.Items.Where(item => !string.IsNullOrWhiteSpace(item.Name)).ToList())
                        {
                            var multipleselectFilterControl = new CheckBox()
                            {
                                Margin = new Thickness(5, 0, 0, 0),
                                Content = item.Name,
                                IsChecked = true,
                                Tag = item.Value,
                            };
                            groupPanel.Children.Add(multipleselectFilterControl);
                        }

                        var selectAllControl = new CheckBox()
                        {
                            Margin = new Thickness(5, 0, 0, 0),
                            Content = "",
                            IsChecked = true,
                            Tag = groupPanel,
                            IsThreeState = false,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        };
                        selectAllControl.Click += new RoutedEventHandler(selectAllControl_Click);
                        panel.Children.Add(selectAllControl);
                        panel.Children.Add(new TextBlock { Text = multipleselectFilter.DisplayName, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center });
                        panel.Children.Add(groupPanel);
                        if (!string.IsNullOrWhiteSpace(multipleselectFilter.Description))
                            panel.Children.Add(new TextBlock { Text = multipleselectFilter.Description, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center, FontStyle = FontStyles.Italic });

                        MultipleSelectFilterListPanel.Children.Add(panel);
                    }
                }
                if (multipleselectFilters.Count == 0)
                {
                    MultipleSelectFilterList.Visibility = System.Windows.Visibility.Collapsed;
                }

                //�ı�����
                var textboxFilters = filterData.TextboxFilters.ToList();
                if (textboxFilters.Count > 0)
                {
                    TextboxFilterList.Visibility = System.Windows.Visibility.Visible;
                    foreach (var textboxFilter in textboxFilters)
                    {
                        var textboxFilterControl = new TextBox()
                        {
                            Margin = new Thickness(5, 0, 0, 0),
                            Tag = textboxFilter.ColumnName,
                            Width = 150,
                        };

                        var panel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(0, 5, 0, 0),
                        };
                        panel.Children.Add(new TextBlock { Text = textboxFilter.DisplayName, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center });
                        panel.Children.Add(textboxFilterControl);
                        if (!string.IsNullOrWhiteSpace(textboxFilter.Description))
                            panel.Children.Add(new TextBlock { Text = textboxFilter.Description, Margin = new Thickness(5, 0, 0, 0), VerticalAlignment = System.Windows.VerticalAlignment.Center, FontStyle = FontStyles.Italic });
                        TextboxFilterListPanel.Children.Add(panel);
                    }
                }
                if (textboxFilters.Count == 0)
                {
                    TextboxFilterList.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void service_GetCategoryDataCompleted(object sender, GetCategoryDataCompletedEventArgs e)
        {

            this.Busy.IsBusy = false;
            if (e.Error != null)
            {
                e.Error.ShowError();
                return;
            }

            if (e.Result != null)
            {
                Data.CategoryData = e.Result.ToList();
                ShowCategory();
            }
        }


        #endregion

        #region ControlEvents

        private void RefreshCategory_Click(object sender, RoutedEventArgs e)
        {
            Data.CategoryData = null;
            //����ı��������ݿ��б�
            DatabaseList.Items.Clear();
            //Ҳ��Ҫ��ձ��б�
            TableListPanel.Children.Clear();
            //ȡ��ȫѡ
            CheckAllTables.IsChecked = false;
            Load(null);
        }

        private void selectAllControl_Click(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;

            var checkbox = sender as CheckBox;
            if (checkbox != null)
            {
                var panel = checkbox.Tag as WrapPanel;
                if (panel != null)
                {
                    var checkboxList = panel.Children.OfType<CheckBox>().ToList();
                    if (checkbox.IsChecked.HasValue)
                    {
                        checkboxList.ForEach(c => c.IsChecked = checkbox.IsChecked.Value);
                    }
                }
            }
        }

        private void AutoRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (AutoRefresh.IsChecked.HasValue)
                autoRefreshTime = AutoRefresh.IsChecked.Value;
        }


        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;

            if (autoRefreshThread == null)
            {
                autoRefreshThread = new Thread(this.AutoRefreshAction)
                {
                    IsBackground = true,
                };
                autoRefreshThread.Start();
            }

            //ÿһ����״̬�ı䶼�����߼�����������Ϊ��Ҫ���¼�������
            AdvancedSearchPanel.IsExpanded = false;
            //ֻ������Ϊ���˲Ż����¼���
            filterData = null;

            var selectedItem = CategoryList.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                var category = selectedItem.Tag as Category;
                if (category != null)
                {
                    //����ı��������ݿ��б�
                    DatabaseList.Items.Clear();
                    //Ҳ��Ҫ��ձ��б�
                    TableListPanel.Children.Clear();
                    CheckAllTables.IsChecked = false;
                    foreach (var subCategory in category.SubCategoryList)
                    {
                        var access = Data.AdminConfigurationItem.MongodbAdminDatabaseConfigurationItems.Values.FirstOrDefault(item => item.DatabasePrefix == "*"
                        || subCategory.DatabasePrefix.ToLower().StartsWith(item.DatabasePrefix.ToLower()));
                        if (access == null) continue;
                        DatabaseList.Items.Add(new ComboBoxItem { Content = subCategory.DisplayName, Tag = subCategory });
                    }
                }
            }

            var selection = GetLastSelection();
            if (selection != null && !selection.DatabaseNameApplied && selection.DatabaseName != null)
            {
                var item = DatabaseList.Items.OfType<ComboBoxItem>().FirstOrDefault(c => (c.Tag as SubCategory).Name == selection.DatabaseName);
                if (item != null) DatabaseList.SelectedItem = item;
                selection.DatabaseNameApplied = true;
            }
        }

        private void DatabaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;

            AdvancedSearchPanel.IsExpanded = false;
            filterData = null;
            var selectedItem = DatabaseList.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                var subCategory = selectedItem.Tag as SubCategory;
                if (subCategory != null)
                {
                    TableListPanel.Children.Clear();
                    CheckAllTables.IsChecked = true;
                    foreach (var tableName in subCategory.TableNames)
                    {
                        var dbaccess = Data.AdminConfigurationItem.MongodbAdminDatabaseConfigurationItems.Values.FirstOrDefault(item => item.DatabasePrefix == "*"
                        || subCategory.DatabasePrefix.ToLower().StartsWith(item.DatabasePrefix.ToLower()));
                        if (dbaccess == null) continue;

                        var tableAccess = dbaccess.MongodbAdminTableConfigurationItems.Values.FirstOrDefault(item => item.TableName == "*"
                            || item.TableName.ToLower().StartsWith(tableName.ToLower()));
                        if (tableAccess == null) continue;

                        var tableCheckbox = new CheckBox
                        {
                            Content = tableName,
                            Margin = new Thickness(5, 5, 0, 0),
                            IsChecked = true,
                        };
                        tableCheckbox.Click += new RoutedEventHandler(tableCheckbox_Click);
                        TableListPanel.Children.Add(tableCheckbox);

                    }
                }
            }

            var selection = GetLastSelection();
            if (selection != null && !selection.TableNamesApplied && selection.TableNames != null)
            {
                TableListPanel.Children.OfType<CheckBox>().ToList().ForEach(item => item.IsChecked = false);
                foreach (var table in selection.TableNames)
                {
                    var item = TableListPanel.Children.OfType<CheckBox>().FirstOrDefault(c => c.Content.ToString() == table);
                    if (item != null) item.IsChecked = true;
                }
                selection.TableNamesApplied = true;
            }

        }

        private void tableCheckbox_Click(object sender, RoutedEventArgs e)
        {

            if (!loaded) return;

            AdvancedSearchPanel.IsExpanded = false;
            filterData = null;

            var tableCheckboxList = TableListPanel.Children.OfType<CheckBox>().ToList();
            //������еĶ�ѡ����
            if (tableCheckboxList.All(c => c.IsChecked.HasValue && c.IsChecked.Value))
                CheckAllTables.IsChecked = true;
            //������ж�ûѡ��
            else if (tableCheckboxList.All(c => c.IsChecked.HasValue && !c.IsChecked.Value))
                CheckAllTables.IsChecked = false;
            //�������
            else
                CheckAllTables.IsChecked = null;

        }

        private void CheckAllTables_Click(object sender, RoutedEventArgs e)
        {

            if (!loaded) return;

            AdvancedSearchPanel.IsExpanded = false;
            filterData = null;

            var tableCheckboxList = TableListPanel.Children.OfType<CheckBox>().ToList();
            if (CheckAllTables.IsChecked.HasValue)
            {
                tableCheckboxList.ForEach(c => c.IsChecked = CheckAllTables.IsChecked.Value);
            }
            else //�������ѡ��Ļ��������ֱ��ȡ��ѡ��
            {
                tableCheckboxList.ForEach(c => c.IsChecked = false);
            }

        }

        private void AdvancedSearchPanel_Expanded(object sender, RoutedEventArgs e)
        {

            if (!loaded) return;

            //����������û�иĶ��Ļ��Ͳ���Ҫ���¼���������
            if (filterData != null) return;

            string categoryName = "";
            string databaseName = "";
            var databasePrefix = GetDatabasePrefix(ref categoryName, ref databaseName);
            if (databasePrefix != null)
            {
                var tableNames = GetTableNames(false);
                if (tableNames != null)
                {
                    var beginTime = GetBeginTime();
                    if (beginTime != null)
                    {
                        var endTime = GetEndTime();
                        if (endTime != null)
                        {
                            this.Busy.IsBusy = true;
                            service.GetFilterDataAsync(databasePrefix, tableNames, beginTime.Value, endTime.Value);
                            return;
                        }
                    }
                }
            }
            //�����֤ûͨ���Ļ���������
            AdvancedSearchPanel.IsExpanded = false;
        }

        private void cascadeFilterControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loaded) return;

            //ѡ��Ŀؼ�Ҳ���Ǹ��ؼ�
            var parentFilter = sender as ComboBox;
            if (parentFilter != null)
            {
                var parentSelectedItem = parentFilter.SelectedItem as ComboBoxItem;
                if (parentSelectedItem != null)
                {
                    var columnName = parentFilter.Tag as string;
                    if (columnName != null)
                    {
                        //���������ҵ����ؼ������ݶ���
                        var parent = filterData.CascadeFilters.FirstOrDefault(f => f.ColumnName == columnName);
                        if (parent != null)
                        {
                            //�ҵ���һ��������
                            var childfilterType = (int)parent.CascadeFilterType + 1;
                            //���������ҵ��ӿؼ������ݶ���
                            var child = filterData.CascadeFilters.FirstOrDefault(f => (int)f.CascadeFilterType == childfilterType);
                            if (child != null)
                            {
                                //�ٸ��������ҵ��ӿؼ�
                                var childFilter = CascadeFilterListPanel.Children.OfType<StackPanel>()
                                    .SelectMany(p => p.Children.OfType<ComboBox>())
                                    .FirstOrDefault(c => (string)c.Tag == child.ColumnName);
                                if (childFilter != null)
                                {
                                    var value = "";
                                    //����ǵ�һ���Ļ���ǰ׺ֻ��Ҫ�Ǹ��ؼ�������
                                    if (parent.CascadeFilterType == CascadeFilterType.LevelOne)
                                    {
                                        value = (string)parentSelectedItem.Content;
                                    }
                                    //����ǵڶ����Ļ�������Ҫ���ѵ�һ�������ݼ���ȥ
                                    else if (parent.CascadeFilterType == CascadeFilterType.LevelTwo)
                                    {
                                        //��һ���ļ���
                                        var parentparentfilterType = (int)parent.CascadeFilterType - 1;
                                        //�����ؼ�����
                                        var parentparent = filterData.CascadeFilters.FirstOrDefault(f => (int)f.CascadeFilterType == parentparentfilterType);
                                        if (parentparent != null)
                                        {
                                            //�����ؼ�
                                            var parentparentFilter = CascadeFilterListPanel.Children.OfType<StackPanel>()
                                                                        .SelectMany(p => p.Children.OfType<ComboBox>())
                                                                        .FirstOrDefault(c => (string)c.Tag == parentparent.ColumnName);
                                            if (parentparentFilter != null)
                                            {
                                                var parentparentSelectedItem = parentparentFilter.SelectedItem as ComboBoxItem;
                                                if (parentparentSelectedItem != null)
                                                {
                                                    //����ֵ����һ��
                                                    value = string.Format("{0}__{1}", (string)parentparentSelectedItem.Content, (string)parentSelectedItem.Content);
                                                }
                                            }
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(value) && childFilter.Items.Count > 0)
                                    {
                                        childFilter.SelectedIndex = 0;
                                    }
                                    else if (child.Items.ContainsKey(value))
                                    {
                                        childFilter.Items.Clear();
                                        foreach (var item in child.Items[value].Where(item => !string.IsNullOrWhiteSpace(item)).ToList())
                                        {
                                            childFilter.Items.Add(new ComboBoxItem { Content = item, Tag = item });
                                        }
                                        if (childFilter.Items.Count > 0)
                                        {
                                            childFilter.Items.Insert(0, new ComboBoxItem { Content = new string(' ', childFilter.Items.OfType<ComboBoxItem>().Max(c => c.Content.ToString().Length)), Tag = null });
                                            childFilter.SelectedIndex = 0;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }

        }

        private void TimeRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //�޸���ʱ����֮������ǰ�������
            timeOffset = 0;
            var selectedItem = TimeRange.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                var value = selectedItem.Tag;
                if (value != null)
                {
                    switch (value.ToString())
                    {
                        case "5Minutes":
                            {
                                timeRange = 5; //�Է���Ϊ��λ
                                break;
                            }
                        case "20Minutes":
                            {
                                timeRange = 20;
                                break;
                            }
                        case "Hour":
                            {
                                timeRange = TimeSpan.FromHours(1).TotalMinutes;
                                break;
                            }
                        case "Day":
                            {
                                timeRange = TimeSpan.FromDays(1).TotalMinutes;
                                break;
                            }
                        case "Week":
                            {
                                timeRange = TimeSpan.FromDays(7).TotalMinutes;
                                break;
                            }
                        case "Month":
                            {
                                timeRange = TimeSpan.FromDays(30).TotalMinutes;
                                break;
                            }
                        case "Quarter":
                            {
                                timeRange = TimeSpan.FromDays(90).TotalMinutes;
                                break;
                            }
                        case "Year":
                            {
                                timeRange = TimeSpan.FromDays(365).TotalMinutes;
                                break;
                            }
                    }
                    //�Զ�����ʱ��
                    SetTime();
                }
            }

        }

        private void PrevTimeOffset_Click(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            if (CheckTimeRange())
            {
                timeOffset++;
                //ÿһ�α䶯����������ʱ��
                SetTime();
            }
        }

        private void CurrentTimeOffset_Click(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            if (CheckTimeRange())
            {
                timeOffset = 0;
                //ÿһ�α䶯����������ʱ��
                SetTime();
            }
        }

        private void NextTimeOffset_Click(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;
            if (CheckTimeRange())
            {
                timeOffset--;
                //ÿһ�α䶯����������ʱ��
                SetTime();
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (!loaded) return;

            string categoryName = "";
            string databaseName = "";
            if (GetDatabasePrefix(ref categoryName, ref databaseName) != null)
            {
                var view = GetView();
                if (RemberLastSelection.IsChecked.Value)
                    UpdateLastSelection(categoryName, databaseName, GetTableNames(true), GetTimeRange(), view);
                else
                    UpdateLastSelection(null, null, null, null, null);

                switch (view)
                {
                    case "List":
                        {
                            ListView();
                            break;
                        }
                    case "Stat":
                        {
                            StatView();
                            break;
                        }
                    case "Group":
                        {
                            GroupView();
                            break;
                        }
                    case "State":
                        {
                            StateView();
                            break;
                        }
                }
            }
        }

        private void SearchDetailOnlyById_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DetailId.Text))
            {
                MessageBox.Show("����дID");
                DetailId.Focus();
                return;
            }
            string categoryName = "";
            string databaseName = "";
            var databasePrefix = GetDatabasePrefix(ref categoryName, ref databaseName);
            if (databasePrefix != null)
            {
                ViewDetailData c = new ViewDetailData();
                var root = Application.Current.RootVisual as FrameworkElement;
                c.DetailCondition = new DetailCondition
                {
                    DatabasePrefix = databasePrefix,
                    ID = DetailId.Text,
                };
                c.Width = root.ActualWidth * 0.98;
                c.Height = root.ActualHeight * 0.98;
                c.HorizontalAlignment = HorizontalAlignment.Center;
                c.VerticalAlignment = VerticalAlignment.Center;
                c.Show();
                service.LogAsync(new OperationLog
                {
                    AccountName = Data.AdminConfigurationItem.UserName,
                    AccountRealName = Data.AdminConfigurationItem.RealName,
                    CategoryName = categoryName,
                    DatabaseName = databaseName,
                    TableName = "",
                    Action = "����ID�鿴",
                    ActionMemo = string.Format("ID��{0}", DetailId.Text),
                });
            }
        }

        private void SearchListOnlyByContextId_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ContextId.Text))
            {
                MessageBox.Show("����дID");
                ContextId.Focus();
                return;
            }
            var root = Application.Current.RootVisual as FrameworkElement;
            ViewListData c = new ViewListData();
            //��������
            c.SearchCondition = new SearchCondition
            {
                ContextId = ContextId.Text,
            };
            //�����Ӵ��ڵĿ�Ⱥͳ���
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            //�Ӵ���ȫ������
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            service.LogAsync(new OperationLog
            {
                AccountName = Data.AdminConfigurationItem.UserName,
                AccountRealName = Data.AdminConfigurationItem.RealName,
                CategoryName = "",
                DatabaseName = "",
                TableName = "",
                Action = "����������ID�鿴",
                ActionMemo = string.Format("ID��{0}", ContextId.Text),
            });
        }

        #endregion

        #region Heplers


        private void UpdateLastSelection(string categoryName, string databaseName, List<string> tableNames, string timeRange, string viewModel)
        {
            Selection selection = null;
            if (setting.Contains(SelectionKey))
            {
                selection = setting[SelectionKey] as Selection;
            }
            else
            {
                selection = new Selection();
            }
            selection.CategoryName = categoryName;
            selection.DatabaseName = databaseName;
            selection.TableNames = tableNames;
            selection.TimeRange = timeRange;
            selection.ViewModel = viewModel;
            setting[SelectionKey] = selection;
        }

        private Selection GetLastSelection()
        {
            Selection selection = selectionFromUrl;
            if (selection != null) return selection;
            if (setting.Contains(SelectionKey))
            {
                selection = setting[SelectionKey] as Selection;
            }
            return selection;
        }


        private void SearchByUrlAddress_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlAddress.Text;
            if (url.StartsWith("http://") && url.Contains("url"))
            {
                int i = url.IndexOf("url=");
                url = url.Substring(i + 4);
            }
            Load(url);
        }

        private void ShowCategory()
        {
            CategoryList.Items.Clear();
            foreach (var category in Data.CategoryData)
            {
                var access = Data.AdminConfigurationItem.MongodbAdminDatabaseConfigurationItems.Values.FirstOrDefault(item => item.DatabasePrefix == "*"
                    || item.DatabasePrefix.Contains(category.Name));
                if (access == null) continue;
                CategoryList.Items.Add(new ComboBoxItem { Content = category.Name, Tag = category });
            }


            var selection = GetLastSelection();
            if (selection != null && !selection.CategoryNameApplied && selection.CategoryName != null)
            {
                var item = CategoryList.Items.OfType<ComboBoxItem>().FirstOrDefault(c => c.Content.ToString() == selection.CategoryName);
                if (item != null) CategoryList.SelectedItem = item;
                selection.CategoryNameApplied = true;
            }
        }

        /// <summary>
        /// �б�ʽ��������
        /// </summary>
        private void ListView()
        {
            string categoryName = "";
            string databaseName = "";
            var databasePrefix = GetDatabasePrefix(ref categoryName, ref databaseName);
            if (databasePrefix != null)
            {
                var tableNames = GetTableNames(false);
                if (tableNames != null)
                {
                    var beginTime = GetBeginTime();
                    if (beginTime != null)
                    {
                        var endTime = GetEndTime();
                        if (endTime != null)
                        {
                            var filters = GetFilters();
                            var sc = new SearchCondition
                            {
                                DatabasePrefix = databasePrefix,
                                TableNames = tableNames,
                                Filters = filters,
                                BeginTime = beginTime.Value,
                                EndTime = endTime.Value,
                            };
                            ShowListView(sc, categoryName, databaseName);
                        }
                    }
                }
            }
        }

        private void ShowListView(SearchCondition sc, string categoryName, string databaseName)
        {
            var root = Application.Current.RootVisual as FrameworkElement;
            ViewListData c = new ViewListData();
            //��������
            c.SearchCondition = sc;
            //�����Ӵ��ڵĿ�Ⱥͳ���
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            //�Ӵ���ȫ������
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            foreach (var tableName in sc.TableNames)
            {
                service.LogAsync(new OperationLog
                {
                    AccountName = Data.AdminConfigurationItem.UserName,
                    AccountRealName = Data.AdminConfigurationItem.RealName,
                    CategoryName = categoryName,
                    DatabaseName = databaseName,
                    TableName = tableName,
                    Action = "�鿴�б���ͼ",
                    ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
                    sc.BeginTime,
                    sc.EndTime,
                    sc.Filters.GetFilterText())
                });
            }
        }

        /// <summary>
        /// ����ͳ�Ʒ�ʽ��������
        /// </summary>
        private void GroupView()
        {
            string categoryName = "";
            string databaseName = "";
            var databasePrefix = GetDatabasePrefix(ref categoryName, ref databaseName);
            if (databasePrefix != null)
            {
                var tableNames = GetTableNames(false);
                if (tableNames != null)
                {
                    var beginTime = GetBeginTime();
                    if (beginTime != null)
                    {
                        var endTime = GetEndTime();
                        if (endTime != null)
                        {
                            var filters = GetFilters();
                            var sc = new SearchCondition
                            {
                                DatabasePrefix = databasePrefix,
                                TableNames = tableNames,
                                Filters = filters,
                                BeginTime = beginTime.Value,
                                EndTime = endTime.Value,
                            };
                            ShowGroupView(sc, categoryName, databaseName);
                        }
                    }
                }
            }
        }

        private void ShowGroupView(SearchCondition sc, string categoryName, string databaseName)
        {
            var root = Application.Current.RootVisual as FrameworkElement;
            ViewGroupData c = new ViewGroupData();
            //��������
            c.SearchCondition = sc;
            //�����Ӵ��ڵĿ�Ⱥͳ���
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            //�Ӵ���ȫ������
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            foreach (var tableName in sc.TableNames)
            {
                service.LogAsync(new OperationLog
                {
                    AccountName = Data.AdminConfigurationItem.UserName,
                    AccountRealName = Data.AdminConfigurationItem.RealName,
                    CategoryName = categoryName,
                    DatabaseName = databaseName,
                    TableName = tableName,
                    Action = "�鿴������ͼ",
                    ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
                      sc.BeginTime,
                      sc.EndTime,
                      sc.Filters.GetFilterText())
                });
            }
        }

        /// <summary>
        /// ������ͳ�Ʒ�ʽ��������
        /// </summary>
        private void StatView()
        {
            string categoryName = "";
            string databaseName = "";
            var databasePrefix = GetDatabasePrefix(ref categoryName, ref databaseName);
            if (databasePrefix != null)
            {
                var tableNames = GetTableNames(false);
                if (tableNames != null)
                {
                    if (tableNames.Count > 10)
                    {
                        MessageBox.Show("����������ͳ�Ʒ�ʽ�������ֻ��ѡ��10����");
                        return;
                    }
                    var beginTime = GetBeginTime();
                    if (beginTime != null)
                    {
                        var endTime = GetEndTime();
                        if (endTime != null)
                        {
                            var filters = GetFilters();
                            var sc = new SearchCondition
                            {
                                DatabasePrefix = databasePrefix,
                                TableNames = tableNames,
                                Filters = filters,
                                BeginTime = beginTime.Value,
                                EndTime = endTime.Value,
                            };
                            ShowStatView(sc, categoryName, databaseName);
                        }
                    }
                }
            }
        }
        private void ShowStatView(SearchCondition sc, string categoryName, string databaseName)
        {
            var root = Application.Current.RootVisual as FrameworkElement;
            ViewStatData c = new ViewStatData();
            c.SearchCondition = sc;
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            foreach (var tableName in sc.TableNames)
            {
                service.LogAsync(new OperationLog
                {
                    AccountName = Data.AdminConfigurationItem.UserName,
                    AccountRealName = Data.AdminConfigurationItem.RealName,
                    CategoryName = categoryName,
                    DatabaseName = databaseName,
                    TableName = tableName,
                    Action = "�鿴ͳ����ͼ",
                    ActionMemo = string.Format("��ʼʱ�䣺{0} ����ʱ�䣺{1} ����������{2}",
                    sc.BeginTime,
                    sc.EndTime,
                    sc.Filters.GetFilterText())
                });
            }
        }

        /// <summary>
        /// ״̬��ʽ��������
        /// </summary>
        private void StateView()
        {
            string categoryName = "";
            string databaseName = "";
            var databasePrefix = GetDatabasePrefix(ref categoryName, ref databaseName);
            if (databasePrefix != null)
            {
                var tableNames = GetTableNames(false);
                if (tableNames != null)
                {
                    if (tableNames.Count > 1)
                    {
                        MessageBox.Show("����״̬��ʽ������ֻѡ��һ����");
                        return;
                    }
                    var filters = GetFilters();
                    var sc = new StateCondition
                    {
                        DatabasePrefix = databasePrefix,
                        TableName = tableNames.First(),
                        Filters = filters,
                    };
                    ShowStateView(sc, categoryName, databaseName);
                }
            }
        }

        private void ShowStateView(StateCondition sc, string categoryName, string databaseName)
        {
            var root = Application.Current.RootVisual as FrameworkElement;
            ViewStateData c = new ViewStateData();
            c.StateCondition = sc;
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            service.LogAsync(new OperationLog
            {
                AccountName = Data.AdminConfigurationItem.UserName,
                AccountRealName = Data.AdminConfigurationItem.RealName,
                CategoryName = categoryName,
                DatabaseName = databaseName,
                TableName = sc.TableName,
                Action = "�鿴״̬��ͼ",
                ActionMemo = "",
            });
        }

        private void ShowDetailView(DetailCondition dc, string categoryName, string databaseName)
        {
            ViewDetailData c = new ViewDetailData();
            var root = Application.Current.RootVisual as FrameworkElement;
            c.DetailCondition = dc;
            c.Width = root.ActualWidth * 0.98;
            c.Height = root.ActualHeight * 0.98;
            c.HorizontalAlignment = HorizontalAlignment.Center;
            c.VerticalAlignment = VerticalAlignment.Center;
            c.Show();
            service.LogAsync(new OperationLog
            {
                AccountName = Data.AdminConfigurationItem.UserName,
                AccountRealName = Data.AdminConfigurationItem.RealName,
                CategoryName = categoryName,
                DatabaseName = databaseName,
                TableName = dc.TableName,
                Action = "�鿴��ϸ��ͼ",
                ActionMemo = string.Format("����������{0} ������{0}", dc.PkColumnName, dc.ID),
            });
        }

        private void SetFilters(Dictionary<string, object> filters)
        {
            if (filters != null && filters.Count > 0)
            {
                AdvancedSearchPanel.IsExpanded = true;
            }
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetFilters()
        {
            var dic = new Dictionary<string, object>();

            if (filterData == null) return dic;

            var cascadeFilters = CascadeFilterListPanel.Children.OfType<StackPanel>()
                .SelectMany(p => p.Children.OfType<ComboBox>()).ToList();

            var ccolumnName = "";
            var cvalue = "";
            foreach (var cascadeFilter in cascadeFilters)
            {
                var selectedItem = cascadeFilter.SelectedItem as ComboBoxItem;
                //���ѡ����
                if (selectedItem != null && !string.IsNullOrWhiteSpace(selectedItem.Content as string))
                {
                    var value = selectedItem.Tag as string;
                    ccolumnName = cascadeFilter.Tag as string;
                    if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(ccolumnName))
                    {
                        cvalue = cvalue + "__" + value;
                    }
                }
            };

            if (!string.IsNullOrEmpty(ccolumnName))
                dic.Add(ccolumnName.ToString(), cvalue.ToString().Trim('_'));

            //��ѡ����
            var selectFilters = SelectFilterListPanel.Children.OfType<StackPanel>()
                .SelectMany(p => p.Children.OfType<ComboBox>()).ToList();

            foreach (var selectFilter in selectFilters)
            {
                var selectedItem = selectFilter.SelectedItem as ComboBoxItem;
                if (selectedItem != null && !string.IsNullOrWhiteSpace(selectedItem.Content as string))
                {
                    //��ѡ�����ݿ�����ö�٣����ֵ�����������͵�
                    var value = selectedItem.Tag;
                    var columnName = selectFilter.Tag as string;
                    if (!string.IsNullOrWhiteSpace(columnName))
                    {
                        dic.Add(columnName.ToString(), value);
                    }
                }
            }

            //��ѡ���ˣ�����������
            var multipleselectFilterGroups = MultipleSelectFilterListPanel.Children.OfType<StackPanel>()
                .SelectMany(p => p.Children.OfType<WrapPanel>()).ToList();

            foreach (var multipleselectFilterGroup in multipleselectFilterGroups)
            {
                var multipleselectFilters = multipleselectFilterGroup.Children.OfType<CheckBox>().ToList();
                //ѡ���˶�����
                var checkConut = multipleselectFilters.Count(c => c.IsChecked.HasValue && c.IsChecked.Value);
                //���ûѡ��������ж�ѡ���˵Ļ����Ͳ���Ҫ��Ϊ����������
                if (checkConut > 0 && checkConut < multipleselectFilters.Count)
                {
                    var value = null as string;
                    var columnName = multipleselectFilterGroup.Tag as string;
                    //����ѡ������Զ��ŷָ�
                    foreach (var cb in multipleselectFilters.Where(c => c.IsChecked.HasValue && c.IsChecked.Value).ToList())
                    {
                        value += string.Format("{0},", cb.Tag);
                    }
                    if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(columnName))
                    {
                        dic.Add(columnName.ToString(), value.ToString().TrimEnd(','));
                    }
                }
            }

            //�ı�����
            var textboxFilters = TextboxFilterListPanel.Children.OfType<StackPanel>()
                    .SelectMany(p => p.Children.OfType<TextBox>()).ToList();
            foreach (var textboxFilter in textboxFilters)
            {
                var value = textboxFilter.Text;
                var columnName = textboxFilter.Tag as string;
                if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(columnName))
                {
                    dic.Add(columnName.ToString(), value.ToString());
                }
            }

            return dic;
        }

        /// <summary>
        /// ��ȡ��ͼ
        /// </summary>
        /// <returns></returns>
        private string GetView()
        {
            var item = ViewModelPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked.HasValue && r.IsChecked.Value);
            if (item != null)
                return item.Tag.ToString();
            else
            {
                MessageBox.Show("��ѡ��һ��������ͼ��");
                ViewModelPanel.Children.OfType<RadioButton>().FirstOrDefault().Focus();
                return "";
            }
        }


        /// <summary>
        /// ���ʱ�䷶Χ
        /// </summary>
        /// <returns></returns>
        private bool CheckTimeRange()
        {
            if ((TimeRange.SelectedItem as ComboBoxItem) != null)
            {
                return true;
            }
            else
            {
                MessageBox.Show("����ѡ��һ����׼ʱ���ȣ�");
                TimeRange.Focus();
                return false;
            }
        }

        private string GetTimeRange()
        {
            var item = TimeRange.SelectedItem as ComboBoxItem;
            if (item != null && item.Tag != null)
                return item.Tag.ToString();
            return "";
        }

        /// <summary>
        /// �Զ�����ʱ��
        /// </summary>
        private void SetTime()
        {
            var begin = DateTime.Now.AddMinutes(-timeRange * (timeOffset + 1));
            var end = DateTime.Now.AddMinutes(-timeRange * timeOffset);
            BeginDate.Text = begin.ToString("yyyy/MM/d");
            EndDate.Text = end.ToString("yyyy/MM/d");
            BeginTime.Value = begin;
            EndTime.Value = end;
        }

        private void SetTime(DateTime begin, DateTime end)
        {
            BeginDate.Text = begin.ToString("yyyy/MM/d");
            EndDate.Text = end.ToString("yyyy/MM/d");
            BeginTime.Value = begin;
            EndTime.Value = end;
        }

        /// <summary>
        /// ��ȡ��������ݿ���
        /// </summary>
        /// <returns></returns>
        private string GetDatabasePrefix(ref string categoryName, ref string databaseName)
        {
            ComboBoxItem selectedCategory = CategoryList.SelectedItem as ComboBoxItem;
            Category category = null;
            if (selectedCategory != null)
                category = selectedCategory.Tag as Category;
            if (selectedCategory == null || category == null)
            {
                MessageBox.Show("��ѡ�����!");
                CategoryList.Focus();
                return null;
            }

            ComboBoxItem selectedDatabase = DatabaseList.SelectedItem as ComboBoxItem;
            SubCategory subCategory = null;
            if (selectedDatabase != null)
                subCategory = selectedDatabase.Tag as SubCategory;
            if (selectedDatabase == null || subCategory == null)
            {
                MessageBox.Show("��ѡ�����ݿ�!");
                DatabaseList.Focus();
                return null;
            }
            categoryName = category.Name;
            databaseName = subCategory.Name;
            return string.Format("{0}__{1}", category.Name, subCategory.Name);
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        private List<string> GetTableNames(bool silence)
        {
            var tableCheckboxList = TableListPanel.Children.OfType<CheckBox>().ToList();
            if (tableCheckboxList.All(c => c.IsChecked.HasValue && !c.IsChecked.Value))
            {
                if (!silence)
                {
                    MessageBox.Show("������ѡ��һ����");
                    CheckAllTables.Focus();
                }
                return null;
            }
            else
            {
                var tableNames = new List<string>();
                tableCheckboxList.ForEach(tableCheckbox =>
                {
                    if (tableCheckbox.IsChecked.HasValue && tableCheckbox.IsChecked.Value)
                        tableNames.Add((string)tableCheckbox.Content);
                });
                return tableNames;
            }
        }

        /// <summary>
        /// ��ȡ��ʼʱ��
        /// </summary>
        /// <returns></returns>
        private DateTime? GetBeginTime()
        {
            if (string.IsNullOrWhiteSpace(BeginDate.Text))
            {
                MessageBox.Show("��ѡ�������һ����ʼ����!");
                return null;
            }
            DateTime time;
            if (!DateTime.TryParse(BeginDate.Text, out time))
            {
                MessageBox.Show("��ʼ���ڵĸ�ʽ����ȷ!");
                BeginDate.Focus();
                return null;
            }
            if (!BeginTime.Value.HasValue)
            {
                MessageBox.Show("��ѡ�������һ����ʼʱ��!");
                BeginTime.Focus();
                return null;
            }
            time = time.Add(BeginTime.Value.Value.TimeOfDay);
            return time;
        }

        /// <summary>
        /// ��ȡ����ʱ��
        /// </summary>
        /// <returns></returns>
        private DateTime? GetEndTime()
        {
            var view = GetView();
            if (string.IsNullOrWhiteSpace(EndDate.Text))
            {
                MessageBox.Show("��ѡ�������һ����ʼ����!");
                return null;
            }
            DateTime time;
            if (!DateTime.TryParse(EndDate.Text, out time))
            {
                MessageBox.Show("�������ڵĸ�ʽ����ȷ!");
                EndDate.Focus();
                return null;
            }
            if (!EndTime.Value.HasValue)
            {
                MessageBox.Show("��ѡ�������һ������ʱ��!");
                EndTime.Focus();
                return null;
            }
            time = time.Add(EndTime.Value.Value.TimeOfDay);
            if (time > DateTime.Now)
            {
                time = DateTime.Now;
                EndDate.Text = DateTime.Now.ToString("yyyy/MM/d");
                EndTime.Value = DateTime.Now;
            }
            if (view == "List")
            {
                time = time.AddMinutes(int.Parse(AddTime.Text));
            }
            return time;
        }

        #endregion

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size();
        }

    }

    [DataContract]
    public class Selection
    {
        [DataMember]
        public string CategoryName { get; set; }

        public bool CategoryNameApplied { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }

        public bool DatabaseNameApplied { get; set; }

        [DataMember]
        public List<string> TableNames { get; set; }

        public bool TableNamesApplied { get; set; }

        [DataMember]
        public string TimeRange { get; set; }

        public bool TimeRangeApplied { get; set; }

        [DataMember]
        public string ViewModel { get; set; }

        public bool ViewModelApplied { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
