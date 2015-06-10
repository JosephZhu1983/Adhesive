

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Remoting.Messaging;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Web;
    using Adhesive.AppInfoCenter;
    using Adhesive.AppInfoCenter.Imp;
    using Adhesive.Common;
    using Adhesive.Mongodb;

    internal class ClientMessageInspector : IClientMessageInspector
    {
        private static Dictionary<string, string> contractVersionCache = new Dictionary<string, string>();
        private static object locker = new object();

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            ClientApplicationContext.Current = reply.GetApplicationContext<ClientApplicationContext>();

            try
            {
                var setting = WcfSettingManager.CurrentClientSetting((correlationState as Type));
                var logSetting = setting.WcfLogSetting;
                if (logSetting.Enabled && logSetting.MessageInfoSetting.Enabled)
                {
                    var direct = logSetting.MessageInfoSetting.MessageDirection;
                    if (direct == MessageDirection.Both || direct == MessageDirection.Receive)
                    {
                        var log = WcfLogProvider.GetClientMessageInfo(
                          (correlationState as Type).FullName,
                          ClientApplicationContext.Current.RequestIdentity,
                          "ClientMessageInspector.AfterReceiveReply",
                          MessageDirection.Receive,
                          reply.ToString());

                        MongodbService.MongodbInsertService.Insert(log);
                    }
                }

                var securitySetting = setting.WcfSecuritySetting;
                if (securitySetting.PasswordCheck.Enable)
                {
                    if (securitySetting.PasswordCheck.Direction == OperationDirection.Both || securitySetting.PasswordCheck.Direction == OperationDirection.Reply)
                    {
                        if (ClientApplicationContext.Current.Password != securitySetting.PasswordCheck.Password)
                            throw new WcfSecurityException(string.Format("PasswordCheck failed in reply for {0}!", (correlationState as Type).FullName));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "ClientMessageInspector", "AfterReceiveReply");
                if (ex is WcfSecurityException) throw;
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            try
            {
                var channelType = channel.GetType();

                var serverContext = new ServerApplicationContext();

                if (HttpContext.Current != null
                    && HttpContext.Current.Items != null
                    && HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextIdentityKey] != null)
                {
                    serverContext.RequestIdentity = HttpContext.Current.Items[AppInfoCenterConfiguration.Const.ContextIdentityKey].ToString();
                }
                else if (CallContext.GetData(AppInfoCenterConfiguration.Const.ContextIdentityKey) != null)
                {
                    serverContext.RequestIdentity = CallContext.GetData(AppInfoCenterConfiguration.Const.ContextIdentityKey).ToString();
                }
                else
                {
                    serverContext.RequestIdentity = Guid.NewGuid().ToString();
                }

                var clientContext = new ClientApplicationContext();
                clientContext.RequestIdentity = serverContext.RequestIdentity;
                ClientApplicationContext.Current = clientContext;

                var setting = WcfSettingManager.CurrentClientSetting(channelType);
                var securitySetting = setting.WcfSecuritySetting;
                if (securitySetting.PasswordCheck.Enable)
                {
                    if (securitySetting.PasswordCheck.Direction == OperationDirection.Both || securitySetting.PasswordCheck.Direction == OperationDirection.Request)
                    {
                        serverContext.Password = securitySetting.PasswordCheck.Password;
                    }
                }

                var logSetting = setting.WcfLogSetting;
                if (logSetting.Enabled && logSetting.MessageInfoSetting.Enabled)
                {
                    var direct = logSetting.MessageInfoSetting.MessageDirection;
                    if (direct == MessageDirection.Both || direct == MessageDirection.Send)
                    {
                        var log = WcfLogProvider.GetClientMessageInfo(
                        channelType.FullName,
                        ClientApplicationContext.Current.RequestIdentity,
                        "ClientMessageInspector.BeforeSendRequest",
                        MessageDirection.Send,
                        request.ToString());
                        MongodbService.MongodbInsertService.Insert(log);
                    }
                }

                serverContext.ClientMachineIP = CommonConfiguration.MachineIP;

                if (!contractVersionCache.ContainsKey(channelType.FullName))
                {
                    lock (locker)
                    {
                        if (!contractVersionCache.ContainsKey(channelType.FullName))
                        {
                            contractVersionCache.Add(channelType.FullName, channelType.Assembly.GetName().Version.ToString());
                        }
                    }
                }
                serverContext.ClientVersion = contractVersionCache[channelType.FullName];
                request.SetApplicationContext(serverContext);

                return channelType;
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "ClientMessageInspector" , "BeforeSendRequest");
            }
            return channel.GetType();
        }
    }
}
