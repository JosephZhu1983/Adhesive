using System.Collections.Generic;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// Base class for HTML input element of type '.'
    /// </summary>
    public abstract class ButtonBase<T> : Input<T> where T : ButtonBase<T>
    {
        protected ButtonBase(string text) : this(text, null) { }

        protected ButtonBase(string text, IEnumerable<IBehaviorMarker> behaviors)
            : base(HtmlInputType.Button, text == null ? null : text.FormatAsHtmlName(), null, behaviors)
        {
            elementValue = text;
        }
    }
}
