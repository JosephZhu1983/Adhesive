

namespace Adhesive.DistributedService.Imp
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [KnownType(typeof(ServerApplicationContext))]
    [KnownType(typeof(ClientApplicationContext))]
    public class ApplicationContext : Dictionary<string, string>
    {
        protected static readonly string CallContextKey = "ApplicationContext";
        internal static readonly string ContextHeaderLocalName = "__ApplicationContext";
        internal static readonly string ContextHeaderNamespace = "Adhesive.DistributedService";

        public new string this[string key]
        {
            get
            {
                if (!base.ContainsKey(key))
                    return null;
                return base[key];
            }
            set
            {
                if (!base.ContainsKey(key))
                    base[key] = value;
            }
        }

        public string RequestIdentity
        {
            get
            {
                return this["RequestIdentity"];
            }
            set
            {
                this["RequestIdentity"] = value;
            }
        }

        public string ServerExceptionID
        {
            get
            {
                return this["ServerExceptionID"];
            }
            set
            {
                this["ServerExceptionID"] = value;
            }
        }

        public string Password
        {
            get
            {
                return this["Password"];
            }
            set
            {
                this["Password"] = value;
            }
        }

    }
}
