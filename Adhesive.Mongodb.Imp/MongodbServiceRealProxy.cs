

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using Adhesive.Common;
using Adhesive.Mongodb.Server;

namespace Adhesive.Mongodb.Imp
{
    internal class MongodbServiceRealProxy : RealProxy
    {
        public MongodbServiceRealProxy() : base(typeof(IMongodbServer)) { }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodReturnMessage methodReturn = null;
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;
            var client = MongodbServiceClientFactory.CreateServiceClient();
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
    internal class MongodbServiceClientFactory
    {
        private static readonly Dictionary<string, ChannelFactory> _channelFactoryCache = new Dictionary<string, ChannelFactory>();
        private static readonly object _locker = new object();
        internal static ChannelFactory<IMongodbServer> CreateServiceClient()
        {
            string typeName = typeof(IMongodbServer).FullName;
            ChannelFactory cf;
            if (!_channelFactoryCache.TryGetValue(typeName, out cf))
            {
                lock (_locker)
                {
                    if (!_channelFactoryCache.TryGetValue(typeName, out cf))
                    {
                        var configServiceAddress = CommonConfiguration.GetConfig().MongodbServiceAddress;
                        var binding = new NetTcpBinding();
                        binding.Security.Mode = SecurityMode.None;
                        //binding.MaxReceivedMessageSize = 655360000;
                        binding.SendTimeout = TimeSpan.FromMinutes(1);//超时时间1分钟
                        var address = string.Format("net.tcp://{0}", configServiceAddress);
                        //LocalLoggingService.Info("Mongodb服务地址:" + address);
                        cf = new ChannelFactory<IMongodbServer>(binding, address);
                        if (cf != null)
                            _channelFactoryCache[typeName] = cf;
                    }
                }
            }
            return (ChannelFactory<IMongodbServer>)cf;
        }
    }
}
