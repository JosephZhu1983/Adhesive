using System;
using Adhesive.Alarm.Common;
using Adhesive.Common;
using Adhesive.Persistence;

namespace Adhesive.Alarm.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            AdhesiveFramework.Start();
            Console.ReadLine();

            IDbContextFactory dbContextFactory = LocalServiceLocator.GetService<IDbContextFactory>();
            using (var context = dbContextFactory.CreateContext<AlarmDbContext>())
            {
                var alarm1 = new AlarmItem
                {
                    AlarmConfigName = "Asdadsad",
                    AlarmStatus = AlarmStatus.Open,
                    OpenTime = DateTime.Now,
                    
                    AlarmTimes = 10,
                };

                var alarm2 = new AlarmItem
                {
                    AlarmConfigName = "Asdadsad",
                    AlarmStatus = AlarmStatus.Handling,
                    HandleTime = DateTime.Now,
                    OpenTime = DateTime.Now,
                    AlarmTimes = 10,
                };

                var alarm3 = new AlarmItem
                {
                    AlarmConfigName = "Asdadsad",
                    AlarmStatus = AlarmStatus.Closed,
                    AlarmTimes = 10,
                    OpenTime = DateTime.Now,
                    CloseTime = DateTime.Now,
                    HandleTime = DateTime.Now,
                };

                var alarmprocess1 = new AlarmProcessItem()
                {
                    AlarmItem = alarm3,
                    MailComment = "邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息",
                    MobileComment = "短信消息短信消息短信消息短信消息短信消息短信消息",
                    AlarmStatus = AlarmStatus.Handling,

                    ProcessUserName = "zhuye",
                    ProcessUserRealName = "朱晔",
                    EventTime = DateTime.Now,
                };

                var alarmprocess2 = new AlarmProcessItem()
                {
                    AlarmItem = alarm3,
                    MailComment = "邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息",
                    MobileComment = "短信消息短信消息短信消息短信消息短信消息短信消息",
                    AlarmStatus = AlarmStatus.Handling,

                    ProcessUserName = "zhuye",
                    ProcessUserRealName = "朱晔",
                    EventTime = DateTime.Now,
                };

                var alarmprocess3 = new AlarmProcessItem()
                {
                    AlarmItem = alarm3,
                    MailComment = "邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息邮件消息",
                    MobileComment = "短信消息短信消息短信消息短信消息短信消息短信消息",
                    AlarmStatus = AlarmStatus.Closed,
                    ProcessUserName = "zhuye",
                    ProcessUserRealName = "朱晔",
                    EventTime = DateTime.Now,
                };

                context.AlarmItems.Add(alarm1);
                context.AlarmItems.Add(alarm2);
                context.AlarmItems.Add(alarm3);
                context.AlarmProcessItems.Add(alarmprocess1);
                context.AlarmProcessItems.Add(alarmprocess2);
                context.AlarmProcessItems.Add(alarmprocess3);
                context.SaveChanges();
            }


            AdhesiveFramework.End();
        }
    }
}
