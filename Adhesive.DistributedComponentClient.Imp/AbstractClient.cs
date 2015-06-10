
using System;
using System.Collections.Generic;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient
{
    public abstract class AbstractClient<T> where T : AbstractClient<T>, new()
    {
        private readonly static Dictionary<string, ClientCluster> clusters = new Dictionary<string, ClientCluster>();

        public string ClusterName { get; private set; }

        public static T GetClient(string clusterName)
        {
            if (!clusters.ContainsKey(clusterName))
            {
                lock (clusters)
                {
                    if (!clusters.ContainsKey(clusterName))
                    {
                        var config = DistributedServerConfiguration.GetClientClusterConfiguration(clusterName);
                        if (config == null)
                            throw new Exception(string.Format("没有找到集群 {0} 的配置信息！", clusterName));
                        LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "AbstractClient", "GetClient", string.Format("开始初始化集群 {0}", clusterName));
                        var cluster = new ClientCluster(config);
                        clusters.Add(clusterName, cluster);
                    }
                }
            }

            var instance = new T();
            instance.ClusterName = clusterName;
            return instance;
        }

        internal ClientCluster GetCluster()
        {
            if (!clusters.ContainsKey(ClusterName))
                throw new Exception(string.Format("没有找到集群 {0}！", ClusterName));
            return clusters[ClusterName];
        }
    }
}
