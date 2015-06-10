using Adhesive.Common;
using Adhesive.Mvc.Core;
using Microsoft.Practices.Unity;

namespace Adhesive.Test.Mvc
{
    public interface ISuckService
    {
        string Suck();
    }

    public class SuckService : ISuckService
    {
        public string Suck()
        {
            return "Suck!";
        }
    }

    public class RegisterServiceTask : BaseRegisterServiceTask
    {
        public RegisterServiceTask(IUnityContainer container) : base(container) { }

        protected override void Register(IUnityContainer container)
        {
            container.RegisterTypeAsSingleton<ISuckService, SuckService>();
        }
    }
}