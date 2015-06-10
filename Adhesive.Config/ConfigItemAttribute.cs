
using System;


namespace Adhesive.Config
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ConfigItemAttribute : Attribute
    {
        public string FriendlyName { get; set; }
        public string Description { get; set; }


    }
}
