using System.Collections.Generic;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    /// <summary>
    /// Contract for any class implementing a list of custom behaviors.
    /// </summary>
    public interface IBehaviorsContainer
    {
        /// <summary>
        /// The collection of <see cref="IBehaviorMarker"/> objects.
        /// </summary>
        IEnumerable<IBehaviorMarker> Behaviors { get; }
    }
}