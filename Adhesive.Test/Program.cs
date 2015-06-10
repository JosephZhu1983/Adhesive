using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Adhesive.Common;
using Adhesive.Config;


namespace Adhesive.Test
{
    class Program
    {
        private static  IConfigService _configService = null;
        static void Main(string[] args)
        {
            //启动框架
            AdhesiveFramework.Start();
            _configService = LocalServiceLocator.GetService<IConfigService>();

            //获取 全局配置 > TradingConfig > SearchConfig 下名为EnableNewEditionSearch配置项，默认值为false
            bool enableNewEditionSearch = _configService.GetConfigItemValue("TradingConfig", 
                                                                        "SearchConfig", 
                                                                        "EnableNewEditionSearch",
                                                                        false, 
                                                                        EnableNewEditionSearch_ConfigItemValueUpdateCallback);
            Console.WriteLine(string.Format("全局配置 > TradingConfig > SearchConfig > EnableNewEditionSearch：{0}",
                                              enableNewEditionSearch));

            //获取 全局配置 > TradingConfig > SearchConfig 下名为Games列表，默认值为  List<string> {"魔兽世界","冒险岛"}
            List<string> games = _configService.GetConfigItemValue("TradingConfig", 
                                                                    "SearchConfig", 
                                                                    "Games", 
                                                                    new List<string> 
                                                                    { 
                                                                        "魔兽世界",
                                                                        "冒险岛" 
                                                                    }, 
                                                                    Games_ConfigItemValueUpdateCallback);
            Console.WriteLine("全局配置 > TradingConfig > SearchConfig > Games：");
            foreach (var game in games)
            {

                Console.WriteLine(game);
            }
            //获取 全局配置 > TradingConfig > SearchConfig 下名为Switches的字典，默认值为  new Dictionary<string,bool> {{"Equipment",true},{"Card",false} }
            Dictionary<string, bool> switches = _configService.GetConfigItemValue("TradingConfig", 
                                                                                   "SearchConfig", 
                                                                                   "Switches", 
                                                                                   new Dictionary<string, bool>
                                                                                       {
                                                                                           { "Equipment", true }, { "Card", false }
                                                                                       }, 
                                                                                   Switches_ConfigItemValueUpdateCallback);
            Console.WriteLine("全局配置 > TradingConfig > SearchConfig > Switches：");
            foreach (var de in switches)
            {
                Console.WriteLine("物品类型：{0},是否开启该业务：{1}",de.Key,de.Value);
            }

            //获取 AdhesiveTest 下名为 BizOfferPromotionConfig 的自定义实体，被动获取
            new Thread(delegate()
                       {
                           while (true)
                           {
                               BizOfferPromotionConfig defaultBizOfferPromotionConfig = new BizOfferPromotionConfig();
                               BizOfferPromotionConfig bizOfferPromotionConfig = _configService.GetConfigItemValue(false,
                                                                             "BizOfferPromotionConfig",
                                                                             defaultBizOfferPromotionConfig);
                               Console.WriteLine("AdhesiveTest > BizOfferPromotionConfig：");
                               Console.WriteLine("OnOff:" + bizOfferPromotionConfig.OnOff);
                               Console.WriteLine("PromotionUrl :" + bizOfferPromotionConfig.PromotionUrl);
                               Console.WriteLine("PromotionUsers :");
                               foreach (var promotionUser in bizOfferPromotionConfig.PromotionUsers)
                               {
                                   Console.ForegroundColor = ConsoleColor.Yellow;
                                   Console.WriteLine(promotionUser);
                                   Console.ResetColor();
                               }
                               Console.WriteLine("PromotionPrice：");
                               Console.WriteLine("PromotionPrice.Game：" + bizOfferPromotionConfig.PromotionPrice.Game);
                               Console.WriteLine("PromotionPrice.BizOfferType： " + bizOfferPromotionConfig.PromotionPrice.BizOfferType);
                               Console.WriteLine("PromotionTimes[0]：");
                               Console.ForegroundColor = ConsoleColor.Yellow;
                               Console.WriteLine(bizOfferPromotionConfig.PromotionTimes[0]);
                               Console.ResetColor();
                               Console.WriteLine("UserGrades：");
                               foreach (var userGrade in bizOfferPromotionConfig.UserGrades)
                               {
                                   Console.ForegroundColor = ConsoleColor.Yellow;
                                   Console.WriteLine(userGrade);
                                   Console.ResetColor();
                               }
                               Thread.Sleep(3000);
                           }
                       }).Start();

            new Thread(delegate()
            {
                while (true)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    AppInfoCenterConfig defConfig = new AppInfoCenterConfig();
                    Console.WriteLine("默认值：" + new Common().MemoryQueueMaxCount);
                    AppInfoCenterConfig aicConfig = _configService.GetConfigItemValue<AppInfoCenterConfig>(false, typeof(AppInfoCenterConfig).FullName, defConfig);
                    sw.Stop();
                    Console.WriteLine(string.Format("获取配置:{0}ms", sw.ElapsedMilliseconds));
                    Console.WriteLine("TimeSpan:" + aicConfig.CommonField.TimeSpanField);
                    Console.WriteLine("MemoryQueueMaxCount:" + aicConfig.CommonField.MemoryQueueMaxCount);
                    Console.WriteLine("commonDico:k1:" + aicConfig.commonDico["k1"]);
                    Console.WriteLine("DateTime:" + aicConfig.CommonField.DateTime);
                    Console.WriteLine("Styles:" + aicConfig.CommonField.styles);
                    Console.WriteLine("ts:" + aicConfig.CommonField.ts);
                    Console.WriteLine("StringField：" + (aicConfig.CommonField.StringField == string.Empty ? "空" : aicConfig.CommonField.StringField));
                    Console.WriteLine(aicConfig.commonList[0].LocalMemoryQueuePath);
                    Console.WriteLine("commonList.Count：" + aicConfig.commonList.Count);
                    Thread.Sleep(5000);
                }
            }).Start();

