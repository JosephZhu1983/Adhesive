
using System;
using Adhesive.Common;
using Adhesive.Config;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb
{
    [ConfigEntity(FriendlyName = "Mongodb后台管理员配置")]
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class MongodbAdminConfigurationItem
    {
        [DataMember]
        public string IP { get; set; }

        [ConfigItem(FriendlyName = "用户名")]
        [DataMember]
        public string UserName { get; set; }

        [ConfigItem(FriendlyName = "密码")]
        [DataMember]
        public string Password { get; set; }

        [ConfigItem(FriendlyName = "真实姓名")]
        [DataMember]
        public string RealName { get; set; }

        [ConfigItem(FriendlyName = "手机号码")]
        [DataMember]
        public string MobileNumber { get; set; }

        [ConfigItem(FriendlyName = "邮箱地址")]
        [DataMember]
        public string MailAddress { get; set; }

        [ConfigItem(FriendlyName = "管理的数据库")]
        [DataMember]
        public Dictionary<string, MongodbAdminDatabaseConfigurationItem> MongodbAdminDatabaseConfigurationItems { get; set; }
    }

    [ConfigEntity(FriendlyName = "Mongodb后台管理员配置-数据库")]
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class MongodbAdminDatabaseConfigurationItem
    {
        [ConfigItem(FriendlyName = "数据库前缀")]
        [DataMember]
        public string DatabasePrefix { get; set; }

        [ConfigItem(FriendlyName = "管理的表")]
        [DataMember]
        public Dictionary<string, MongodbAdminTableConfigurationItem> MongodbAdminTableConfigurationItems { get; set; }
    }

    [ConfigEntity(FriendlyName = "Mongodb后台管理员配置-表")]
    [DataContract(Namespace = "Adhesive.Mongodb")]
    public class MongodbAdminTableConfigurationItem
    {
        [ConfigItem(FriendlyName = "表名")]
        [DataMember]
        public string TableName { get; set; }
    }
}
