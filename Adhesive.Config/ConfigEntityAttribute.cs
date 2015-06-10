using System;

namespace Adhesive.Config
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConfigEntityAttribute : Attribute
    {
        public string FriendlyName { get; set; }
        public string Description { get; set; }


    }
}
