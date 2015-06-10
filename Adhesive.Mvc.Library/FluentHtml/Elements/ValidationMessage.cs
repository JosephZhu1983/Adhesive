using System.Collections.Generic;
using System.Linq.Expressions;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// Generate a validation message (text inside a span element).
    /// </summary>
    public class ValidationMessage : LiteralBase<ValidationMessage>
    {
        public ValidationMessage(MemberExpression forMember, IEnumerable<IBehaviorMarker> behaviors)
            : base("", forMember, behaviors) { }

        public ValidationMessage() : base("") { }

        public override string ToHtmlString()
        {
            if (rawValue == null)
            {
                return null;
            }
            if (!builder.Attributes.ContainsKey("class"))
            {
                Class("field-validation-error");
            }
            return base.ToHtmlString();
        }
    }
}