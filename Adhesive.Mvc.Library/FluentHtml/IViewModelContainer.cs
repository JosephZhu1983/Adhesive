using System.Web.Mvc;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;

namespace Adhesive.Mvc.Library.FluentHtml
{
    public interface IViewModelContainer<T> : IViewDataContainer, IBehaviorsContainer where T : class
    {
        T ViewModel { get; }
        string HtmlNamePrefix { get; set; }
        HtmlHelper Html { get; }
    }
}