using System;
using Adhesive.AppInfoCenter;
using Adhesive.Common;

namespace Adhesive.Mongodb.Test
{
    class Program
    {
        private static Random rnd = new Random();
        private static string[] stringfilterpool = new string[] { "Trading", "Consignment", "Escort" };

        static void Main(string[] args)
        {

         
            AdhesiveFramework.Start();

            while (Console.ReadLine() != "exit")
            {
                for (int i = 0; i < 1000; i++)
                {
                    AppInfoCenterService.LoggingService.Error("朱晔测试" + i.ToString());
                }              
            }

            //var extraInfo = new ExtraInfo
            //{
            //    DisplayItems = new Dictionary<string, string>()
            //    {
            //        { "DisplayItem1", "DisplayItem1" },
            //        { "DisplayItem2", "DisplayItem2" }
            //    },
            //    DropDownListFilterItem1 = stringfilterpool[rnd.Next(3)],
            //    DropDownListFilterItem2 = stringfilterpool[rnd.Next(3)],
            //    CheckBoxListFilterItem1 = stringfilterpool[rnd.Next(3)],
            //    CheckBoxListFilterItem2 = stringfilterpool[rnd.Next(3)],
            //};

            //while (true)
            //{
            //    var test = new Test
            //    {
            //        NormalColumn1 = "朱晔",
            //        NormalColumn2 = "4324324234",
            //        ListColumn1 = new List<string> { "苹果", "桔子" },
            //        DictionaryColumn1 = new Dictionary<string, string> { { "a", "A" }, { "b", "B" } },
            //        ListColumn2 = Enumerable.Range(1, 2).ToList(),
            //        DictionaryColumn2 = new Dictionary<int, string> { { 1, "x" }, { 2, "y" } },
            //        PkColumn1 = Guid.NewGuid().ToString(),
            //        ShownInStateColumn1 = rnd.Next(10000),
            //        ShownInStateColumn2 = rnd.Next(5000),
            //        StatTimeColumn1 = DateTime.Now,
            //        EnumColumn1 = (Enum1)rnd.Next(1, 4),
            //        EnumColumn2 = (Enum1)rnd.Next(1, 4),
            //        IgnoreColumn1 = "asdasdas",
            //        IgnoreColumn2 = "sadasdsad",
            //        TextboxFilterColumn1 = stringfilterpool[rnd.Next(0, 3)],
            //        TextboxFilterColumn2 = stringfilterpool[rnd.Next(0, 3)],
            //        DropDownListFilterColumn1 = stringfilterpool[rnd.Next(0, 3)],
            //        CheckBoxFilterColumn1 = stringfilterpool[rnd.Next(0, 3)],
            //        CascadeFilterColumnLevelOne1 = stringfilterpool[rnd.Next(0, 3)] + "1",
            //        CascadeFilterColumnLevelTwo1 = stringfilterpool[rnd.Next(0, 3)] + "2",
            //        CascadeFilterColumnLevelThree1 = stringfilterpool[rnd.Next(0, 3)] + "3",
            //        CheckBoxFilterColumn2 = (Enum2)rnd.Next(1, 4),
            //        DropDownListFilterColumn2 = (Enum2)rnd.Next(1, 4),
            //        TableNameColumn1 = "Escort",
            //        ExtColumn1 = new Ext
            //        {
            //            NormalColumn3 = DateTime.Parse("2011/8/1").AddMinutes(rnd.Next(100 * 60 * 24)),
            //            DictionaryColumn3 = new Dictionary<string, string> { { "a", "aaa" }, { "b", "bbb" } },
            //            ListColumn3 = Enumerable.Range(1, 2).ToList(),
            //            ShownInStateColumn3 = rnd.Next(2000),
            //            EnumColumn3 = (Enum3)rnd.Next(1, 4),
            //            IgnoreColumn3 = "asdasdas",
            //            TextboxFilterColumn3 = stringfilterpool[rnd.Next(0, 3)],
            //            DropDownListFilterColumn3 = stringfilterpool[rnd.Next(0, 3)],
            //            CheckBoxFilterColumn3 = stringfilterpool[rnd.Next(0, 3)],

            //            ExtListColumn3 = new List<ExtItem>
            //        {
            //            new ExtItem
            //            {
            //                NormalColumn4 = 100,
            //                DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                ListColumn4 = Enumerable.Range(1, 2).ToList(),

            //                EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                IgnoreColumn4 = "asdasdas",
            //            },
            //            new ExtItem
            //            {
            //                NormalColumn4 = 200,
            //                DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                ListColumn4 = Enumerable.Range(1, 2).ToList(),
                    
            //                EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                IgnoreColumn4 = "asdasdas",
            //            },
            //        },
            //            ExtDictionaryColumn3 = new Dictionary<string, ExtItem>
            //        {
            //            { "Key1", new ExtItem
            //                {
            //                    NormalColumn4 = 100,
            //                    DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                    ListColumn4 = Enumerable.Range(1, 2).ToList(),
                          
            //                    EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                    IgnoreColumn4 = "asdasdas",
            //                }
            //            },
            //            { "Key2", new ExtItem
            //                {
            //                    NormalColumn4 = 100,
            //                    DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                    ListColumn4 = Enumerable.Range(1, 2).ToList(),
                              
            //                    EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                    IgnoreColumn4 = "asdasdas",
            //                }
            //            },
            //        }
            //        },
            //        ExtColumn2 = new Ext2
            //        {
            //            NormalColumn3 = DateTime.Now,
            //            DictionaryColumn3 = new Dictionary<string, int> { { "a", 100 }, { "b", 200 } },
            //            ListColumn3 = Enumerable.Range(1, 2).ToList(),
            //            ShownInStateColumn3 = (short)rnd.Next(20),
            //            EnumColumn3 = (Enum3)rnd.Next(1, 4),
            //            IgnoreColumn3 = "asdasdas",
            //            TextboxFilterColumn3 = stringfilterpool[rnd.Next(0, 3)],
            //            DropDownListFilterColumn3 = stringfilterpool[rnd.Next(0, 3)],
            //            CheckBoxFilterColumn3 = stringfilterpool[rnd.Next(0, 3)],
            //            ExtListColumn3 = new List<ExtItem>
            //        {
            //            new ExtItem
            //            {
            //                NormalColumn4 = 100,
            //                DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                ListColumn4 = Enumerable.Range(1, 2).ToList(),
                          
            //                EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                IgnoreColumn4 = "asdasdas",
            //            },
            //            new ExtItem
            //            {
            //                NormalColumn4 = 200,
            //                DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                ListColumn4 = Enumerable.Range(1, 2).ToList(),
                            
            //                EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                IgnoreColumn4 = "asdasdas",
            //            },
            //        },
            //            ExtDictionaryColumn3 = new Dictionary<string, ExtItem>
            //        {
            //            { "Key1", new ExtItem
            //                {
            //                    NormalColumn4 = 100,
            //                    DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                    ListColumn4 = Enumerable.Range(1, 2).ToList(),
                              
            //                    EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                    IgnoreColumn4 = "asdasdas",
            //                }
            //            },
            //            { "Key2", new ExtItem
            //                {
            //                    NormalColumn4 = 100,
            //                    DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                    ListColumn4 = Enumerable.Range(1, 2).ToList(),
                                
            //                    EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                    IgnoreColumn4 = "asdasdas",
            //                }
            //            },
            //        }
            //        },
            //        ExtListColumn1 = new List<ExtItem>
            //    {
            //        new ExtItem
            //        {
            //            NormalColumn4 = 100,
            //            DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //            ListColumn4 = Enumerable.Range(1, 2).ToList(),
                       
            //            EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //            IgnoreColumn4 = "asdasdas",
            //        },
            //        new ExtItem
            //        {
            //            NormalColumn4 = 200,
            //            DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //            ListColumn4 = Enumerable.Range(1, 2).ToList(),
                       
            //            EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //            IgnoreColumn4 = "asdasdas",
            //        },
            //    },
            //        ExtDictionaryColumn1 = new Dictionary<string, ExtItem>
            //    {
            //        { "Key1", new ExtItem
            //            {
            //                NormalColumn4 = 100,
            //                DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                ListColumn4 = Enumerable.Range(1, 2).ToList(),
                            
            //                EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                IgnoreColumn4 = "asdasdas",
            //            }
            //        },
            //        { "Key2", new ExtItem
            //            {
            //                NormalColumn4 = 100,
            //                DictionaryColumn4 = new Dictionary<int, string>  { { 1, "x" }, { 2, "y" } },
            //                ListColumn4 = Enumerable.Range(1, 2).ToList(),
                            
            //                EnumColumn4 = (Enum4)rnd.Next(1, 4),
            //                IgnoreColumn4 = "asdasdas",
            //            }
            //        },
            //    }
            //    };

            //    MongodbService.MongodbInsertService.Insert(test);

            //    Console.ReadLine();
            //}

            //while (Console.ReadLine() != "exit")
            //{
            //    var service = new Mongodb.Server.Imp.MongodbServer();

            //    var a = service.GetDetailDataOnlyById("Aic__WebsiteException", "ef0ec7dc-f288-46ae-8f84-77efeb2b5a9c");
            //    var serverInfo = service.GetServerInfo();
            //    var category = service.GetCategoryData();
            //    var condition = new Dictionary<string, object> {
            //    { "DropDownListFilterColumn22", 1 }, 
            //    {"CheckBoxFilterColumn11",  "Trading,Escort" } };

            //    var filters = service.GetFilterData("Test__Test2", new List<string> { "Trading" }, DateTime.Parse("2011/8/1"), DateTime.Parse("2011/10/1"));
            //    var tables = service.GetTableData("Test__Test2", new List<string> { "Trading", "Escort" }, DateTime.Parse("2011/6/1"), DateTime.Parse("2011/10/1"), 0, 200, null);

            //    var stat = service.GetStatisticsData("Test__Test2", new List<string> { "Escort" }, DateTime.Parse("2011/8/4"), DateTime.Parse("2011/8/5"), TimeSpan.FromHours(1), null);
            //    foreach (var item in stat.First().StatisticsItems)
            //        Console.WriteLine(item.BeginTime + ":" + item.Value);
            //    var group = service.GetGroupData("Test__Test2", new List<string> { "Escort" }, DateTime.Parse("2011/8/1"), DateTime.Parse("2011/10/1"), condition);
            //    var detail = service.GetDetailData("Test__Test2", "Test__Test2__201109", "Trading", "PkColumn11", tables.First().Data.First()["#TestBase.PkColumn"].ToString(), true);
            //    var state = service.GetStateData("StateInfo__MongodbServerStateInfo", "Adhesive.Mongodb.Server", DateTime.Parse("2011/8/1"), DateTime.Parse("2011/10/1"), null);
            //    var contextTables = service.GetTableDataByContextId(DateTime.Parse("2011/8/1"), DateTime.Parse("2011/10/1"), "de4c8441-4d98-4b60-802c-cd14be93a6e0");

            //}

        }
    }
}
