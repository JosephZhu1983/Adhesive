using System;
using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    [Obsolete("Use IBehavior<IMemberElement> instead.")]
    public interface IMemberBehavior : IBehavior<IMemberElement> { }
}