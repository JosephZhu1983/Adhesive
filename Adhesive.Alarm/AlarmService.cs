
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Adhesive.Alarm.Common;
using Adhesive.AppInfoCenter;
using Adhesive.Common;
using Adhesive.MemoryQueue;
using Adhesive.Mongodb;
using Adhesive.Persistence;

namespace Adhesive.Alarm
{
    public class AlarmService : AbstractService
    {
        private static IMemoryQueueService mailMemoryQueueService = LocalServiceLocator.GetService<IMemoryQueueService>();
        private static IMemoryQueueService mobileMemoryQueueService = LocalServiceLocator.GetService<IMemoryQueueService>();
        private static List<AlarmServiceState> alarmServiceStates = new List<AlarmServiceState>();
        private static IMongodbQueryService mongodbQueryService = MongodbService.MongodbQueryService;
        private static MailService mailService = new MailService();
        private static MobileService mobileService = new MobileService();
        private static Timer clearExpiredAlarmItemTimer;

        private static MongodbAdminConfigurationItem GetMongodbAdminConfigurationItem(string name)
        {
            return mongodbQueryService.GetAdminConfigurationInternal(name);
        }

        private static void InitClearExpiredAlarmItem()
        {
            clearExpiredAlarmItemTimer = new Timer(state =>
            {
                try
                {
                    var dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
                    using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
                    {
                        var openItems = context.AlarmItems.Where(a => a.AlarmStatusId == (int)AlarmStatus.Handling).ToList();
                        foreach (var openItem in openItems)
                        {
                            var configItem = AlarmConfiguration.GetAlarmConfigurationItem(openItem.AlarmConfigName);
                            if (configItem != null && openItem.HandleTime < DateTime.Now.Subtract(configItem.ProcessTimeout))
                            {
                                openItem.AlarmStatus = AlarmStatus.Closed;
                                openItem.CloseTime = DateTime.Now;

                                var process = new AlarmProcessItem
                                {
                                    AlarmItemId = openItem.Id,
                                    EventTime = DateTime.Now,
                                    MobileComment = string.Format("事件在 {0} 内没有处理完成，系统自动关闭", configItem.ProcessTimeout),
                                    MailComment = string.Format("事件在 {0} 内没有处理完成，系统自动关闭", configItem.ProcessTimeout),
                                    ProcessUserName = "",
                                    ProcessUserRealName = "",
                                    AlarmStatus = AlarmStatus.Closed,
                                };
                                context.AlarmProcessItems.Add(process);
                            }
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Handle(AlarmConfigurationBase.ModuleName, "AlarmService", "InitClearExpiredAlarmItem");
                }
            }, null, 0, 1000 * 60);
        }

        private static bool AlarmIsHandling(string configName)
        {
            try
            {
                var dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
                using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
                {
                    var item = context.AlarmItems.FirstOrDefault(a => a.AlarmStatusId != (int)AlarmStatus.Closed
                        && a.AlarmConfigName == configName);
                    if (item != null)
                    {
                        if (item.AlarmStatus != AlarmStatus.Closed)
                        {
                            item.AlarmTimes++;
                            context.SaveChanges();
                        }

                        if (item.AlarmStatus == AlarmStatus.Handling)
                            return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.Handle(AlarmConfigurationBase.ModuleName, "AlarmService", "AlarmIsHandling");
                return false;
            }
        }

        private static void CreateAlarmEvent(AlarmConfigurationItemBase configItem)
        {
            try
            {
                var dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
                using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
                {
                    var item = context.AlarmItems.FirstOrDefault(a => a.AlarmStatusId != (int)AlarmStatus.Closed
                        && a.AlarmConfigName == configItem.ConfigName);
                    if (item == null)
                    {
                        item = new AlarmItem()
                        {
                            AlarmDatabaseName = configItem.DatabasePrefix,
                            AlarmTableName = configItem.TableName,
                            AlarmConfigName = configItem.ConfigName,
                            AlarmStatus = AlarmStatus.Open,
                            OpenTime = DateTime.Now,
                        };
                        context.AlarmItems.Add(item);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(AlarmConfigurationBase.ModuleName, "AlarmService", "CreateAlarmEvent");
            }
        }

        private static void CheckActionForState(object state)
        {
            var configItemName = state as string;
            if (configItemName == null) return;

            var configItem = AlarmConfiguration.GetAlarmConfigurationItemByState(configItemName);
            if (configItem == null)
            {
                AppInfoCenterService.LoggingService.Warning(AlarmConfigurationBase.ModuleName, "AlarmService", "CheckActionForState",
                    string.Format("没取到configItem，配置项名字 {0}", configItemName));
                return;
            }

            var alarmState = alarmServiceStates.FirstOrDefault(s => s.AlarmConfigurationItemName == configItemName);
            if (alarmState == null)
            {
                AppInfoCenterService.LoggingService.Warning(AlarmConfigurationBase.ModuleName, "AlarmService", "CheckActionForState",
                    string.Format("没取到alarmState，配置项名字 {0}", configItemName));
                return;
            }

            try
            {
                var stateResult = mongodbQueryService.GetStateData(configItem.DatabasePrefix, configItem.TableName, DateTime.Now.Subtract(configItem.DataTimeSpan), DateTime.Now, configItem.Filters);
                if (stateResult == null)
                    return;
                var columns = new List<Detail>();
                stateResult.Data.ForEach(detail => InternalCheckStateColumn(detail, configItem.ColumnName, columns));
                columns.Each(column =>
                {
                    var count = Convert.ToInt32(column.Value);
                    var columnName = string.Format("{0}:{1}", column.DisplayName, column.ColumnName);
                    IntervalCheckAction(configItem, count, columnName, alarmState, null);
                });
            }
            catch (Exception ex)
            {
                ex.Handle(AlarmConfigurationBase.ModuleName, "AlarmService", "CheckActionForState",
                    string.Format("CheckActionForState出现异常！配置项名字 {0}", configItemName));
            }
        }

        private static void InternalCheckStateColumn(Detail detail, string columnName, List<Detail> stateColumns)
        {
            if (detail.SubDetails == null)
            {
                if (detail.ColumnName.ToLower().Contains(columnName.ToLower()))
                {
                    stateColumns.Add(detail);
                }
            }
            else if (detail.SubDetails.Count > 0)
            {
                detail.SubDetails.ForEach(subdetail => InternalCheckStateColumn(subdetail, columnName, stateColumns));
            }
        }

        private static void CheckActionForStatistics(object state)
        {
            var configItemName = state as string;
            var configItem = AlarmConfiguration.GetAlarmConfigurationItemByStatistics(configItemName);
            if (configItem == null)
            {
                AppInfoCenterService.LoggingService.Warning(AlarmConfigurationBase.ModuleName, "AlarmService ", "CheckActionForStatistics",
                    string.Format("没取到configItem，配置项名字 {0}", configItemName));
                return;
            }

            var alarmState = alarmServiceStates.FirstOrDefault(s => s.AlarmConfigurationItemName == configItemName);
            if (alarmState == null)
            {
                AppInfoCenterService.LoggingService.Warning(AlarmConfigurationBase.ModuleName, "AlarmService", "CheckActionForStatistics",
                    string.Format("没取到alarmState，配置项名字 {0}", configItemName));
                return;
            }

            try
            {
                var count = mongodbQueryService.GetDataCount(configItem.DatabasePrefix, configItem.TableName, DateTime.Now.Subtract(configItem.DataTimeSpan), DateTime.Now, configItem.Filters);
                IntervalCheckAction(configItem, count, "量", alarmState, () =>
                {
                    if (configItem.DetailType == AlarmDetailType.ShowColumnContent && !string.IsNullOrWhiteSpace(configItem.AlarmDetailShowColumnContentColumnDisplayName))
                    {
                        try
                        {
                            var list = mongodbQueryService.GetTableData(configItem.DatabasePrefix, new List<string> { configItem.TableName },
                                DateTime.Now.Subtract(configItem.DataTimeSpan), DateTime.Now, 0, 1, configItem.Filters);
                            if (list.Count > 0)
                            {
                                var l = list.First();
                                if (l.Tables.Count > 0)
                                {
                                    var ll = l.Tables.First();
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append("详:");
                                    foreach (var item in ll.Data)
                                    {
                                        if (item.ContainsKey(configItem.AlarmDetailShowColumnContentColumnDisplayName))
                                        {
                                            sb.AppendFormat("{0},", item[configItem.AlarmDetailShowColumnContentColumnDisplayName]);
                                        }
                                    }
                                    return sb.ToString().TrimEnd(',');
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Handle(AlarmConfigurationBase.ModuleName, "AlarmService", "CheckActionForStatistics",
                                string.Format("获取ShowColumnContent方式的详细数据出现异常！配置项名字 {0}，列名 {1}", configItemName, configItem.AlarmDetailShowColumnContentColumnDisplayName));
                        }
                    }
                    else if (configItem.DetailType == AlarmDetailType.ShowGroupTopContent)
                    {
                        try
                        {
                            var group = mongodbQueryService.GetGroupData(configItem.DatabasePrefix, new List<string> { configItem.TableName }, DateTime.Now.Subtract(configItem.DataTimeSpan), DateTime.Now, configItem.Filters);
                            if (group.Count > 0)
                            {
                                var g = group.First();
                                StringBuilder sb = new StringBuilder();
                                sb.Append("详：");
                                foreach (var item in g.GroupItems)
                                {
                                    if (item.Values.Count >0)
                                    {
                                        var top = item.Values.OrderByDescending(v => v.Value).First();
                                        if (top.Key != null && !string.IsNullOrEmpty(top.Key.DisplayName))
                                            sb.AppendFormat("{0}({1}),", top.Key.DisplayName, top.Value);
                                    }
                                }
                                return sb.ToString().TrimEnd(',');
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Handle(AlarmConfigurationBase.ModuleName, "AlarmService", "CheckActionForStatistics",
                                string.Format("获取ShowGroupTopContent方式的详细数据出现异常！配置项名字 {0}", configItemName));
                        }
                    }
                    return "";
                });
            }
            catch (Exception ex)
            {
                ex.Handle(AlarmConfigurationBase.ModuleName, "AlarmService", "CheckActionForStatistics",
                    string.Format("CheckActionForStatistics出现异常！配置项名字 {0}", configItemName));
            }
        }

        private static void IntervalCheckAction(AlarmConfigurationItemBase configItem, int count, string columnName, AlarmServiceState state, Func<string> getDetail)
        {
            var needAction = false;
            switch (configItem.ConditionType)
            {
                case AlarmConditionType.LessThan:
                    {
                        if (count < configItem.Value)
                            needAction = true;
                        break;
                    }
                case AlarmConditionType.LessThanAndEqualTo:
                    {
                        if (count <= configItem.Value)
                            needAction = true;
                        break;
                    }
                case AlarmConditionType.MoreThan:
                    {
                        if (count > configItem.Value)
                            needAction = true;
                        break;
                    }
                case AlarmConditionType.MoreThanAndEqualTo:
                    {
                        if (count >= configItem.Value)
                            needAction = true;
                        break;
                    }
            }
            if (!needAction)
            {
                var logMessage = FormatMessage(AlarmConfiguration.GetConfig().LogMessageTemlate, configItem, count, columnName, null);
                AppInfoCenterService.LoggingService.Debug(AlarmConfigurationBase.ModuleName, "AlarmService", "IntervalCheckAction",
                    string.Format("{0} 没达到报警条件", logMessage), new ExtraInfo
                {
                    DropDownListFilterItem1 = "没达到报警条件",
                    DropDownListFilterItem2 = configItem.ConfigName,
                });
                return;
            }

            foreach (var groupName in configItem.AlarmReceiverGroupNames.Values)
            {
                InternalHandleAlarm(configItem, state.AlarmServiceStateItems[groupName], groupName, count, columnName, getDetail);
            }
        }

        private static void InternalHandleAlarm(AlarmConfigurationItemBase configItem, AlarmServiceStateItem state, string groupName, int count, string columnName, Func<string> getDetail)
        {
            var group = AlarmConfiguration.GetConfig().AlarmReceiverGroups.Values.FirstOrDefault(g => g.GroupName == groupName);
            if (group == null) return;
            var logMessgae = FormatMessage(AlarmConfiguration.GetConfig().LogMessageTemlate, configItem, count, columnName, null);

            CreateAlarmEvent(configItem);

            bool b = AlarmIsHandling(configItem.ConfigName);

            if (group.EnableMailMessage)
            {
                var mailPastTime = DateTime.Now - state.AlarmReceiverGroupLastMailMessageTime;
                if (b)
                {
                    AppInfoCenterService.LoggingService.Info(AlarmConfigurationBase.ModuleName, "AlarmService", "InternalHandleAlarm", "事件正在处理，暂停报警",
                        new ExtraInfo
                        {
                            DropDownListFilterItem1 = "事件正在处理，暂停邮件报警",
                            DropDownListFilterItem2 = configItem.ConfigName,
                        });
                }
                else if (state.AlarmReceiverGroupLastMailMessageTime == DateTime.MinValue || mailPastTime > group.MailMessageIntervalTimeSpan)
                {
                    var mailItems = group.AlarmReceivers.Select(r =>
                    {
                        var contact = GetMongodbAdminConfigurationItem(r.Value.Name);
                        if (contact != null && !string.IsNullOrEmpty(contact.MailAddress))
                        {
                            return new MailItem
                            {
                                MailAddress = contact.MailAddress,
                                MailTitle = FormatMessage(AlarmConfiguration.GetConfig().MailTitleTemplate, configItem, count, columnName, getDetail),
                                MailBody = FormatMessage(AlarmConfiguration.GetConfig().MailBodyTemplate, configItem, count, columnName, getDetail),
                            };
                        }
                        return null;
                    }).ToList();

                    mailItems.Where(item => item != null).ToList().ForEach(mailItem =>
                    {
                        mailMemoryQueueService.Enqueue(mailItem);
                        AppInfoCenterService.LoggingService.Info(AlarmConfigurationBase.ModuleName, "AlarmService", "InternalHandleAlarm",
                            string.Format("{0} {1} > {2} 达到邮件发送条件 -> {3}", logMessgae, mailPastTime.ToString(), group.MailMessageIntervalTimeSpan.ToString(), mailItem.MailAddress),
                            new ExtraInfo
                            {
                                DropDownListFilterItem1 = "达到邮件发送条件",
                                DropDownListFilterItem2 = configItem.ConfigName,
                            });
                    });
                    state.AlarmReceiverGroupLastMailMessageTime = DateTime.Now;
                }
                else
                {
                    AppInfoCenterService.LoggingService.Info(AlarmConfigurationBase.ModuleName, "AlarmService", "InternalHandleAlarm",
                        string.Format("{0} {1} <= {2} 没达到邮件发送条件", logMessgae, mailPastTime.ToString(), group.MailMessageIntervalTimeSpan.ToString()),
                        new ExtraInfo
                        {
                            DropDownListFilterItem1 = "没达到邮件发送条件",
                            DropDownListFilterItem2 = configItem.ConfigName,
                        });
                }
            }

            if (group.EnableMobileMessage)
            {
                var mobilePastTime = DateTime.Now - state.AlarmReceiverGroupLastMobileMessageTime;
                if (b)
                {
                    AppInfoCenterService.LoggingService.Info(AlarmConfigurationBase.ModuleName, "AlarmService", "InternalHandleAlarm", "事件正在处理，暂停报警",
                        new ExtraInfo
                        {
                            DropDownListFilterItem1 = "事件正在处理，暂停短信报警",
                            DropDownListFilterItem2 = configItem.ConfigName,
                        });
                }
                else if (state.AlarmReceiverGroupLastMobileMessageTime == DateTime.MinValue || mobilePastTime > group.MobileMessageIntervalTimeSpan)
                {
                    var mobileItems = group.AlarmReceivers.Select(r =>
                    {
                        var contact = GetMongodbAdminConfigurationItem(r.Value.Name);
                        if (contact != null && !string.IsNullOrEmpty(contact.MobileNumber))
                        {
                            return new MobileItem
                            {
                                MobileNumber = contact.MobileNumber,
                                MobileMessage = FormatMessage(AlarmConfiguration.GetConfig().MobileMessageTemlate, configItem, count, columnName, getDetail),
                            };
                        };
                        return null;
                    }).ToList();

                    mobileItems.Where(item => item != null).ToList().ForEach(mobileItem =>
                    {
                        mobileMemoryQueueService.Enqueue(mobileItem);
                        AppInfoCenterService.LoggingService.Info(AlarmConfigurationBase.ModuleName, "AlarmService", "InternalHandleAlarm",
                            string.Format("{0} {1} > {2} 达到短信发送条件 -> {3}", logMessgae, mobilePastTime.ToString(), group.MobileMessageIntervalTimeSpan.ToString(), mobileItem.MobileNumber),
                            new ExtraInfo
                            {
                                DropDownListFilterItem1 = "达到短信发送条件",
                                DropDownListFilterItem2 = configItem.ConfigName,
                            });
                    });
                    state.AlarmReceiverGroupLastMobileMessageTime = DateTime.Now;
                }
                else
                {
                    AppInfoCenterService.LoggingService.Info(AlarmConfigurationBase.ModuleName, "AlarmService", "InternalHandleAlarm",
                        string.Format("{0} {1} <= {2} 没达到短信发送条件", logMessgae, mobilePastTime.ToString(), group.MobileMessageIntervalTimeSpan.ToString()), new ExtraInfo
                        {
                            DropDownListFilterItem1 = "没达到短信发送条件",
                            DropDownListFilterItem2 = configItem.ConfigName,
                        });
                }
            }
        }

        private static string FormatMessage(string message, AlarmConfigurationItemBase configItem, int count, string columnName, Func<string> getDetail)
        {
            message = message.Replace("{ConfigName}", configItem.ConfigName);
            message = message.Replace("{Description}", configItem.Description);
            message = message.Replace("{ColumnName}", columnName);
            message = message.Replace("{DataTimeSpanSeconds}", configItem.DataTimeSpan.TotalSeconds.ToString());
            message = message.Replace("{ActualItemCount}", count.ToString());
            var conditionType = "";
            switch (configItem.ConditionType)
            {
                case AlarmConditionType.LessThan:
                    conditionType = "<";
                    break;
                case AlarmConditionType.LessThanAndEqualTo:
                    conditionType = "<=";
                    break;
                case AlarmConditionType.MoreThan:
                    conditionType = ">";
                    break;
                case AlarmConditionType.MoreThanAndEqualTo:
                    conditionType = ">=";
                    break;
            }
            message = message.Replace("{ConditionType}", conditionType);
            message = message.Replace("{ItemCount}", configItem.Value.ToString());
            if (message.Contains("{Detail}"))
            {
                if (configItem is AlarmConfigurationItemByStatistics && getDetail != null)
                    message = message.Replace("{Detail}", getDetail());
                else
                    message = message.Replace("{Detail}", "");
            }
            return message;
        }

        public static new void Dispose()
        {
            alarmServiceStates.Each(alarmService => alarmService.CheckTimer.Dispose());
            mailMemoryQueueService.Dispose();
            mobileMemoryQueueService.Dispose();
        }

        internal static void Init()
        {
            mailMemoryQueueService = LocalServiceLocator.GetService<IMemoryQueueService>();
            mobileMemoryQueueService = LocalServiceLocator.GetService<IMemoryQueueService>();
            alarmServiceStates = new List<AlarmServiceState>();

            mailService = new MailService();
            mobileService = new MobileService();

            mailService.Init(AlarmConfiguration.GetConfig().MailSmtp, AlarmConfiguration.GetConfig().MailUsername, AlarmConfiguration.GetConfig().MailPassword);
            mobileService.Init(AlarmConfiguration.GetConfig().MobileCategoryId);

            mailMemoryQueueService.Init(new MemoryQueueServiceConfiguration("AlarmService_MailQueue", mailService.Send)
            {
                ConsumeItemCountInOneBatch = 1,
                ConsumeIntervalMilliseconds = AlarmConfiguration.GetConfig().MailMessageInerval,
                ConsumeIntervalWhenErrorMilliseconds = AlarmConfiguration.GetConfig().MailMessageErrorInerval,
                ConsumeErrorAction = MemoryQueueServiceConsumeErrorAction.EnqueueTwiceAndLogException
            });
            mobileMemoryQueueService.Init(new MemoryQueueServiceConfiguration("AlarmService_MobileQueue", mobileService.Send)
            {
                ConsumeItemCountInOneBatch = 1,
                ConsumeIntervalMilliseconds = AlarmConfiguration.GetConfig().MobileMessageInerval,
                ConsumeIntervalWhenErrorMilliseconds = AlarmConfiguration.GetConfig().MobileMessageErrorInerval,
                ConsumeErrorAction = MemoryQueueServiceConsumeErrorAction.EnqueueTwiceAndLogException
            });

            InitClearExpiredAlarmItem();

            var config = AlarmConfiguration.GetConfig();
            foreach (var item in config.AlarmConfigurationByStatistics)
            {
                var alarmServiceState = new AlarmServiceState
                {
                    AlarmConfigurationItemName = item.Value.ConfigName,
                    AlarmServiceStateItems = new Dictionary<string, AlarmServiceStateItem>(),
                };
                item.Value.AlarmReceiverGroupNames.Values.Each(groupName =>
                {
                    alarmServiceState.AlarmServiceStateItems.Add(groupName, new AlarmServiceStateItem
                    {
                        ReceiverGroupName = groupName,
                        AlarmReceiverGroupLastMailMessageTime = DateTime.MinValue,
                        AlarmReceiverGroupLastMobileMessageTime = DateTime.MinValue,
                    });
                });
                var interval = item.Value.CheckTimeSpan;
                var timer = new System.Threading.Timer(CheckActionForStatistics, item.Value.ConfigName, interval, interval);
                alarmServiceState.CheckTimer = timer;
                alarmServiceStates.Add(alarmServiceState);
            }

            foreach (var item in config.AlarmConfigurationByStates)
            {
                var alarmServiceState = new AlarmServiceState
                {
                    AlarmConfigurationItemName = item.Value.ConfigName,
                    AlarmServiceStateItems = new Dictionary<string, AlarmServiceStateItem>(),
                };
                item.Value.AlarmReceiverGroupNames.Values.Each(groupName =>
                {
                    alarmServiceState.AlarmServiceStateItems.Add(groupName, new AlarmServiceStateItem
                    {
                        ReceiverGroupName = groupName,
                        AlarmReceiverGroupLastMailMessageTime = DateTime.MinValue,
                        AlarmReceiverGroupLastMobileMessageTime = DateTime.MinValue,
                    });
                });
                var interval = item.Value.CheckTimeSpan;
                var timer = new System.Threading.Timer(CheckActionForState, item.Value.ConfigName, interval, interval);
                alarmServiceState.CheckTimer = timer;
                alarmServiceStates.Add(alarmServiceState);
            }

            AppInfoCenterService.LoggingService.Info(AlarmConfigurationBase.ModuleName, "AlarmService", "Init",
                string.Format("完成一次报警服务初始化，读取到 {0} 个状态报警，{1} 个统计报警", config.AlarmConfigurationByStates.Count.ToString(), config.AlarmConfigurationByStatistics.Count.ToString()));
        }
    }
}
