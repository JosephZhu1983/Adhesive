using Adhesive.Mvc.Library.FluentHtml.Elements;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    public abstract class ThreadSafeBehavior : IBehavior<IElement>
    {
        private static readonly object objLock = new object();

        public void Execute(IElement element)
        {
            lock (objLock)
            {
                DoExecute(element);
            }
        }

        protected abstract void DoExecute(IElement element);
    }
}