using System;

using Adhesive.Config;
namespace Adhesive.Alarm.Common
{
    [ConfigEntity(FriendlyName = "报警服务接收者")]
    [Serializable]
    public class AlarmReceiverConfiguration
    {
        [ConfigItem(FriendlyName = "帐号名", Description = "对应Mongodb数据服务服务端配置中管理员的账号名")]
        public string Name { get; set; }
    }
}
