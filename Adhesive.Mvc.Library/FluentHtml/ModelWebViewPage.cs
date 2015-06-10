using System.Collections.Generic;
using System.Web.Mvc;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;

namespace Adhesive.Mvc.Library.FluentHtml
{
    public abstract class ModelWebViewPage<T> : WebViewPage<T>, IViewModelContainer<T> where T : class
    {
        protected readonly List<IBehaviorMarker> behaviors = new List<IBehaviorMarker>();

        protected ModelWebViewPage()
        {
            behaviors.Add(new ValidationBehavior(() => ViewData.ModelState));
        }

        protected ModelWebViewPage(params IBehaviorMarker[] behaviors) : this(null, behaviors) { }

        protected ModelWebViewPage(string htmlNamePrefix, params IBehaviorMarker[] memberBehaviors)
            : this()
        {
            HtmlNamePrefix = htmlNamePrefix;
            if (memberBehaviors != null)
            {
                behaviors.AddRange(memberBehaviors);
            }
        }

        public string HtmlNamePrefix { get; set; }

        public T ViewModel
        {
            get { return ViewData.Model; }
        }

        public IEnumerable<IBehaviorMarker> Behaviors
        {
            get { return behaviors; }
        }

        HtmlHelper IViewModelContainer<T>.Html
        {
            get { return Html; }
        }
    }
}