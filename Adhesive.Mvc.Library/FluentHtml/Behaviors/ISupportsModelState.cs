using System.Web.Mvc;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    public interface ISupportsModelState
    {
        void ApplyModelState(ModelState state);
    }
}