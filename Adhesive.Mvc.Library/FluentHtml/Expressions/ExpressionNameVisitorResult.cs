using System.Linq.Expressions;

namespace Adhesive.Mvc.Library.FluentHtml.Expressions
{
    public class ExpressionNameVisitorResult
    {
        public Expression NextExpression { get; set; }
        public string Name { get; set; }
    }
}
