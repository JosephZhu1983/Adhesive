

namespace Adhesive.DistributedService.Imp
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using Adhesive.AppInfoCenter.Imp;

    internal class WcfLogProvider : BaseService
    {
        internal static readonly string ModuleName = "WCF服务模块";

        private static TAbstractLogInfo GetAbstractLogInfo<TAbstractLogInfo>(
            string requestIdentity,
            string extraInfo)
            where TAbstractLogInfo : AbstractInfo, new()
        {
            var abstractinfo = new TAbstractLogInfo();
            ProcessInfo(abstractinfo);
            abstractinfo.ContextIdentity = requestIdentity;
            return abstractinfo;
        }

        private static TMessageInfo GetMessageInfo<TMessageInfo>(
            string requestIdentity,
            string extraInfo,
            MessageDirection direction,
            string message)
            where TMessageInfo : WcfMessageInfo, new()
        {
            var log = GetAbstractLogInfo<TMessageInfo>(requestIdentity, extraInfo);
            log.Message = message;
            log.MessageDirection = direction;
            return log;
        }

        private static TStartInfo GetStartInfo<TStartInfo>(
            string extraInfo)
          where TStartInfo : StartInfo, new()
        {
            var log = GetAbstractLogInfo<TStartInfo>(string.Empty, extraInfo);
            return log;
        }

        private static TInvokeInfo GetInvokeInfo<TInvokeInfo>(
            string extraInfo,
            long executionTime,
            bool isSuccessuful,
            string methodName,
            ApplicationContext context)
            where TInvokeInfo : InvokeInfo, new()
        {
            var invokeinfo = GetAbstractLogInfo<TInvokeInfo>(context == null ? string.Empty : context.RequestIdentity, extraInfo);
            invokeinfo.ExecutionTime = executionTime;
            invokeinfo.IsSuccessuful = isSuccessuful;
            invokeinfo.MethodName = methodName;
            invokeinfo.ApplicationContext = context;
            return invokeinfo;
        }

        internal static ServerStartInfo GetServerStartInfo(
            string serviceName,
            string extraInfo,
            WcfServiceConfig service)
        {
            var log = GetStartInfo<ServerStartInfo>(extraInfo);
            log.WcfService = service;
            log.ServiceName = serviceName;
            return log;
        }

        internal static ClientStartInfo GetClientStartInfo(
            string contractName,
            string extraInfo,
            WcfClientEndpointConfig endpoint)
        {
            var log = GetStartInfo<ClientStartInfo>(extraInfo);
            log.ContractName = contractName;
            log.WcfEndpoint = endpoint;
            return log;
        }

        internal static ClientInvokeInfo GetClientInvokeLog(
            string contractName,
            string extraInfo,
            long executionTime,
            bool isSuccessuful,
            string methodName,
            ApplicationContext context)
        {
            var log = GetInvokeInfo<ClientInvokeInfo>(extraInfo, executionTime, isSuccessuful, methodName, context);
            log.ContractName = contractName;
            log.CategoryName = log.ContractName;
            log.SubCategoryName = log.MethodName;
            return log;
        }

        internal static ServerInvokeInfo GetServerInvokeInfo(
            string extraInfo,
            long executionTime,
            bool isSyncInvoke,
            bool isSuccessuful,
            string methodName,
            ApplicationContext context,
            List<string> parameters,
            List<string> results)
        {
            var log = GetInvokeInfo<ServerInvokeInfo>(extraInfo, executionTime, isSuccessuful, methodName, context);
            log.ServiceName = OperationContext.Current.GetCurrentServiceDescription().ServiceType.FullName;
            log.IsSyncInvoke = isSyncInvoke;
            log.Parameters = parameters;
            log.Results = results;
            log.CategoryName = log.ServiceName;
            log.SubCategoryName = log.MethodName;
            return log;
        }

        internal static ClientMessageInfo GetClientMessageInfo(
            string contractName,
            string requestIdentity,
            string extraInfo,
            MessageDirection direction,
            string message)
        {
            var log = GetMessageInfo<ClientMessageInfo>(requestIdentity, extraInfo, direction, message);
            log.ContractName = contractName;
            log.CategoryName = log.ContractName;
            return log;
        }

        internal static ServerMessageInfo GetServerMessageInfo(
            string extraInfo,
            MessageDirection direction,
            string message)
        {
            var log = GetMessageInfo<ServerMessageInfo>(ServerApplicationContext.Current.RequestIdentity, extraInfo, direction, message);
            log.ServiceName = OperationContext.Current.GetCurrentServiceDescription().ServiceType.FullName;
            log.CategoryName = log.ServiceName;
            return log;
        }
    }
}
