
using Adhesive.Common;
using Microsoft.Practices.Unity;
using System.ServiceModel;
using System;

namespace Adhesive.Mongodb.Server.Imp
{
    public class RegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }

    
        public override TaskContinuation Execute()
        {
            //AppInfoCenterConfigurationDefaultConfig.RegisterStateServiceConfigurationItem(new StateServiceConfigurationItem
            //{
            //    Enabled = true,
            //    TypeFullName = typeof(MongodbServerStateInfo).FullName,
            //});

            container.RegisterTypeAsSingleton<IMongodbServer, MongodbServer>();

         
            return TaskContinuation.Continue;
        }

        protected override void InternalDispose()
        {
            container.Resolve<IMongodbServer>().Dispose();
         
        }
    }
}
