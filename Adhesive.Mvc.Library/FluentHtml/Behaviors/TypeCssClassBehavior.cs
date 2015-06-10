using System;
using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    /// <summary>
    /// Adds CSS classes to the element based on the type of the member.  Specifically handles
    /// number and date elements.
    /// </summary>
    public class TypeCssClassBehavior : ThreadSafeMemberBehavior
    {
        private readonly string numberClass;
        private readonly string dateClass;

        /// <summary>
        /// Constructor.  Sets "number" as the class to add for number elements and "date" for date element.
        /// </summary>
        public TypeCssClassBehavior() : this("number", "date") { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numberClass">The class to use for number elements.</param>
        /// <param name="dateClass">The class to use for date elements.</param>
        public TypeCssClassBehavior(string numberClass, string dateClass)
        {
            this.numberClass = numberClass;
            this.dateClass = dateClass;
        }

        protected override void DoExecute(IMemberElement element)
        {
            if (element.ForMember == null)
            {
                return;
            }
            if (!element.IsTextElement())
            {
                return;
            }
            var memberType = element.ForMember.GetPropertyOrFieldType();
            if (memberType.IsNumber())
            {
                element.AddClass(numberClass);
            }
            else if (memberType == typeof(DateTime) || memberType == typeof(DateTime?))
            {
                element.AddClass(dateClass);
            }
        }
    }
}