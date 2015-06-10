

using System;
using System.Collections.Generic;
using System.Linq;
using Adhesive.AppInfoCenter;
using Adhesive.Common;
using Fost.MobileMessages;
using HTB.DevFx.Esb;

namespace Adhesive.Alarm.Common
{
    public class MobileService
    {
        private int categoryId;
        private static IMobileMessageService service = ServiceLocator.GetService<IMobileMessageService>();

        public void Init(int categoryId)
        {
            if (categoryId == 0)
                throw new ArgumentException("不正确的categoryId", "categoryId");
            this.categoryId = categoryId;
        }

        public void Send(IList<MobileItem> items)
        {
            items.Each(mobileItem =>
            {
                var result = service.SendSingleMessage(categoryId, mobileItem.MobileNumber, mobileItem.MobileMessage);
                if (result.ResultNo != 0)
                    AppInfoCenterService.LoggingService.Error(AlarmConfigurationBase.ModuleName, "MailService", "Send", 
                        string.Format("发送短信出错，发送结果代码：{0}", result.ToString()));
            });
        }

        public void Send(IList<object> items)
        {
            var mobileItems = items.Cast<MobileItem>().ToList();
            Send(mobileItems);
        }
    }
}
