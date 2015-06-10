using System.Collections.Generic;
using System.Linq.Expressions;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// A list of checkboxes buttons.
    /// </summary>
    public class CheckBoxList : CheckBoxListBase<CheckBoxList>
    {
        public CheckBoxList(string name) : base(HtmlTag.Div, name) { }

        public CheckBoxList(string name, MemberExpression forMember, IEnumerable<IBehaviorMarker> behaviors)
            : base(HtmlTag.Div, name, forMember, behaviors) { }
    }
}