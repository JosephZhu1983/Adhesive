using System.Web.Mvc;
using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    /// <summary>
    /// Used internally by the ValidationBehavior to handle ModelState values for a particular element type.
    /// </summary>
    public interface IModelStateHandler
    {
        bool Handle(IElement element, ModelState state);
    }
}