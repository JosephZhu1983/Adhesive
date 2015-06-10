using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// Base class for form elements.
    /// </summary>
    /// <typeparam name="T">Derived type</typeparam>
    public abstract class FormElement<T> : DisableableElement<T>, ISupportsModelState where T : FormElement<T>, IElement
    {
        protected FormElement(string tag, string name, MemberExpression forMember, IEnumerable<IBehaviorMarker> behaviors)
            : base(tag, forMember, behaviors)
        {
            SetName(name);
        }

        protected FormElement(string tag, string name)
            : base(tag)
        {
            SetName(name);
        }

        public override string ToHtmlString()
        {
            InferIdFromName();
            return base.ToHtmlString();
        }

        /// <summary>
        /// Determines how the HTML element is closed.
        /// </summary>
        protected override TagRenderMode TagRenderMode
        {
            get { return TagRenderMode.SelfClosing; }
        }

        protected virtual void InferIdFromName()
        {
            if (!builder.Attributes.ContainsKey(HtmlAttribute.Id))
            {
                Attr(HtmlAttribute.Id, builder.Attributes[HtmlAttribute.Name].FormatAsHtmlId());
            }
        }

        protected void SetName(string name)
        {
            ((IElement)this).SetAttr(HtmlAttribute.Name, name);
        }

        void ISupportsModelState.ApplyModelState(ModelState state)
        {
            ApplyModelState(state);
        }

        protected abstract void ApplyModelState(ModelState state);
    }
}
