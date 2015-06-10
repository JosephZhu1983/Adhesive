
using System.Runtime.Serialization;
using Adhesive.Config;
using System;
namespace Adhesive.Mongodb
{
    [DataContract(Namespace = "Adhesive.Mongodb")]
    [ConfigEntity(FriendlyName = "服务器地址")]
    public class MongodbServerUrl
    {
        [DataMember]
        [ConfigItem(FriendlyName = "名字")]
        public string Name { get; set; }

        [DataMember]
        [ConfigItem(FriendlyName = "写服务器")]
        public string Master { get; set; }

        [DataMember]
        [ConfigItem(FriendlyName = "读服务器")]
        public string Slave { get; set; }

        [DataMember]
        [ConfigItem(FriendlyName = "同步延迟")]
        public TimeSpan SyncDelay { get; set; }

        public MongodbServerUrl()
        {
            SyncDelay = TimeSpan.Zero;
        }
    }
}
