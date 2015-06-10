

using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.Persistence.Imp
{
    public class RegisterServiceTask : RegisterServiceBootstrapperTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }
        public override TaskContinuation Execute()
        {
            container.RegisterTypeAsSingleton<IIdentityGenerator, SequentialGuidGenerator>();
            container.RegisterTypeAsSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            container.RegisterTypeAsSingleton<IConnectionProvider, ConnectionProvider>();
            container.RegisterTypeAsSingleton<IDbContextFactory, DbContextFactory>();
            container.RegisterTypeAsSingleton<IRepositoryFactory, RepositoryFactory>();
            return TaskContinuation.Continue;
        }
    }
}
