
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adhesive.Common;
using Adhesive.DistributedComponentClient.Memcached.Protocol;
using Adhesive.DistributedComponentClient.Memcached.Protocol.Request;
using Adhesive.DistributedComponentClient.Memcached.Protocol.Response;
using Adhesive.DistributedComponentClient.Utility;

namespace Adhesive.DistributedComponentClient.Memcached
{
    public partial class MemcachedClient : AbstractClient<MemcachedClient>
    {
        internal Dictionary<string, bool> InternalFlush(TimeSpan expire)
        {
            var result = new Dictionary<string, bool>();
            var nodes = GetCluster().AcquireNodes();
            foreach (var node in nodes)
            {
                using (var socket = node.GetClientSocket())
                {
                    if (socket != null)
                    {
                        var requestPackage = expire == TimeSpan.MaxValue ? new FlushRequestPackage() : new FlushRequestPackage(expire);
                        var requestData = requestPackage.GetBytes();
                        if (requestData != null)
                        {
                            socket.Write(requestData);
                            var responsePackage = ResponsePackageCreator.GetPackage(socket);
                            if (responsePackage != null)
                            {
                                if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                                {
                                    result.Add(node.Name, true);
                                    continue;
                                }
                                else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                                {
                                    LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalFlush",
                                        string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        string.Empty,
                                        responsePackage.ResponseStatus));
                                }
                            }
                            else
                            {
                                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalFlush",
                                    string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        string.Empty));
                            }
                        }
                    }
                }
                if (!result.ContainsKey(node.Name))
                    result.Add(node.Name, false);
            }
            return result;
        }

        internal Dictionary<string, string> InternalVersion()
        {
            var result = new Dictionary<string, string>();
            var nodes = GetCluster().AcquireNodes();
            foreach (var node in nodes)
            {
                using (var socket = node.GetClientSocket())
                {
                    if (socket != null)
                    {
                        var requestPackage = new VersionRequestPackage();
                        var requestData = requestPackage.GetBytes();
                        if (requestData != null)
                        {
                            socket.Write(requestData);
                            var responsePackage = ResponsePackageCreator.GetPackage(socket);
                            if (requestData == null) return null;
                            if (responsePackage != null)
                            {
                                if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                                {
                                    result.Add(node.Name, responsePackage.Value);
                                    continue;
                                }
                                else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                                {
                                    LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalVersion", 
                                        string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        string.Empty,
                                        responsePackage.ResponseStatus));
                                }
                            }
                            else
                            {
                                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalVersion", 
                                    string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        string.Empty));
                            }
                        }
                    }
                }
                if (!result.ContainsKey(node.Name))
                    result.Add(node.Name, string.Empty);
            }
            return result;
        }

        internal Dictionary<string, Dictionary<string, string>> InternalStat(StatType statType)
        {
            var stat = new Dictionary<string, Dictionary<string, string>>();
            var nodes = GetCluster().AcquireNodes();
            foreach (var node in nodes)
            {
                var statItem = new Dictionary<string, string>();
                using (var socket = node.GetClientSocket())
                {
                    if (socket != null)
                    {
                        AbstractRequestPackage requestPackage = null;
                        switch (statType)
                        {
                            case StatType.General:
                                requestPackage = new StatRequestPackage(StatTypeCode.General);
                                break;
                            case StatType.Item:
                                requestPackage = new StatRequestPackage(StatTypeCode.Item);
                                break;
                            case StatType.Setting:
                                requestPackage = new StatRequestPackage(StatTypeCode.Setting);
                                break;
                        }
                        var requestData = requestPackage.GetBytes();
                        if (requestData != null)
                        {
                            socket.Write(requestData);
                            while (true)
                            {
                                var responsePackage = ResponsePackageCreator.GetPackage(socket);
                                if (responsePackage != null)
                                {
                                    if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                                    {
                                        if (!string.IsNullOrEmpty(responsePackage.Key))
                                        {
                                            if (!statItem.ContainsKey(responsePackage.Key))
                                                statItem.Add(responsePackage.Key, responsePackage.Value);
                                        }
                                        else
                                            break;
                                    }
                                    else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                                    {
                                        LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalStat", 
                                            string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                            socket.Endpoint.ToString(),
                                            requestPackage.Opcode,
                                            responsePackage.Key,
                                            responsePackage.ResponseStatus));
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!stat.ContainsKey(node.Name))
                    stat.Add(node.Name, statItem);
            }
            return stat;
        }

        private ulong? InternalIncrement(string key, ulong? seed, TimeSpan expire, ulong amount, ulong version)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = null;
                    if (seed.HasValue)
                    {
                        if (expire == TimeSpan.MaxValue)
                            requestPackage = new IncrementRequestPackage(key, amount, seed.Value, version);
                        else
                            requestPackage = new IncrementRequestPackage(key, amount, seed.Value, expire, version);
                    }
                    else
                    {
                        requestPackage = new IncrementRequestPackage(key, amount, version);
                    }
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return responsePackage.ValueBytes.GetLittleEndianUInt64();
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalIncrement", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        key,
                                        responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalIncrement", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        key));
                        }
                    }
                }
            }
            return null;
        }

        private ulong? InternalDecrement(string key, ulong? seed, TimeSpan expire, ulong amount, ulong version)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = null;
                    if (seed.HasValue)
                    {
                        if (expire == TimeSpan.MaxValue)
                            requestPackage = new DecrementRequestPackage(key, amount, seed.Value, version);
                        else
                            requestPackage = new DecrementRequestPackage(key, amount, seed.Value, expire, version);
                    }
                    else
                    {
                        requestPackage = new DecrementRequestPackage(key, amount, version);
                    }
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return responsePackage.ValueBytes.GetLittleEndianUInt64();
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                     && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalDecrement", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        key,
                                        responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalDecrement", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        key));
                        }
                    }
                }
            }
            return null;
        }

        private string InternalGet(string key, out ulong version)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    var requestPackage = new GetRequestPackage(key);
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                version = responsePackage.Version;
                                return responsePackage.Value ?? string.Empty;
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalGet", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        key,
                                        responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalGet", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                       socket.Endpoint.ToString(),
                                       requestPackage.Opcode,
                                       key));
                        }
                    }
                }
            }
            version = 0;
            return null;
        }

        private bool InternalDelete(string key)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = requestPackage = new DeleteRequestPackage(key);
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return true;
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalDelete", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        key,
                                        responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalDelete", 
                               string.Format( "在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                       socket.Endpoint.ToString(),
                                       requestPackage.Opcode,
                                       key));
                        }
                    }
                }
            }
            return false;
        }

        private void InternalFastDelete(string key)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = new DeleteQRequestPackage(key);
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                    }
                }
            }
        }

        private Dictionary<string, string> InternalGetMultiple(IList<string> keys)
        {
            var nodeCache = new Dictionary<ClientNode, List<string>>();
            foreach (var key in keys)
            {
                var node = GetCluster().AcquireNode(key);
                if (!nodeCache.ContainsKey(node))
                    nodeCache.Add(node, new List<string> { key });
                else if (!nodeCache[node].Contains(key))
                    nodeCache[node].Add(key);
            }

            var data = new Dictionary<string, string>();
            Parallel.ForEach(nodeCache, node =>
            {
                using (var socket = node.Key.GetClientSocket())
                {
                    if (socket != null)
                    {
                        var count = node.Value.Count;
                        for (int i = 0; i < count; i++)
                        {
                            var requestPackage = new GetKRequestPackage(node.Value[i]);
                            var requestData = requestPackage.GetBytes();
                            if (requestData != null)
                            {
                                socket.Write(requestData);
                            }
                            else
                            {
                                count--;
                            }
                        }

                        for (int i = 0; i < count; i++)
                        {
                            var responsePackage = ResponsePackageCreator.GetPackage(socket);
                            if (responsePackage != null)
                            {
                                if (!string.IsNullOrEmpty(responsePackage.Key))
                                {
                                    if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                                    {
                                        data.Add(responsePackage.Key, responsePackage.Value);
                                    }
                                    else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                                    {
                                        LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalGetMultiple", 
                                            string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                        socket.Endpoint.ToString(),
                                        responsePackage.Opcode,
                                        responsePackage.Key,
                                        responsePackage.ResponseStatus));
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            });
            return data;
        }

        private bool InternalAppend(string key, string value)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = new AppendRequestPackage(key, value);
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return true;
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalAppend", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                            socket.Endpoint.ToString(),
                                            requestPackage.Opcode,
                                            key,
                                            responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalAppend", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                       socket.Endpoint.ToString(),
                                       requestPackage.Opcode,
                                       key));
                        }
                    }
                }
            }
            return false;
        }

        private bool InternalPrepend(string key, string value)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = new PrependRequestPackage(key, value);
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return true;
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalPrepend", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                            socket.Endpoint.ToString(),
                                            requestPackage.Opcode,
                                            key,
                                            responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalPrepend", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                        socket.Endpoint.ToString(),
                                        requestPackage.Opcode,
                                        key));
                        }
                    }
                }
            }
            return false;
        }

        private bool InternalSet(string key, string value, TimeSpan expire, ulong version)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = expire == TimeSpan.MaxValue ? new SetRequestPackage(key, value, version)
                            : new SetRequestPackage(key, value, expire, version);
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return true;
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalSet", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                            socket.Endpoint.ToString(),
                                            requestPackage.Opcode,
                                            key,
                                            responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalSet", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                       socket.Endpoint.ToString(),
                                       requestPackage.Opcode,
                                       key));
                        }
                    }
                }
            }
            return false;
        }

        private void InternalFastSet(string key, string value, TimeSpan expire, ulong version)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = expire == TimeSpan.MaxValue ? new SetQRequestPackage(key, value, version)
                            : new SetQRequestPackage(key, value, expire, version);
                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                    }
                }
            }
        }

        private bool InternalAdd(string key, string value, TimeSpan expire, ulong version)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = expire == TimeSpan.MaxValue ? new AddRequestPackage(key, value, version)
                            : new AddRequestPackage(key, value, expire, version);

                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return true;
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalAdd", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                            socket.Endpoint.ToString(),
                                            requestPackage.Opcode,
                                            key,
                                            responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalAdd", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                       socket.Endpoint.ToString(),
                                       requestPackage.Opcode,
                                       key));
                        }
                    }
                }
            }
            return false;
        }

        private bool InternalReplace(string key, string value, TimeSpan expire, ulong version)
        {
            using (var socket = GetCluster().AcquireSocket(key))
            {
                if (socket != null)
                {
                    AbstractRequestPackage requestPackage = expire == TimeSpan.MaxValue ? new ReplaceRequestPackage(key, value, version)
                            : new ReplaceRequestPackage(key, value, expire, version);

                    var requestData = requestPackage.GetBytes();
                    if (requestData != null)
                    {
                        socket.Write(requestData);
                        var responsePackage = ResponsePackageCreator.GetPackage(socket);
                        if (responsePackage != null)
                        {
                            if (responsePackage.ResponseStatus == ResponseStatus.NoError)
                            {
                                return true;
                            }
                            else if (responsePackage.ResponseStatus != ResponseStatus.KeyExists
                                    && responsePackage.ResponseStatus != ResponseStatus.KeyNotFound)
                            {
                                LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalReplace", 
                                    string.Format("在 {0} 上执行操作 {1} 得到了不正确的回复 Key : {2} -> {3}",
                                            socket.Endpoint.ToString(),
                                            requestPackage.Opcode,
                                            key,
                                            responsePackage.ResponseStatus));
                            }
                        }
                        else
                        {
                            LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "InternalReplace", 
                                string.Format("在 {0} 上执行操作 {1} 没有得到回复 Key : {2}",
                                       socket.Endpoint.ToString(),
                                       requestPackage.Opcode,
                                       key));
                        }
                    }
                }
            }
            return false;
        }
    }
}
