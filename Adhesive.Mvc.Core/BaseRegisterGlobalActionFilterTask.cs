

using Adhesive.Common;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace Adhesive.Mvc.Core
{
    public class BaseRegisterGlobalActionFilterTask : InitServiceBootstrapperTask
    {
        public BaseRegisterGlobalActionFilterTask(IUnityContainer container) : base(container) { }

        protected virtual void Register(GlobalFilterCollection filters)
        {

        }

        public override TaskContinuation Execute()
        {
            var filters = container.Resolve<GlobalFilterCollection>();
            filters.Add(new ExceptionHandlerActionFilter());
            filters.Add(new CrossCuttingActionFilter());
            Register(filters);
            return TaskContinuation.Continue;
        }
    }
}
