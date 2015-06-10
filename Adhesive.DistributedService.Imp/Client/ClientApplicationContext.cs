

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;

    [CollectionDataContract(Namespace = "Adhesive.DistributedService", ItemName = "Context")]
    public class ClientApplicationContext : ApplicationContext
    {
        public string ServerMachineIP
        {
            get
            {
                return base["ServerMachineIP"];
            }
            set
            {
                base["ServerMachineIP"] = value;
            }
        }

        public string ServerVersion
        {
            get
            {
                return base["ServerVersion"];
            }
            set
            {
                base["ServerVersion"] = value;
            }
        }

        public static ClientApplicationContext Current
        {
            get
            {
                return CallContext.GetData(CallContextKey) as ClientApplicationContext;
            }
            set
            {
                CallContext.SetData(CallContextKey, value);
            }
        }
    }
}