            _configService.GetConfigItemValue(false, "BizOfferPromotionConfig", new BizOfferPromotionConfig(), BizOfferPromotionConfig_ConfigItemValueUpdateCallback);

            new Thread(delegate()
            {
                while (true)
                {
                    string cateName = _configService.GetConfigItemValue(false, "cateName", "cateName");
                    Console.WriteLine("cateName：" + cateName);
                    Thread.Sleep(3000);
                }
            }).Start();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            //结束框架
            AdhesiveFramework.End();

        }
        public static void EnableNewEditionSearch_ConfigItemValueUpdateCallback(ConfigItemValueUpdateArguments args)
        {
            bool enableNewEditionSearch = _configService.GetConfigItemValue("TradingConfig", "SearchConfig", "EnableNewEditionSearch", false);
            Console.WriteLine(string.Format("配置项 EnableNewEditionSearch 值更改，最新值为：{0}", enableNewEditionSearch));
        }
        public static void Games_ConfigItemValueUpdateCallback(ConfigItemValueUpdateArguments args)
        {

            Console.WriteLine("配置项 Games 值更改，最新值为：");
            List<string> games = _configService.GetConfigItemValue("TradingConfig", "SearchConfig", "Games", new List<string> { "魔兽世界", "冒险岛" });
            Console.WriteLine("全局配置 > TradingConfig > SearchConfig > Games：");
            foreach (var game in games)
            {

                Console.WriteLine(game);
            }
        }
        public static void Switches_ConfigItemValueUpdateCallback(ConfigItemValueUpdateArguments args)
        {
            Console.WriteLine("配置项 Switches 值更改，最新值为：");
            Dictionary<string, bool> switches = _configService.GetConfigItemValue("TradingConfig", "SearchConfig", "Switches", new Dictionary<string, bool> { { "Equipment", true }, { "Card", false } }, Switches_ConfigItemValueUpdateCallback);
            Console.WriteLine("全局配置 > TradingConfig > SearchConfig > Switches：");
            foreach (var de in switches)
            {
                Console.WriteLine("物品类型：{0},是否开启该业务：{1}", de.Key, de.Value);
            }
        }
        public static void BizOfferPromotionConfig_ConfigItemValueUpdateCallback(ConfigItemValueUpdateArguments args)
        {
            LocalLoggingService.Debug("BizOfferPromotionConfig changed");
        }
        public static void ConfigItemValueUpdateCallback(ConfigItemValueUpdateArguments args)
        {
            
            //Console.WriteLine(string.Format("配置项 EnableNewEditionSearch 值更改，最新值为：{0}"));
        }
    }
}
