

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Adhesive.Common;
    using Microsoft.Practices.Unity;

    internal class UnityInstanceProvider : IInstanceProvider
    {
        private static IUnityContainer Container { set; get; }
        public Type ServiceType { set; get; }

        static UnityInstanceProvider()
        {
            Container = AdhesiveFramework.Container;
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return Container.Resolve(ServiceType);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}
