using System.Collections.Generic;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// Base class for HTML input element of type 'Reset.'
    /// </summary>
    public abstract class ResetButtonBase<T> : Input<T> where T : ResetButtonBase<T>
    {
        protected ResetButtonBase(string text) : this(text, null) { }

        protected ResetButtonBase(string text, IEnumerable<IBehaviorMarker> behaviors)
            : base(HtmlInputType.Reset, text == null ? null : text.FormatAsHtmlName(), null, behaviors)
        {
            elementValue = text;
        }
    }
}
