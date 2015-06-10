using System;
using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    /// <summary>
    /// Generic implementation of behavior for an <see cref="IBehaviorMarker"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBehavior<T> : IBehaviorMarker
    {
        /// <summary>
        /// Perform behavior modification on an object.
        /// </summary>
        /// <param name="behavee">The object to modify based on the behavior.</param>
        void Execute(T behavee);
    }

    /// <summary>
    /// Behavior the specifies an execution order.
    /// </summary>
    public interface IOrderedBehavior : IBehaviorMarker
    {
        /// <summary>
        /// The order in which the behavior will execute with respect to other ordered behaviors.
        /// </summary>
        int Order { get; }
    }

    /// <summary>
    /// Generic implementation of behavior <see cref="IOrderedBehavior"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrderedBehavior<T> : IOrderedBehavior
    {
        /// <summary>
        /// Perform behavior modification on an object.
        /// </summary>
        /// <param name="behavee">The object to modify based on the behavior.</param>
        void Execute(T behavee);
    }

    [Obsolete("Use IBehavior<IElement> instead.")]
    public interface IBehavior : IBehavior<IElement> { }
}