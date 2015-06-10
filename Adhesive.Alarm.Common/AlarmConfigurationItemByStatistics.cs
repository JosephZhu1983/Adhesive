using System;

using Adhesive.Config;

namespace Adhesive.Alarm.Common
{
    [ConfigEntity(FriendlyName = "基于统计值的报警配置")]
    [Serializable]
    public class AlarmConfigurationItemByStatistics : AlarmConfigurationItemBase
    {
        [ConfigItem(FriendlyName = "详细数据的类型")]
        public AlarmDetailType DetailType { get; set; }

        [ConfigItem(FriendlyName = "详细数据要显示的列名",Description = "仅针对AlarmDetailType=ShowColumnContent方式，这个配置的是友好名")]
        public string AlarmDetailShowColumnContentColumnDisplayName { get; set; }

        public AlarmConfigurationItemByStatistics()
        {
            DetailType = AlarmDetailType.ShowGroupTopContent;
            AlarmDetailShowColumnContentColumnDisplayName = "";            
        }
    }    
}
