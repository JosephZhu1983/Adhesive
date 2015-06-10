
namespace Adhesive.Common
{
    public abstract class AbstractService : Disposable
    {
        public bool Enabled { get; protected set; }
        public virtual string ServiceName { get; protected set; }

        public AbstractService()
        {
            Enabled = true;
            ServiceName = this.GetType().Name;
        }

        protected override void InternalDispose()
        {
            Enabled = false;
        }
    }
}
