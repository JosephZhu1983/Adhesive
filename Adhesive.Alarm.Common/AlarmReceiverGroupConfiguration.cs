
using System;
using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.Alarm.Common
{
    [ConfigEntity(FriendlyName = "报警服务接收组")]
    [Serializable]
    public class AlarmReceiverGroupConfiguration
    {
        [ConfigItem(FriendlyName = "组名")]
        public string GroupName { get; set; }

        [ConfigItem(FriendlyName = "开启邮件消息")]
        public bool EnableMailMessage { get; set; }

        [ConfigItem(FriendlyName = "开启手机消息")]
        public bool EnableMobileMessage { get; set; }

        [ConfigItem(FriendlyName = "对于一个配置发手机消息的间隔")]
        public TimeSpan MobileMessageIntervalTimeSpan { get; set; }

        [ConfigItem(FriendlyName = "对于一个配置发邮件消息的间隔")]
        public TimeSpan MailMessageIntervalTimeSpan { get; set; }

        [ConfigItem(FriendlyName = "所有接收者")]
        public Dictionary<string, AlarmReceiverConfiguration> AlarmReceivers { get; set; }
    }
}
