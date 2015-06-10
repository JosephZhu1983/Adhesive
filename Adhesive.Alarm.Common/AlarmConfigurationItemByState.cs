using System;

using Adhesive.Config;
namespace Adhesive.Alarm.Common
{
    [ConfigEntity(FriendlyName = "基于状态值的报警配置")]
    [Serializable]
    public class AlarmConfigurationItemByState : AlarmConfigurationItemBase
    {
        [ConfigItem(FriendlyName = "列名", Description = "模糊匹配")]
        public string ColumnName { get; set; }
    }
}
