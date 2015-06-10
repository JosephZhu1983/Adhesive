using System;
using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    /// <summary>
    /// Adds a CSS class to the element if the it has an attribute of the specified type or 
    /// if the model member is a non-nullable value type.
    /// </summary>
    public class RequiredCssClassBehavior : ThreadSafeMemberBehavior
    {
        private readonly string requiredClass;

        /// <summary>
        /// Constructor.  Sets "required" as the class to add.
        /// </summary>
        public RequiredCssClassBehavior() : this("required") { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requiredClass">The class to add for required elements.</param>
        public RequiredCssClassBehavior(string requiredClass)
        {
            this.requiredClass = requiredClass;
        }

        protected override void DoExecute(IMemberElement element)
        {
            if (IsRequired(element))
            {
                element.AddClass(requiredClass);
            }
        }

        protected virtual bool IsRequired(IMemberElement element)
        {
            return IsAutomaticallyRequired(element);
        }

        protected bool IsAutomaticallyRequired(IMemberElement element)
        {
            if (element.ForMember == null || !element.IsTextElement())
            {
                return false;
            }
            var memberType = element.ForMember.GetPropertyOrFieldType();
            if (memberType == null)
            {
                return false;
            }
            return memberType.IsValueType && !memberType.IsNullableType();
        }
    }

    /// <summary>
    /// Adds a CSS class to the element if the it has an attribute of the specified type or 
    /// if the model member is a non-nullable value type.
    /// </summary>
    /// <typeparam name="TRequiredAttribute"></typeparam>
    public class RequiredCssClassBehavior<TRequiredAttribute> : RequiredCssClassBehavior where TRequiredAttribute : Attribute
    {
        protected override bool IsRequired(IMemberElement element)
        {
            return base.IsRequired(element) || element.GetAttribute<TRequiredAttribute>() != null;
        }
    }
}