using System;

using System.Collections.Generic;
using Adhesive.Config;

namespace Adhesive.Alarm.Common
{
    [ConfigEntity(FriendlyName = "报警服务配置")]
    [Serializable]
    public class AlarmConfigurationEntity
    {
        [ConfigItem(FriendlyName = "短信服务的类别ID")]
        public int MobileCategoryId { get; set; }

        [ConfigItem(FriendlyName = "邮件服务器地址")]
        public string MailSmtp { get; set; }

        [ConfigItem(FriendlyName = "邮件服务器用户名")]
        public string MailUsername { get; set; }

        [ConfigItem(FriendlyName = "邮件服务器密码")]
        public string MailPassword { get; set; }

        [ConfigItem(FriendlyName = "邮件标题模板")]
        public string MailTitleTemplate { get; set; }

        [ConfigItem(FriendlyName = "邮件正文模板")]
        public string MailBodyTemplate { get; set; }

        [ConfigItem(FriendlyName = "短消息模板")]
        public string MobileMessageTemlate { get; set; }

        [ConfigItem(FriendlyName = "日志消息模板")]
        public string LogMessageTemlate { get; set; }

        [ConfigItem(FriendlyName = "发邮件的间隔毫秒")]
        public int MailMessageInerval { get; set; }

        [ConfigItem(FriendlyName = "发短消息的间隔毫秒")]
        public int MobileMessageInerval { get; set; }

        [ConfigItem(FriendlyName = "发邮件出错后的间隔毫秒")]
        public int MailMessageErrorInerval { get; set; }

        [ConfigItem(FriendlyName = "发短消息出错后的间隔毫秒")]
        public int MobileMessageErrorInerval { get; set; }

        [ConfigItem(FriendlyName = "状态数据的报警配置")]
        public Dictionary<string, AlarmConfigurationItemByState> AlarmConfigurationByStates = new Dictionary<string, AlarmConfigurationItemByState>();

        [ConfigItem(FriendlyName = "统计数据的报警配置")]
        public Dictionary<string, AlarmConfigurationItemByStatistics> AlarmConfigurationByStatistics = new Dictionary<string, AlarmConfigurationItemByStatistics>();

        [ConfigItem(FriendlyName = "报警接收者的配置")]
        public Dictionary<string, AlarmReceiverGroupConfiguration> AlarmReceiverGroups = new Dictionary<string, AlarmReceiverGroupConfiguration>();
    }
}
