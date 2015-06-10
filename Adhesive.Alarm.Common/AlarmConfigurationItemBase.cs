


using System;
using System.Collections.Generic;
using Adhesive.Config;
namespace Adhesive.Alarm.Common
{
    [Serializable]
    public class AlarmConfigurationItemBase
    {
        [ConfigItem(FriendlyName = "配置名")]
        public string ConfigName { get; set; }

        [ConfigItem(FriendlyName = "描述")]
        public string Description { get; set; }

        [ConfigItem(FriendlyName = "数据量阀值")]
        public int Value { get; set; }

        [ConfigItem(FriendlyName = "检查时间间隔")]
        public TimeSpan CheckTimeSpan { get; set; }

        [ConfigItem(FriendlyName = "数据跨度时间间隔")]
        public TimeSpan DataTimeSpan { get; set; }

        [ConfigItem(FriendlyName = "处理事件的超时时间" , Description = "超过这个时间，事件会被系统关闭，报警重新开启")]
        public TimeSpan ProcessTimeout { get; set; }

        [ConfigItem(FriendlyName = "数据库名")]
        public string DatabasePrefix { get; set; }

        [ConfigItem(FriendlyName = "表名")]
        public string TableName { get; set; }

        [ConfigItem(FriendlyName = "条件类型")]
        public AlarmConditionType ConditionType { get; set; }

        [ConfigItem(FriendlyName = "接收者的组名")]
        public Dictionary<string, string> AlarmReceiverGroupNames { get; set; }

        [ConfigItem(FriendlyName = "数据过滤")]
        public Dictionary<string, object> Filters { get; set; }
    }
}
