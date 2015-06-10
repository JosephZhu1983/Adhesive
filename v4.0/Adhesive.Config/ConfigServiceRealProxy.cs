

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using Adhesive.Common;

namespace Adhesive.Config
{
    internal class ConfigServiceRealProxy : RealProxy
    {
        public ConfigServiceRealProxy() : base(typeof(IConfigServer)) { }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodReturnMessage methodReturn = null;
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;
            var client = ConfigServiceClientFactory.CreateServiceClient();
            var channel = client.CreateChannel();
            try
            {
                object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
                methodCall.Args.CopyTo(copiedArgs, 0);
                object returnValue = methodCall.MethodBase.Invoke(channel, copiedArgs);
                methodReturn = new ReturnMessage(returnValue,
                                                copiedArgs,
                                                copiedArgs.Length,
                                                methodCall.LogicalCallContext,
                                                methodCall);
            }
            catch (Exception ex)
            {
                var exception = ex;
                if (ex.InnerException != null)
                    exception = ex.InnerException;
                methodReturn = new ReturnMessage(exception, methodCall);
            }
            finally
            {
                var commObj = channel as ICommunicationObject;
                if (commObj != null)
                {
                    try
                    {
                        commObj.Close();
                    }
                    catch (CommunicationException)
                    {
                        commObj.Abort();
                    }
                    catch (TimeoutException)
                    {
                        commObj.Abort();
                    }
                    catch (Exception)
                    {
                        commObj.Abort();
                        throw;
                    }
                }
            }
            return methodReturn;
        }
    }
    internal class ConfigServiceClientFactory
    {
        private static readonly Dictionary<string, ChannelFactory> _channelFactoryCache = new Dictionary<string, ChannelFactory>();
        private static readonly object _locker = new object();
        internal static ChannelFactory<IConfigServer> CreateServiceClient()
        {
            string typeName = typeof(IConfigServer).FullName;
            ChannelFactory cf;
            if (!_channelFactoryCache.TryGetValue(typeName, out cf))
            {
                lock (_locker)
                {
                    if (!_channelFactoryCache.TryGetValue(typeName, out cf))
                    {
                        var configServiceAddress = CommonConfiguration.GetConfig().ConfigServiceAddress;
                        var binding = new NetTcpBinding();
                        binding.Security.Mode = SecurityMode.None;
                        binding.MaxReceivedMessageSize = 655360000;
                        binding.SendTimeout = TimeSpan.FromMinutes(10);
                        var address = string.Format("net.tcp://{0}", configServiceAddress);
                        cf = new ChannelFactory<IConfigServer>(binding, address);
                        if (cf != null)
                            _channelFactoryCache[typeName] = cf;
                    }
                }
            }
            return (ChannelFactory<IConfigServer>)cf;
        }
    }
}
