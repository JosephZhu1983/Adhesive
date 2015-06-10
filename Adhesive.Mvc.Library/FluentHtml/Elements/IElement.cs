using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Adhesive.Mvc.Library.FluentHtml.Elements
{
    /// <summary>
    /// Interface for elements.
    /// </summary>
    public interface IElement : IHtmlString
    {
        /// <summary>
        /// TagBuilder object used to generate HTML.
        /// </summary>
        TagBuilder Builder { get; }

        /// <summary>
        /// How the tag should be closed.
        /// </summary>
        TagRenderMode TagRenderMode { get; }

        /// <summary>
        /// Set the value of the specified attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        void SetAttr(string name, object value);

        /// <summary>
        /// Set the value of the specified attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        string GetAttr(string name);

        /// <summary>
        /// Remove an attribute.
        /// </summary>
        /// <param name="name">The name of the attribute to remove.</param>
        void RemoveAttr(string name);

        /// <summary>
        /// The text for the label rendered before the element.
        /// </summary>
        string LabelBeforeText { get; set; }

        /// <summary>
        /// The text for the label rendered after the element.
        /// </summary>
        string LabelAfterText { get; set; }

        /// <summary>
        /// The class for labels rendered before or after the element.
        /// </summary>
        string LabelClass { get; set; }

        /// <summary>
        /// Additional JSON metadata that should be added to the element's CSS Class attribute.
        /// </summary>
        IDictionary<object, object> Metadata { get; }

        /// <summary>
        /// If no label before has been explicitly set, set the label before using the element name.
        /// </summary>
        void SetAutoLabel();

        /// <summary>
        /// If no label after has been explicitly set, set the label after using the element name.
        /// </summary>
        void SetAutoLabelAfter();

        /// <summary>
        /// Adds the value to the CSS class attribute.
        /// </summary>
        void AddClass(string value);
    }
}