using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Adhesive.Mvc.Library.FluentHtml.Elements;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    public static class BehaviorHelperExtensions
    {
        /// <summary>
        /// Gets the attibute, if esists, of the specified type for the specified element.
        /// </summary>
        /// <typeparam name="T">The type of attribute to get.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns>The attribute or null if not exists.</returns>
        public static T GetAttribute<T>(this IMemberElement element) where T : Attribute
        {
            return element.ForMember.GetAttribute<T>();
        }

        /// <summary>
        /// Gets the attibute, if esists, of the specified type for the specified member expression.
        /// </summary>
        /// <typeparam name="T">The type of attribute to get.</typeparam>
        /// <param name="expression">The member expression.</param>
        /// <returns>The attribute or null if not exists.</returns>
        public static T GetAttribute<T>(this MemberExpression expression) where T : Attribute
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

        /// <summary>
        /// Determines whether this is an element that the user enters text into (e.g., input of type text or textarea)
        /// </summary>
        /// <param name="e">element</param>
        public static bool IsTextElement(this IElement e)
        {
            var tagName = e.Builder.TagName;
            var htmlTypeAttribute = e.GetAttr(HtmlAttribute.Type);
            return
                (tagName == HtmlTag.Input && htmlTypeAttribute == HtmlInputType.Text) ||
                tagName == HtmlTag.TextArea;
        }

        /// <summary>
        /// Gets the type of the the member if it's a property or a field.
        /// </summary>
        /// <param name="m">The member expression</param>
        /// <returns>The type or null if the member is not a property or field.</returns>
        public static Type GetPropertyOrFieldType(this MemberExpression m)
        {
            if (m.Member.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)m.Member).PropertyType;
            }
            if (m.Member.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)m.Member).FieldType;
            }
            return null;
        }

        /// <summary>
        /// Gets whether the type is a number.
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns>Whether it's a number.</returns>
        public static bool IsNumber(this Type t)
        {
            t = t.GetTypeWithoutNullability();
            return
                t == typeof(Int16) ||
                t == typeof(Int32) ||
                t == typeof(Int64) ||
                t == typeof(UInt16) ||
                t == typeof(UInt32) ||
                t == typeof(UInt64) ||
                t == typeof(decimal) ||
                t == typeof(float) ||
                t == typeof(double);
        }

        /// <summary>
        /// Gets the type with nullability removed if exists.
        /// </summary>
        /// <param name="t">The type</param>
        public static Type GetTypeWithoutNullability(this Type t)
        {
            return t.IsNullableType() ? new NullableConverter(t).UnderlyingType : t;
        }

        /// <summary>
        /// Gets whether the types is a nullable type
        /// </summary>
        /// <param name="t">The type</param>
        public static bool IsNullableType(this Type t)
        {
            return t.IsGenericType &&
                   t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}