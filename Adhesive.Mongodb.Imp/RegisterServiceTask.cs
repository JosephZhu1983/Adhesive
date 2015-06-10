
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.Mongodb.Imp
{
    public class RegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }

        public override TaskContinuation Execute()
        {
            //AppInfoCenterConfigurationDefaultConfig.RegisterStateServiceConfigurationItem(new StateServiceConfigurationItem
            //{
            //    Enabled = true,
            //    TypeFullName = typeof(MongodbServiceStateInfo).FullName,
            //});

            container.RegisterTypeAsSingleton<IMongodbInsertService, MongodbInsertService>();
            container.RegisterTypeAsSingleton<IMongodbQueryService, MongodbQueryService>();

            return TaskContinuation.Continue;
        }

        protected override void InternalDispose()
        {
            container.Resolve<IMongodbInsertService>().Dispose();
            container.Resolve<IMongodbQueryService>().Dispose();
        }
    }
}
