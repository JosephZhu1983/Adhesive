

namespace Adhesive.DistributedService.Imp
{
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;

    [CollectionDataContract(Namespace = "Adhesive.DistributedService", ItemName = "Context")]
    public class ServerApplicationContext : ApplicationContext
    {
        public string ClientMachineIP
        {
            get
            {
                return base["ClientMachineIP"];
            }
            set
            {
                base["ClientMachineIP"] = value;
            }
        }

        public string ClientVersion
        {
            get
            {
                return base["ClientVersion"];
            }
            set
            {
                base["ClientVersion"] = value;
            }
        }

        public static ServerApplicationContext Current
        {
            get
            {
                return CallContext.GetData(CallContextKey) as ServerApplicationContext;
            }
            set
            {
                CallContext.SetData(CallContextKey, value);
            }
        }
    }
}
