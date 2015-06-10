

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Adhesive.AppInfoCenter;
    using Adhesive.Common;
    using Adhesive.Mongodb;

    internal class ServerMessageInspector : IDispatchMessageInspector
    {
        private static Dictionary<string, string> serviceVersionCache = new Dictionary<string, string>();
        private static object locker = new object();

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            ServerApplicationContext.Current = request.GetApplicationContext<ServerApplicationContext>();

            try
            {
                var setting = WcfSettingManager.CurrentServerSetting(OperationContext.Current.GetCurrentServiceDescription().ServiceType);
                var logSetting = setting.WcfLogSetting;
                if (logSetting.Enabled && logSetting.MessageInfoSetting.Enabled)
                {
                    var direct = logSetting.MessageInfoSetting.MessageDirection;
                    if (direct == MessageDirection.Both || direct == MessageDirection.Receive)
                    {
                        var log = WcfLogProvider.GetServerMessageInfo(
                            "ServerMessageInspector.AfterReceiveRequest",
                            MessageDirection.Receive,
                            request.ToString());
                        MongodbService.MongodbInsertService.Insert(log);
                    }
                }

                var securitySetting = setting.WcfSecuritySetting;
                if (securitySetting.PasswordCheck.Enable)
                {
                    if (securitySetting.PasswordCheck.Direction == OperationDirection.Both || securitySetting.PasswordCheck.Direction == OperationDirection.Request)
                    {
                        if (ServerApplicationContext.Current.Password != securitySetting.PasswordCheck.Password)
                            throw new WcfSecurityException(string.Format("PasswordCheck failed in request for {0}!", OperationContext.Current.GetCurrentServiceDescription().ServiceType.FullName));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "ServerMessageInspector", "AfterReceiveRequest");
            }
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            try
            {
                var clientContext = new ClientApplicationContext();
                clientContext.ServerMachineIP = CommonConfiguration.MachineIP;
                clientContext.RequestIdentity = ServerApplicationContext.Current.RequestIdentity;
                clientContext.ServerExceptionID = ServerApplicationContext.Current.ServerExceptionID ?? "";

                var serviceType = OperationContext.Current.GetCurrentServiceDescription().ServiceType;
                if (!serviceVersionCache.ContainsKey(serviceType.FullName))
                {
                    lock (locker)
                    {
                        if (!serviceVersionCache.ContainsKey(serviceType.FullName))
                        {
                            serviceVersionCache.Add(serviceType.FullName, serviceType.Assembly.GetName().Version.ToString());
                        }
                    }
                }
                clientContext.ServerVersion = serviceVersionCache[serviceType.FullName];

                reply.SetApplicationContext(clientContext);

                var setting = WcfSettingManager.CurrentServerSetting(OperationContext.Current.GetCurrentServiceDescription().ServiceType);

                var securitySetting = setting.WcfSecuritySetting;
                if (securitySetting.PasswordCheck.Enable)
                {
                    if (securitySetting.PasswordCheck.Direction == OperationDirection.Both || securitySetting.PasswordCheck.Direction == OperationDirection.Reply)
                    {
                        clientContext.Password = securitySetting.PasswordCheck.Password;
                    }
                }

                var logSetting = setting.WcfLogSetting;
                if (logSetting.Enabled && logSetting.MessageInfoSetting.Enabled)
                {
                    var direct = logSetting.MessageInfoSetting.MessageDirection;
                    if (direct == MessageDirection.Both ||
                        direct == MessageDirection.Send)
                    {
                        var log = WcfLogProvider.GetServerMessageInfo(
                            "ServerMessageInspector.BeforeSendReply",
                            MessageDirection.Send,
                            reply.ToString());
                        MongodbService.MongodbInsertService.Insert(log);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "ServerMessageInspector", "BeforeSendReply");
            }
        }
    }
}
