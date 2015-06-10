using System;

using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;
using Adhesive.Config;

namespace Adhesive.Mongodb.Server.Imp
{
    public class MongodbServerConfiguration
    {
        internal static readonly string ModuleName = "Mongodb数据服务服务端模块";
        internal static readonly string MetaDataDbName = "Metadata";
        private static IConfigService configService = LocalServiceLocator.GetService<IConfigService>();

        public static MongodbServerConfigurationEntity GetConfig()
        {
            var defaultConfig = new MongodbServerConfigurationEntity
            {
                MaintainceIntervalMilliSeconds = 1000 * 60,
                EnableCache = false,
#if DEBUG
                MemcachedClusterName = "TestMemcachedCluster",
#else
                MemcachedClusterName = "AdhesiveMemcached",
#endif
                MongodbServerUrls = new Dictionary<string, MongodbServerUrl>
                {
#if DEBUG
                    { "Main", new MongodbServerUrl
                        {
                            Name = "Main",
                            Master = "mongodb://192.168.129.142:20000",
                            Slave = "mongodb://192.168.129.172:20000/?SlaveOK=true",
                            SyncDelay = TimeSpan.FromSeconds(10),
                        }
                    }
#else
                     { "Main", new MongodbServerUrl
                        {
                            Name = "Main",
                            Master = "mongodb://192.168.2.219:10000",
                            Slave = "mongodb://192.168.2.226:10000/?SlaveOK=true;socketTimeoutMS=600000;connectTimeoutMS=10000",
                            SyncDelay = TimeSpan.FromSeconds(10),
                        }
                    }
#endif
                },
                MongodbAdminConfigurationItems = new Dictionary<string, MongodbAdminConfigurationItem>
                {
                    {"Admin", new MongodbAdminConfigurationItem
                        {
                            UserName = "Admin",
                            Password = "",
                            RealName = "管理员",
                            MailAddress = "zhuye@5173.com",
                            MobileNumber = "13651657101",

                            MongodbAdminDatabaseConfigurationItems = new Dictionary<string,MongodbAdminDatabaseConfigurationItem>                            
                            {
                                {
                                    "*", new MongodbAdminDatabaseConfigurationItem
                                    {
                                        DatabasePrefix = "*",
                                        MongodbAdminTableConfigurationItems = new Dictionary<string,MongodbAdminTableConfigurationItem>
                                        {
                                            {
                                                "*", new MongodbAdminTableConfigurationItem
                                                {
                                                    TableName = "*",
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                },
                MongodbServerConfigurationItems = new List<MongodbServerConfigurationItem>()
                {
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.LogInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.AppDomainUnhandledExceptionInfo",  MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.HandledExceptionInfo",  MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WebSiteUnhandledExceptionInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WcfUnhandledClientExceptionInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WcfUnhandledServerExceptionInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.ApplicationStateInfo",  MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WebsitePageExecutionInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WebsiteRequestStateInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.PerformanceInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.Mongodb.Imp.MongodbServiceStateInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.Mongodb.Server.Imp.MongodbServerStateInfo", MongodbServerUrlName = "Main"},
                  
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ClientInvokeInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ClientMessageInfo",  MongodbServerUrlName = "Main"} ,
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ClientStartInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ServerInvokeInfo",  MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ServerMessageInfo", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ServerStartInfo",  MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.WcfServerStateInfo",  MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.DistributedService.WcfClientStateInfo", MongodbServerUrlName = "Main"},

                    new MongodbServerConfigurationItem() { TypeFullName = "Adhesive.Mongodb.Silverlight.Web.OperationLog", MongodbServerUrlName = "Main"},
                    new MongodbServerConfigurationItem() { TypeFullName = "Bk.Core.Imp.SearchOfferInfoJob.MqOffer", MongodbServerUrlName = "Main"},
                }.ToDictionary(s => s.TypeFullName),
            };

            var config = configService.GetConfigItemValue(true, "MongodbServerConfiguration", defaultConfig, update =>
                {
                    MongodbServer.ConfigUpdateCallbackForMaster();
                    MongodbServer.ConfigUpdateCallbackForSlave();
                });
            return config;
        }

        internal static MongodbServerConfigurationItem GetMongodbServerConfigurationItem(string typeFullName)
        {
            MongodbServerConfigurationItem item = GetConfig().MongodbServerConfigurationItems.Values.FirstOrDefault(c => c.TypeFullName == typeFullName);
            return item;
        }

        internal static MongodbServerUrl GetMongodbServerUrl(string name)
        {
            MongodbServerUrl item = GetConfig().MongodbServerUrls.Values.FirstOrDefault(c => c.Name == name);
            return item;
        }
    }
}
