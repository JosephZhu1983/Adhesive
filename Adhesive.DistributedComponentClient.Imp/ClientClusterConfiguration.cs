

using System;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient
{
    public class ClientClusterConfiguration
    {
        public string Name { get; set; }

        public TimeSpanEx TryRecoverNodeInterval { get; set; }

        public SerializableDictionary<string, ClientNodeConfiguration> ClientNodeConfigurations { get; set; }

        public ClientClusterConfiguration()
        {
            TryRecoverNodeInterval = TimeSpan.FromSeconds(10);
        }
    }
}
