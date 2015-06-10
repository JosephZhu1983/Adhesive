using System.Collections.Generic;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// Base class for HTML input element of type 'submit.'
    /// </summary>
    public abstract class SubmitButtonBase<T> : Input<T> where T : SubmitButtonBase<T>
    {
        protected SubmitButtonBase(string text) : this(text, null) { }

        protected SubmitButtonBase(string text, IEnumerable<IBehaviorMarker> behaviors)
            : base(HtmlInputType.Submit, text == null ? null : text.FormatAsHtmlName(), null, behaviors)
        {
            elementValue = text;
        }
    }
}
