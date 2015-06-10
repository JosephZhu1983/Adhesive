using System;
using System.Linq.Expressions;
using System.Reflection;
using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    [Obsolete("Use Member Element extensions instead.")]
    public class MemberBehaviorHelper<T> where T : Attribute
    {
        public T GetAttribute(IMemberElement element)
        {
            return GetAttribute(element.ForMember);
        }

        public T GetAttribute(MemberExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            var attributes = expression.Member.GetCustomAttributes(typeof(T), true);
            if (attributes == null || attributes.Length == 0)
            {
                return null;
            }
            return (T)attributes[0];
        }

        public MethodInfo GetMethod(IMemberElement element, string methodName)
        {
            return element.ForMember == null
                    ? null
                    : element.GetType().GetMethod(methodName);
        }

        public void InvokeMethod(MethodInfo method, IMemberElement element, params object[] parameters)
        {
            method.Invoke(element, parameters);
        }
    }
}