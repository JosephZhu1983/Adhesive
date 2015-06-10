using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Adhesive.Alarm.Common;
using Adhesive.Common;
using Adhesive.Persistence;
using System.Threading;
using Adhesive.AppInfoCenter;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb.Silverlight.Web
{
    [DataContract]
    public class GetAlarmItemsResult
    {
        [DataMember]
        public List<AlarmItem> Data { get; set; }
        [DataMember]
        public int Count { get; set; }
    }

    public partial class DataService
    {
        private static MailService mailService = new MailService();
        private static MobileService mobileService = new MobileService();
        private static IMongodbQueryService mongodbQueryService = MongodbService.MongodbQueryService;

        static DataService()
        {
            mailService.Init(AlarmConfigurationBase.GetConfig().MailSmtp, AlarmConfigurationBase.GetConfig().MailUsername, AlarmConfigurationBase.GetConfig().MailPassword);
            mobileService.Init(AlarmConfigurationBase.GetConfig().MobileCategoryId);
        }

        [OperationContract]
        public GetAlarmItemsResult GetAlarmItems(AlarmStatus status, MongodbAdminConfigurationItem admin, int pageSize, int pageIndex)
        {
            try
            {
                var databases = admin.MongodbAdminDatabaseConfigurationItems.Values.Select(b => b.DatabasePrefix).Distinct().ToList();
                var tables = admin.MongodbAdminDatabaseConfigurationItems.Values.SelectMany(b => b.MongodbAdminTableConfigurationItems).Select(c => c.Value.TableName).Distinct().ToList();

                GetAlarmItemsResult r = new GetAlarmItemsResult();
                var dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
                using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
                {
                    var q = context.AlarmItems.Where(_ => _.AlarmStatusId == (int)status);
                    if (!databases.Contains("*"))
                        q = q.Where(_ => databases.Contains(_.AlarmDatabaseName));
                    if (!tables.Contains("*"))
                        q = q.Where(_ => tables.Contains(_.AlarmTableName));
                    r.Count = q.Count();
                    r.Data = q.OrderByDescending(_ => _.OpenTime)
                        .Skip(pageIndex * pageSize).Take(pageSize).ToList();
                    return r;
                }
            }
            catch (Exception ex)
            {
                ex.Handle("GetAlarmItems");
                throw;
            }
        }

        [OperationContract]
        public List<AlarmProcessItem> GetAlarmProcessItem(string id)
        {
            try
            {
                var dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
                using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
                {
                    return context.AlarmProcessItems.Where(a => a.AlarmItemId == id).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Handle("GetAlarmProcessItem");
                throw;
            }
        }

        [OperationContract]
        public List<string> GetAlarmGroup()
        {
            var group = AlarmConfigurationBase.GetConfig().AlarmReceiverGroups.Values.Select(a => a.GroupName).ToList();
            return group;
        }

        [OperationContract]
        public List<MongodbAdminConfigurationItem> GetAlarmReceivers(string groupName)
        {
            List<MongodbAdminConfigurationItem> receivers = new List<MongodbAdminConfigurationItem>();
            var group = AlarmConfigurationBase.GetConfig().AlarmReceiverGroups.Values.FirstOrDefault(g => g.GroupName == groupName);
            foreach (var r in group.AlarmReceivers)
            {
                var admin = mongodbQueryService.GetAdminConfigurationInternal(r.Value.Name);
                if (admin != null)
                {
                    receivers.Add(admin);
                }
            }
            return receivers;
        }


        [OperationContract]
        private void SendMessage(string configName, string mobileMessage, string mailMessage)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    var configitem = AlarmConfigurationBase.GetConfig().AlarmConfigurationByStatistics.Values.FirstOrDefault(c => c.ConfigName == configName);
                    if (configitem != null)
                    {
                        foreach (var groupName in configitem.AlarmReceiverGroupNames.Values)
                        {
                            var group = AlarmConfigurationBase.GetConfig().AlarmReceiverGroups.Values.FirstOrDefault(g => g.GroupName == groupName);
                            foreach (var r in group.AlarmReceivers)
                            {
                                var admin = mongodbQueryService.GetAdminConfigurationInternal(r.Value.Name);
                                if (admin != null)
                                {
                                    if (group.EnableMobileMessage)
                                    {
                                        var mobile = new MobileItem
                                        {
                                            MobileNumber = admin.MobileNumber,
                                            MobileMessage = mobileMessage,
                                        };
                                        mobileService.Send(new List<MobileItem> { mobile });

                                    }

                                    if (group.EnableMailMessage)
                                    {
                                        var mail = new MailItem
                                        {
                                            MailTitle = mailMessage,
                                            MailBody = mailMessage,
                                            MailAddress = admin.MailAddress,
                                        };
                                        mailService.Send(new List<MailItem> { mail });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Handle("SendMessage");
                    throw;
                }
            });
        }

        [OperationContract]
        public void SendMobile(List<string> userNames,string Message)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    foreach (var username in userNames)
                    {
                        var admin = mongodbQueryService.GetAdminConfigurationInternal(username);
                        if (admin != null)
                        {
                            var mobile = new MobileItem
                                        {
                                            MobileNumber = admin.MobileNumber,
                                            MobileMessage = Message,
                                        };
                            mobileService.Send(new List<MobileItem> { mobile });
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Handle("SendMessage");
                    throw;
                }
            });
        }

        [OperationContract]
        public void SendEmail(List<string> userNames, string Message)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    {
                        foreach (var username in userNames)
                        {
                            var admin = mongodbQueryService.GetAdminConfigurationInternal(username);
                            if (admin != null)
                            {
                                var mail = new MailItem
                                {
                                    MailTitle = Message,
                                    MailBody = Message,
                                    MailAddress = admin.MailAddress,
                                };
                                mailService.Send(new List<MailItem> { mail });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Handle("SendMessage");
                    throw;
                }
            });
        }

        [OperationContract]
        public void HandleAlarmEvent(string id, string mobileMessage, string mailMessage, string userName, string userRealName)
        {
            try
            {
                var dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
                using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
                {
                    var item = context.AlarmItems.SingleOrDefault(a => a.Id == id && a.AlarmStatusId != (int)AlarmStatus.Closed);
                    if (item != null)
                    {
                        item.HandleTime = DateTime.Now;
                        item.AlarmStatus = AlarmStatus.Handling;
                        var process = new AlarmProcessItem
                        {
                            AlarmItemId = id,
                            EventTime = DateTime.Now,
                            MobileComment = string.Format("{0} 接手了事件： {1}，备注：{2}", userName + "/" + userRealName, item.AlarmConfigName, mobileMessage),
                            MailComment = string.Format("{0} 接手了事件： {1}，备注：{2}", userName + "/" + userRealName, item.AlarmConfigName, mailMessage),
                            ProcessUserName = userName,
                            ProcessUserRealName = userRealName,
                            AlarmStatus = AlarmStatus.Handling,
                        };
                        context.AlarmProcessItems.Add(process);
                        context.SaveChanges();

                        if (item.AlarmStatus == AlarmStatus.Handling)
                        {
                            mobileMessage = string.Format("{0} 接手了事件： {1}，备注：{2}", userName + "/" + userRealName, item.AlarmConfigName, mobileMessage);
                            mailMessage = string.Format("{0} 接手了事件： {1}，备注：{2}", userName + "/" + userRealName, item.AlarmConfigName, mailMessage);
                        }
                        else
                        {
                            mobileMessage = string.Format("{0} 开始处理事件： {1}，备注：{2}", userName + "/" + userRealName, item.AlarmConfigName, mobileMessage);
                            mailMessage = string.Format("{0} 开始处理事件： {1}，备注：{2}", userName + "/" + userRealName, item.AlarmConfigName, mailMessage);
                        }
                        SendMessage(item.AlarmConfigName, mobileMessage, mailMessage);                       
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle("HandleAlarmEvent");
                throw;
            }
        }



        [OperationContract]
        public void CloseAlarmEvent(string id, string mobileMessage, string mailMessage, string userName, string userRealName)
        {
            try
            {
                var dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
                using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
                {
                    var item = context.AlarmItems.SingleOrDefault(a => a.Id == id && a.AlarmStatusId != (int)AlarmStatus.Closed);
                    if (item != null)
                    {
                        item.CloseTime = DateTime.Now;
                        item.AlarmStatus = AlarmStatus.Closed;
                        var process = new AlarmProcessItem
                        {
                            AlarmItemId = id,
                            EventTime = DateTime.Now,
                            MobileComment = string.Format("{0} 关闭了事件： {1}，结论：{2}", userRealName, item.AlarmConfigName, mobileMessage),
                            MailComment = string.Format("{0} 关闭了事件： {1}，结论：{2}", userRealName, item.AlarmConfigName, mailMessage),
                            ProcessUserName = userName,
                            ProcessUserRealName = userRealName,
                            AlarmStatus = AlarmStatus.Closed,
                        };
                        context.AlarmProcessItems.Add(process);
                        context.SaveChanges();

                        mobileMessage = string.Format("{0} 关闭了事件： {1}，结论：{2}", userRealName, item.AlarmConfigName, mobileMessage);
                        mailMessage = string.Format("{0} 关闭了事件： {1}，结论：{2}", userRealName, item.AlarmConfigName, mailMessage);
                        SendMessage(item.AlarmConfigName, mobileMessage, mailMessage);                      
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle("CloseAlarmEvent");
                throw;
            }
        }
    }
}