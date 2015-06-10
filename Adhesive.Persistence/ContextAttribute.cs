
using System;

namespace Adhesive.Persistence
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ContextAttribute : Attribute
    {
        public string ContextName { get; set; }
        public ContextAttribute(string contextName)
        {
            ContextName = contextName;
        }

    }
}
