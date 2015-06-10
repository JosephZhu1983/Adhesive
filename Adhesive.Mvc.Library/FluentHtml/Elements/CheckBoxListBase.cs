using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Adhesive.Mvc.Library.FluentHtml.Behaviors;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// Base class for a list of checkboxes.
    /// </summary>
    public abstract class CheckBoxListBase<T> : OptionsElementBase<T> where T : CheckBoxListBase<T>
    {
        protected string _itemFormat;
        protected string _itemClass;
        protected Action<CheckBox, object, int> _optionModifier;

        protected CheckBoxListBase(string tag, string name, MemberExpression forMember, IEnumerable<IBehaviorMarker> behaviors)
            : base(tag, name, forMember, behaviors) { }

        protected CheckBoxListBase(string tag, string name) : base(tag, name) { }

        /// <summary>
        /// Set the selected values.
        /// </summary>
        /// <param name="selectedValues">Values matching the values of options to be selected.</param>
        public virtual T Selected(IEnumerable selectedValues)
        {
            _selectedValues = selectedValues;
            return (T)this;
        }

        /// <summary>
        /// Specify a format string for the HTML of each checkbox button and label.
        /// </summary>
        /// <param name="value">A format string.</param>
        public virtual T ItemFormat(string value)
        {
            _itemFormat = value;
            return (T)this;
        }

        /// <summary>
        /// Specify the class for the input and label elements of each item.
        /// </summary>
        /// <param name="value">A format string.</param>
        public virtual T ItemClass(string value)
        {
            _itemClass = value;
            return (T)this;
        }

        /// <summary>
        /// An action performed after each CheckBox element has been created.  This is useful to
        /// modify the element before is rendered.
        /// </summary>
        /// <param name="action">The action to perform. The parameters to the action are the CheckBox element, 
        /// the option, and the position of the option.</param>
        public virtual T EachOption(Action<CheckBox, object, int> action)
        {
            _optionModifier = action;
            return (T)this;
        }

        protected override TagRenderMode TagRenderMode
        {
            get { return TagRenderMode.Normal; }
        }

        protected override string RenderOptions()
        {
            var name = builder.Attributes[HtmlAttribute.Name];
            builder.Attributes.Remove(HtmlAttribute.Name);
            var sb = new StringBuilder();
            var i = 0;
            foreach (var option in _options)
            {
                var value = _valueFieldSelector(option);
                var behaviorsToPassDown = behaviors == null
                    ? null :
                    behaviors.Where(x => (x is ValidationBehavior) == false);
                var checkbox = (new CheckBox(name, forMember, behaviorsToPassDown)
                    .Id(string.Format("{0}_{1}", name.FormatAsHtmlId(), i))
                    .Value(value))
                    .LabelAfter(_textFieldSelector(option).ToString(), _itemClass)
                    .Checked(IsSelectedValue(value));
                if (_itemClass != null)
                {
                    checkbox.Class(_itemClass);
                }
                if (_optionModifier != null)
                {
                    _optionModifier(checkbox, option, i);
                }
                sb.Append(_itemFormat == null
                    ? checkbox.ToCheckBoxOnlyHtml()
                    : string.Format(_itemFormat, checkbox.ToCheckBoxOnlyHtml()));
                i++;
            }
            return sb.ToString();
        }

        protected override void ApplyModelState(ModelState state)
        {
            Selected((string[])state.Value.ConvertTo(typeof(string[])));
        }
    }
}