
using System;
using System.Collections.Generic;
using Adhesive.Common;
using Adhesive.Config;

namespace Adhesive.Alarm.Common
{
    public class AlarmConfigurationBase
    {
        public static readonly string ModuleName = "报警服务模块";
        private static IConfigService configService = LocalServiceLocator.GetService<IConfigService>();

        public static AlarmConfigurationEntity GetConfig()
        {
            var defaultConfig = GetDefaultConfig();
            var config = configService.GetConfigItemValue(true, "AlarmConfiguration", defaultConfig);
            return config;
        }

        public static AlarmConfigurationEntity GetDefaultConfig()
        {
            var defaultConfig = new AlarmConfigurationEntity
            {
                MailTitleTemplate = "{ConfigName} ({Description}) {ColumnName} {DataTimeSpanSeconds}秒 {ActualItemCount} {ConditionType} {ItemCount}",
                MailBodyTemplate = "{ConfigName} ({Description}) {ColumnName} {DataTimeSpanSeconds}秒 {ActualItemCount} {ConditionType} {ItemCount} {Detail}",
                MobileMessageTemlate = "{ConfigName} ({Description}) {ColumnName} {DataTimeSpanSeconds}秒 {ActualItemCount} {ConditionType} {ItemCount} {Detail}",
                LogMessageTemlate = "{ConfigName} ({Description}) {ColumnName} {DataTimeSpanSeconds}秒 {ActualItemCount} {ConditionType} {ItemCount}",
                MailMessageInerval = 200,
                MobileMessageInerval = 200,
                MailMessageErrorInerval = 2000,
                MobileMessageErrorInerval = 2000,
                MailUsername = "aic@5173.com",
                MailPassword = "sYwfRY2t0CTh58W",
                MailSmtp = "mail.5173.com",
                MobileCategoryId = 7210,
                

                AlarmConfigurationByStates = new Dictionary<string, AlarmConfigurationItemByState>
                {
                    { 
                         "Mongodb服务端内存队列监控", new AlarmConfigurationItemByState
                         {
                             ConfigName = "Mongodb服务端内存队列监控",
                             Description = "Mongodb服务端内存队列监控",
                             CheckTimeSpan = TimeSpan.FromSeconds(10),
                             DataTimeSpan = TimeSpan.FromMinutes(1),
                             ConditionType = AlarmConditionType.MoreThan,
                             AlarmReceiverGroupNames = new Dictionary<string,string>
                             {
                                 {"测试组", "测试"},
                             },
                             DatabasePrefix = "State__MongodbServer",
                             TableName = "Adhesive.Mongodb.Server",
                             ColumnName = "CIC",
                             Value = 100,
                             ProcessTimeout = TimeSpan.FromHours(1),
                         }
                     }
                },
                AlarmConfigurationByStatistics = new Dictionary<string, AlarmConfigurationItemByStatistics>
                {
                     { 
                         "Adhesive.Test.WebApp错误日志监控", new AlarmConfigurationItemByStatistics
                         {
                             ConfigName = "Adhesive.Test.WebApp错误日志监控",
                             Description = "Adhesive.Test.WebApp错误日志监控",
                             CheckTimeSpan = TimeSpan.FromSeconds(10),
                             DataTimeSpan = TimeSpan.FromMinutes(1),
                             ConditionType = AlarmConditionType.MoreThan,
                             AlarmReceiverGroupNames = new Dictionary<string,string>
                             {
                                 {"测试组", "测试"},
                             },
                             Filters = new Dictionary<string, object>
                             {
                                 {"L", 4 }
                             },
                             DatabasePrefix = "Aic__Log",
                             TableName = "Adhesive.Test.WebApp",
                             Value = 100,
                             ProcessTimeout = TimeSpan.FromHours(1),
                             DetailType = AlarmDetailType.ShowGroupTopContent,
                             AlarmDetailShowColumnContentColumnDisplayName = "日志消息",
                         }
                     },
                },
                AlarmReceiverGroups = new Dictionary<string, AlarmReceiverGroupConfiguration>
                {
                    {
                        "测试", new AlarmReceiverGroupConfiguration
                        {
                            GroupName = "测试",
                            MailMessageIntervalTimeSpan = TimeSpan.FromSeconds(30),
                            MobileMessageIntervalTimeSpan = TimeSpan.FromSeconds(30),
                            EnableMailMessage  =true,
                            EnableMobileMessage = true,
                            AlarmReceivers = new Dictionary<string,AlarmReceiverConfiguration>
                            {
                                {
                                    "朱晔", new AlarmReceiverConfiguration
                                    {
                                        Name = "Admin",
                                    }                                    
                                },
                                {
                                    "周国选", new AlarmReceiverConfiguration
                                    {
                                        Name = "zhougx",
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return defaultConfig;
        }
    }
}
