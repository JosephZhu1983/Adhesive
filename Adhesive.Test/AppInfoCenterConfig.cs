using System;
using System.Collections.Generic;
using Adhesive.Common;
using Adhesive.Config;

namespace Adhesive.Test
{
    [Serializable]
    [ConfigEntity(FriendlyName = "应用程序信息中心配置")]
    public class AppInfoCenterConfig
    {
        [ConfigItem(FriendlyName = "Common字段")]
        public Common CommonField = new Common();
        [ConfigItem(FriendlyName = "Common属性")]
        public Common CommonProperty { get; set; }

        [ConfigItem(FriendlyName = "commons列表")]
        public List<Common> commonList = new List<Common>();
        [ConfigItem(FriendlyName = "commons字典")]
        public SerializableDictionary<string, Common> commonDic = new SerializableDictionary<string, Common>();

        public SerializableDictionary<string, string> commonDics = new SerializableDictionary<string, string>();
        public SerializableDictionary<string, object> commonDico = new SerializableDictionary<string, object>();
        public List<object> commonListo = new List<object>();
        public AppInfoCenterConfig()
        {
            commonList.Add(new Common());
            commonList.Add(new Common());

            commonDic.Add("k1", new Common());
            commonDic.Add("k2", new Common());

            commonDics.Add("k1", "test1");
            commonDics.Add("k2", "test2");
            commonDico.Add("k1", 4);
            commonDico.Add("k22",true);
            commonListo.Add(12);
        }

    }
    [ConfigEntity]
    public class Common
    {
        [ConfigItem(FriendlyName = "当DB不可用时保存本地")]
        public readonly bool SaveToLocalWhenDBUnavailable = true;
        public string LocalMemoryQueuePath = "";
        public int WorkThreadCount = Environment.ProcessorCount * 2;
        public int WorkThreadSleepInterval = 50;
        public int SubmitToServerBatchSize = 500;
        public int SubmitToServerTimeOut = 60;
        public int MemoryQueueMaxCount = 50000;
        public int RefreshDomainIPPageInternal = 60000;
        [ConfigItem]
        public TradingServiceType ts = TradingServiceType.Consignment;
        [ConfigItem]
        public Styles styles = Styles.ShowBorder| Styles.ShowCaption;
        public decimal totalProce = (decimal)23.09;
        public DateTime DateTime = DateTime.Now;
        public TimeSpanEx TimeSpanField = TimeSpan.FromSeconds(12);
        public string StringField;
        public string NewAddedStringField;

    }
}
