
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adhesive.AppInfoCenter;
using Adhesive.Common;
using Dimac.JMail;

namespace Adhesive.Alarm.Common
{
    public class MailService
    {
        private string hostName = "";
        private string username = "";
        private string password = "";

        public void Init(string hostName, string username, string password)
        {
            if (string.IsNullOrEmpty(hostName))
                throw new ArgumentException("不正确的hostName", "hostName");
            this.hostName = hostName;
            this.username = username;
            this.password = password;
        }

        public void Send(IList<MailItem> items)
        {
            items.Each(mailItem =>
            {
                Message message = new Message();
                message.BodyHtml = mailItem.MailBody;
                message.Subject = mailItem.MailTitle;
                message.Charset = Encoding.UTF8;
                message.From = new Address(username, "Adhesive框架邮件服务");
                message.To = new AddressList() { mailItem.MailAddress };
                try
                {
                    Smtp.Send(message, hostName, 25, "5173.com", SmtpAuthentication.Any, username, password);
                }
                catch (Exception ex)
                {
                    ex.Handle(AlarmConfigurationBase.ModuleName, "MailService", "Send");
                }
            });
        }

        public void Send(IList<object> items)
        {
            var mailItems = items.Cast<MailItem>().ToList();
            Send(mailItems);
        }
    }
}
