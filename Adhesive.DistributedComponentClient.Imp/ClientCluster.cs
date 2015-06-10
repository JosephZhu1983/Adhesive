
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Adhesive.Common;
namespace Adhesive.DistributedComponentClient
{
    internal class ClientCluster
    {
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly ClientNodeLocator nodeLocator = new ClientNodeLocator();
        private readonly Dictionary<string, ClientNode> clientNodes = new Dictionary<string, ClientNode>();
        private readonly Dictionary<string, ClientNode> deadClientNodes = new Dictionary<string, ClientNode>();
        private readonly ClientClusterConfiguration config;
        private readonly Thread tryRecoverNodeThread;

        private void NodeError(ClientNode node)
        {
            LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "NodeError", 
                string.Format("集群 {0} 中节点 {1} 出现错误，尝试从集群中移除节点", config.Name, node.Name));
            locker.EnterWriteLock();
            try
            {
                if (!deadClientNodes.ContainsKey(node.Name))
                {
                    deadClientNodes.Add(node.Name, node);
                }
                if (clientNodes.ContainsKey(node.Name))
                {
                    clientNodes.Remove(node.Name);
                }
                if (clientNodes.Count == 0)
                {
                    LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "NodeError", 
                        string.Format("严重错误，集群 {0} 中的所有节点都已经失效！", config.Name));
                }
                node.Destory();
                InitNodeLocator();
                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "NodeError", 
                    string.Format("集群 {0} 中节点 {1} 出现错误，已经从集群中移除", config.Name, node.Name));
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "NodeError", ex.Message);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private void TryRecoverNodeThreadAction()
        {
            while (true)
            {
                locker.EnterUpgradeableReadLock();
                try
                {
                    var deadClientNodesCopy = deadClientNodes.Select(i => i.Value).ToArray();
                    foreach (var node in deadClientNodesCopy)
                    {
                        LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "TryRecoverNodeThreadAction",
                            string.Format("集群 {0} 开始检查节点 {1} 是否已经恢复", config.Name, node.Name));
                        using (var socket = node.GetDirectClientSocket())
                        {
                            if (socket != null)
                            {
                                LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "TryRecoverNodeThreadAction", 
                                    string.Format("集群 {0} 开始恢复节点 {1}", config.Name, node.Name));
                                node.Recover();
                                locker.EnterWriteLock();
                                try
                                {
                                    if (deadClientNodes.ContainsKey(node.Name))
                                    {
                                        deadClientNodes.Remove(node.Name);
                                        LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "TryRecoverNodeThreadAction", 
                                            string.Format("集群 {0} 把节点 {1} 从死亡列表中去除", config.Name, node.Name));
                                    }
                                    if (!clientNodes.ContainsKey(node.Name))
                                    {
                                        clientNodes.Add(node.Name, node);
                                        LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "TryRecoverNodeThreadAction", 
                                            string.Format("集群 {0} 把节点 {1} 重新加入正常列表中", config.Name, node.Name));
                                    }
                                    InitNodeLocator();
                                }
                                finally
                                {
                                    locker.ExitWriteLock();
                                }
                                LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "TryRecoverNodeThreadAction", 
                                    string.Format("集群 {0} 节点 {1} 已经恢复", config.Name, node.Name));
                            }
                            else
                            {
                                LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "TryRecoverNodeThreadAction", 
                                    string.Format("集群 {0} 节点 {1} 并没有恢复", config.Name, node.Name));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "TryRecoverNodeThreadAction", ex.Message);
                }
                finally
                {
                    locker.ExitUpgradeableReadLock();
                }
                Thread.Sleep(Math.Max((int)((TimeSpan)config.TryRecoverNodeInterval).TotalMilliseconds, 1000));
            }

        }

        private void InitNodeLocator()
        {
            nodeLocator.Initialize(clientNodes.Select(item => item.Value).ToList());
            LocalLoggingService.Debug("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "InitNodeLocator", 
                string.Format("为集群 {0} 初始化一致性哈希表完成", config.Name));
        }

        internal ClientCluster(ClientClusterConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("传入的集群配置为空！");
            if (string.IsNullOrEmpty(config.Name))
                throw new ArgumentException("集群名为空！");

            this.config = config;

            tryRecoverNodeThread = new Thread(TryRecoverNodeThreadAction)
            {
                Name = string.Format("{0}_{1}", "Adhesive.DistributedComponentClient_TryRecoverNodeThread", config.Name),
                IsBackground = true,
            };
            tryRecoverNodeThread.Start();

            foreach (var nodeConfig in config.ClientNodeConfigurations.Select(item => item.Value).ToList())
            {
                if (clientNodes.ContainsKey(nodeConfig.Name))
                    throw new Exception(string.Format("在集群 {0} 中已经存在名为 {1} 的节点!", config.Name, nodeConfig.Name));
                var node = new ClientNode(nodeConfig, this.NodeError);
                locker.EnterWriteLock();
                try
                {
                    clientNodes.Add(nodeConfig.Name, node);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }

            InitNodeLocator();

        }

        internal ClientNode AcquireNode(string itemKey)
        {
            locker.EnterReadLock();
            try
            {
                var node = nodeLocator.LocateNode(itemKey);
                return node;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal List<ClientNode> AcquireNodes()
        {
            locker.EnterReadLock();
            try
            {
                var nodes = clientNodes.Select(item => item.Value).ToList();
                return nodes;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        internal ClientSocket AcquireSocket(string itemKey)
        {
            ClientNode node = AcquireNode(itemKey);
            if (node == null)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientCluster", "AcquireSocket", 
                    string.Format("在集群 {0} 中根据 Key {1} 没有获取到节点，集群现有节点数量 {2}", config.Name, itemKey, clientNodes.Count));
                return null;
            }
            if (!node.IsAlive)
            {
                NodeError(node);
            }
            //AppInfoCenterService.LoggingService.Debug("集群分配方案 Key {0} -> 节点 {1}", itemKey, node.Name);
            return node.GetClientSocket();
        }
    }
}
