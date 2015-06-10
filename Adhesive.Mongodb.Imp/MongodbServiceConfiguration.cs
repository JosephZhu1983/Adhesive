
using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;
using Adhesive.Config;

namespace Adhesive.Mongodb.Imp
{
    internal class MongodbServiceConfiguration
    {
        internal static readonly string ModuleName = "Mongodb数据服务客户端模块";
        private static IConfigService configService = LocalServiceLocator.GetService<IConfigService>();

        internal static MongodbServiceConfigurationEntity GetConfig()
        {
            var defaultConfig = new MongodbServiceConfigurationEntity
            {
                MongodbInsertServiceConfigurationItems = new List<MongodbInsertServiceConfigurationItem>
                {
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.LogInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.AppDomainUnhandledExceptionInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.HandledExceptionInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WebSiteUnhandledExceptionInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WcfUnhandledClientExceptionInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WcfUnhandledServerExceptionInfo"} ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.ApplicationStateInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WebsitePageExecutionInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.WebsiteRequestStateInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.AppInfoCenter.Imp.PerformanceInfo"} ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.Mongodb.Imp.MongodbServiceStateInfo" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.Mongodb.Server.Imp.MongodbServerStateInfo" } ,

                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ClientInvokeInfo"} ,
                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ClientMessageInfo" } ,
                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ClientStartInfo" } ,
                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ServerInvokeInfo" } ,
                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ServerMessageInfo" } ,
                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.ServerStartInfo"} ,
                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.WcfServerStateInfo"} ,
                    //new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.DistributedService.WcfClientStateInfo" } ,

                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Adhesive.Mongodb.Silverlight.Web.OperationLog" } ,
                    new MongodbInsertServiceConfigurationItem() { TypeFullName = "Bk.Core.Imp.SearchOfferInfoJob.MqOffer" } ,
                }.ToDictionary(s => s.TypeFullName),
            };

            var config = configService.GetConfigItemValue(true, "MongodbServiceConfiguration", defaultConfig, update => MongodbInsertService.ConfigUpdateCallback());
            return config;
        }

        internal static MongodbInsertServiceConfigurationItem GetMongodbInsertServiceConfigurationItem(string typeFullName)
        {
            MongodbInsertServiceConfigurationItem item = GetConfig().MongodbInsertServiceConfigurationItems.Values.FirstOrDefault(c => c.TypeFullName == typeFullName);
            return item;
        }
    }
}
