
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Adhesive.Mvc.Core
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return (serviceType.IsClass && !serviceType.IsAbstract) ||
                container.IsRegistered(serviceType) ?
                container.Resolve(serviceType) : null;
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return (serviceType.IsClass && !serviceType.IsAbstract) ||
                container.IsRegistered(serviceType) ?
                container.ResolveAll(serviceType) : new object[] { };
        }
    }
}
