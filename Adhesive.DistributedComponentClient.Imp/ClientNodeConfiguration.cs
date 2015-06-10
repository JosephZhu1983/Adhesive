
using System;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient
{
    public class ClientNodeConfiguration
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public ClientNodeWeight Weight { get; set; }

        public int MaxConnections { get; set; }

        public int MinConnections { get; set; }

        public TimeSpanEx MaxIdleTime { get; set; }

        public TimeSpanEx MaxBusyTime { get; set; }

        public TimeSpanEx ConnectTimeout { get; set; }

        public TimeSpanEx SendTimeout { get; set; }

        public TimeSpanEx ReceiveTimeout { get; set; }

        public TimeSpanEx MaintenanceInterval { get; set; }

        public ClientNodeConfiguration()
        {
            MaxConnections = 50;
            MinConnections = 5;
            MaxIdleTime = TimeSpan.FromMinutes(1);
            MaxBusyTime = TimeSpan.FromMinutes(1);
            SendTimeout = TimeSpan.FromSeconds(5);
            ReceiveTimeout = TimeSpan.FromSeconds(5);
            ConnectTimeout = TimeSpan.FromSeconds(5);
            MaintenanceInterval = TimeSpan.FromSeconds(300);
            Weight = ClientNodeWeight.Medium;
        }
    }
}
