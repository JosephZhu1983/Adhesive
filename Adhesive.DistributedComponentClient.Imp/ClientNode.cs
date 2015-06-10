
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient
{
    internal class ClientNode
    {
        private readonly List<ClientSocket> idleSockets = new List<ClientSocket>();
        private readonly List<ClientSocket> busySockets = new List<ClientSocket>();
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly Thread maintainThread;
        private readonly ClientNodeConfiguration config;
        private Action<ClientNode> nodeErrorCallbackAction;

        internal bool IsAlive { get; private set; }

        internal ClientNodeWeight Weight
        {
            get
            {
                return config.Weight;
            }
        }

        internal string Name
        {
            get
            {
                return config.Name;
            }
        }

        internal string Address
        {
            get
            {
                return config.Address;
            }
        }

        internal void Recover()
        {
            Init();
            IsAlive = true;
            LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "Recover", 
                string.Format("节点 {0} 成功恢复", Name));
        }

        private void MaintainThreadAction()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(Convert.ToInt32(((TimeSpan)config.MaintenanceInterval).TotalMilliseconds));
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error("{0} {1}", config.MaintenanceInterval, ex.Message);
                    Thread.Sleep(1000 * 60);
                }
                if (IsAlive)
                {
                    locker.EnterUpgradeableReadLock();

                    try
                    {
                        //AppInfoCenterService.LoggingService.Debug(DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThreadAction", 
                        //    string.Format("节点 {0} 中有 {1} 个忙碌socket，{2} 个空闲socket，一共 {3} 个", Name, busySockets.Count, idleSockets.Count, idleSockets.Count + busySockets.Count));

                        var maxIdleTimeoutSocketsRemoveCount = Math.Max(0, busySockets.Count + idleSockets.Count - config.MinConnections);
                        var idleTimeoutSockets = idleSockets.Where(s => s.IdleTime != DateTime.MaxValue && s.IdleTime + config.MaxIdleTime < DateTime.Now)
                            .OrderBy(s => s.IdleTime)
                            .Take(maxIdleTimeoutSocketsRemoveCount).ToList();

                        if (idleTimeoutSockets.Count > 0)
                        {
                            LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThreadAction", 
                                string.Format("准备从节点 {0} 中移除 {1} 个空闲超时的socket", Name, idleTimeoutSockets.Count));
                            locker.EnterWriteLock();

                            try
                            {
                                foreach (var socket in idleTimeoutSockets)
                                {
                                    idleSockets.Remove(socket);
                                    socket.Destroy();
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThreadAction", ex.Message);
                            }
                            finally
                            {
                                locker.ExitWriteLock();
                            }
                        }
                        var busyTimeoutSockets = busySockets.Where(s => s.BusyTime != DateTime.MaxValue && s.BusyTime + config.MaxBusyTime < DateTime.Now).ToList();
                        if (busyTimeoutSockets.Count > 0)
                        {
                            LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThreadAction", 
                                string.Format("准备从节点 {0} 中移除 {1} 个忙碌超时的socket", Name, busyTimeoutSockets.Count));

                            locker.EnterWriteLock();
                            try
                            {
                                foreach (var socket in busyTimeoutSockets)
                                {
                                    busySockets.Remove(socket);
                                    socket.Destroy();
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThread", ex.Message);
                            }
                            finally
                            {
                                locker.ExitWriteLock();
                            }
                        }

                        var needAddSocketsCount = Math.Max(0, config.MinConnections - idleSockets.Count - busySockets.Count);
                        if (needAddSocketsCount > 0)
                        {
                            LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThreadAction", 
                                string.Format("准备从节点 {0} 中添加 {1} 个新的socket", Name, needAddSocketsCount));
                            for (int i = 0; i < needAddSocketsCount; i++)
                            {
                                if (IsAlive)
                                {
                                    var socket = CreateClientSocket(false, ReturnClientSocket, CloseClientSocket, CloseClientSocketAndNode);
                                    if (socket != null)
                                    {
                                        locker.EnterWriteLock();
                                        try
                                        {
                                            idleSockets.Add(socket);
                                        }
                                        finally
                                        {
                                            locker.ExitWriteLock();
                                        }
                                    }
                                    else
                                        LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThreadAction", 
                                            string.Format("节点 {0} 中MaintainThreadAction尝试创建一个新的socket失败", Name));
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "MaintainThread", ex.Message);
                    }
                    finally
                    {
                        locker.ExitUpgradeableReadLock();
                    }
                }
            }
        }

        private void Init()
        {
            locker.EnterWriteLock();

            idleSockets.Clear();
            busySockets.Clear();

            locker.ExitWriteLock();

            for (int i = 0; i < config.MinConnections; i++)
            {
                var socket = CreateClientSocket(false, ReturnClientSocket, CloseClientSocket, CloseClientSocketAndNode);
                if (socket != null)
                {
                    locker.EnterWriteLock();
                    try
                    {
                        idleSockets.Add(socket);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
                else
                {
                    LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "Init", 
                        string.Format("节点 {0} 中Init尝试创建一个新的socket失败", config.Name));
                    break;
                }
            }
            LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "Init", 
                string.Format("节点 {0} 初始化 {1} 个socket", config.Name, idleSockets.Count));
        }

        private ClientSocket CreateClientSocket(bool isDirectSocket, Action<ClientSocket> disposeAction, Action<ClientSocket> lowlevelErrorAction, Action<ClientSocket> highLevelErrorAction)
        {
            var ip = config.Address.Split(':')[0];
            var port = Convert.ToInt16(config.Address.Split(':')[1]);
            var socket = new ClientSocket(disposeAction, lowlevelErrorAction, highLevelErrorAction,
                        Convert.ToInt16(((TimeSpan)config.SendTimeout).TotalMilliseconds),
                        Convert.ToInt16(((TimeSpan)config.ReceiveTimeout).TotalMilliseconds), ip, port);
            try
            {
                socket.Connect(Convert.ToInt16(((TimeSpan)config.ConnectTimeout).TotalMilliseconds));
                return socket;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "Init", ex, string.Format("节点 {0} 连接socket出错", config.Name), ex.Message);
                if (!isDirectSocket)
                    CloseClientSocketAndNode(socket);
                else
                    socket.Destroy();
            }

            return null;
        }

        private void CloseClientSocket(ClientSocket socket)
        {
            socket.Destroy();
            LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "CloseClientSocket", 
                string.Format("关闭节点 {0} 中的一个socket", config.Name));
            locker.EnterWriteLock();
            try
            {
                busySockets.Remove(socket);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        private void CloseClientSocketAndNode(ClientSocket socket)
        {
            LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "CloseClientSocketAndNode", 
                string.Format("节点 {0} 已经无效", config.Name));
            this.IsAlive = false;
            socket.Destroy();
            nodeErrorCallbackAction(this);
        }

        private void ReturnClientSocket(ClientSocket socket)
        {
            socket.Release();
            locker.EnterWriteLock();
            try
            {
                busySockets.Remove(socket);
                if (idleSockets.Count + busySockets.Count < config.MaxConnections)
                {
                    idleSockets.Add(socket);
                }
                else
                {
                    LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "ReturnClientSocket", 
                        string.Format("节点 {0} 的socket池已达到最大限制 {1}，socket无须放回池", config.Name, config.MaxConnections));
                    socket.Destroy();
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        internal ClientNode(ClientNodeConfiguration config, Action<ClientNode> errorAction)
        {
            if (string.IsNullOrEmpty(config.Address))
                throw new ArgumentNullException(string.Format("{0}.Address"), config.Name);
            if (config.Address.Split(':').Length != 2)
                throw new ArgumentException(string.Format("{0}.Address格式必须为IP:Port", config.Name), "Address");
            var port = 0;
            if (!int.TryParse(config.Address.Split(':')[1], out port))
                throw new ArgumentException(string.Format("{0}.Address的端口号不是数字", config.Name), "Address");
            if (config.MaxConnections < config.MinConnections)
                throw new ArgumentException(string.Format("{0}.MaxConnections不能小于{0}.MinConnections", config.Name), "MaxConnections");

            if (config.ConnectTimeout < TimeSpan.FromMilliseconds(100))
                config.ConnectTimeout = TimeSpan.FromMilliseconds(100);
            if (config.ReceiveTimeout < TimeSpan.FromMilliseconds(100))
                config.ReceiveTimeout = TimeSpan.FromMilliseconds(100);
            if (config.SendTimeout < TimeSpan.FromMilliseconds(100))
                config.SendTimeout = TimeSpan.FromMilliseconds(100);

            if (config.MaxIdleTime < TimeSpan.FromSeconds(1))
                config.MaxIdleTime = TimeSpan.FromSeconds(1);
            if (config.MaxBusyTime < TimeSpan.FromSeconds(1))
                config.MaxBusyTime = TimeSpan.FromSeconds(1);
            if (config.MaintenanceInterval < TimeSpan.FromSeconds(1))
                config.MaintenanceInterval = TimeSpan.FromSeconds(1);

            this.config = config;
            this.nodeErrorCallbackAction = errorAction;
            LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "ClientNode", 
                string.Format("开始初始化一个新的节点：{0}", Name));
            Init();
            maintainThread = new Thread(MaintainThreadAction)
            {
                IsBackground = true,
                Name = string.Format("{0}_{1}", "Adhesive.DistributedComponentClient_NodeMaintainThread", config.Name),
            };
            maintainThread.Start();
            IsAlive = true;
        }

        internal ClientSocket GetDirectClientSocket()
        {
            LocalLoggingService.Debug("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "GetDirectClientSocket", 
                string.Format("节点 {0} 获取一个短连接", config.Name));
            return CreateClientSocket(true, socket =>
            {
                socket.Destroy();
            }, socket =>
            {
                socket.Destroy();
            }, socket =>
            {
                socket.Destroy();
            });
        }

        internal ClientSocket GetClientSocket()
        {
            if (!this.IsAlive)
            {
                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "GetClientSocket", 
                    string.Format("节点 {0} 已经死亡", config.Name));
                return null;
            }

            locker.EnterUpgradeableReadLock();
            ClientSocket socket = null;
            try
            {
                socket = idleSockets.FirstOrDefault();
                if (socket != null)
                {
                    socket.Acquire();
                    locker.EnterWriteLock();
                    try
                    {
                        idleSockets.Remove(socket);
                        busySockets.Add(socket);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
                else
                {
                    if (busySockets.Count < config.MaxConnections)
                    {
                        LocalLoggingService.Debug("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "GetClientSocket", 
                            string.Format("节点 {0} 中没有空闲的socket，新创建一个socket并加入池", config.Name));
                        socket = CreateClientSocket(false, ReturnClientSocket, CloseClientSocket, CloseClientSocketAndNode);
                        if (socket != null)
                        {
                            socket.Acquire();
                            locker.EnterWriteLock();
                            try
                            {
                                busySockets.Add(socket);
                            }
                            finally
                            {
                                locker.ExitWriteLock();
                            }
                        }
                        else
                        {
                            LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "GetClientSocket", 
                                string.Format("节点 {0} 中GetClientSocket尝试创建一个新的socket失败", config.Name));
                        }
                    }
                    else
                    {
                        LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "GetClientSocket", 
                            string.Format("节点 {0} 的socket池已满，无法获得socket ", config.Name));
                    }
                }
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
            if (socket == null)
                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "GetClientSocket", 
                    string.Format("节点 {0} 没有获取到socket ", config.Name));
            return socket;
        }

        internal void Destory()
        {
            foreach (var socket in idleSockets)
            {
                socket.Destroy();
            }
            foreach (var socket in busySockets)
            {
                socket.Destroy();
            }
            idleSockets.Clear();
            busySockets.Clear();
            LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientNode", "Destory", 
                string.Format("节点 {0} 已经清理完毕", config.Name));
        }
    }
}
