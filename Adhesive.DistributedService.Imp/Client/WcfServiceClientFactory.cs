

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Configuration;
    using System.Xml;
    using Adhesive.AppInfoCenter;
    using Adhesive.Common;
    using Adhesive.Config;
    using Adhesive.Mongodb;

    internal class WcfServiceClientFactory
    {
        private static readonly Dictionary<string, ChannelFactory> channelFactoryCache = new Dictionary<string, ChannelFactory>();
        private static readonly object cacheLocker = new object();
        //private static IStateService clientStateService;

        //static WcfServiceClientFactory()
        //{
        //    clientStateService = AppInfoCenterService.StateService;
        //    clientStateService.Init(new StateServiceConfiguration(typeof(WcfClientStateInfo).FullName, WcfClientStateService.GetState));
        //}

        private static ChannelFactory<T> CreateChannelFactory<T>(WcfClientEndpointConfig endpoint)
        {
            if (endpoint == null)
                throw new Exception("不能找到Wcf客户端配置,请检查配置数据库!");

            try
            {
                var binding = ConfigHelper.CreateBinding(endpoint.EndpointBindingType, endpoint.EndpointBindingXml);
                if (binding is NetNamedPipeBinding)
                    endpoint.EndpointAddress = "localhost";
                var address = ConfigHelper.CreateAddress(endpoint.EndpointProtocol, endpoint.EndpointAddress, endpoint.EndpointPort, endpoint.EndpointName);
                var factory = new ChannelFactory<T>(binding, address);
                factory.Endpoint.Behaviors.Add(new MessageInspectorEndpointBehavior());
                if (!string.IsNullOrEmpty(endpoint.EndpointBehaviorXml))
                    AddEndpointBehavior(factory, endpoint.EndpointBehaviorXml);
                return factory;
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfServiceClientFactory", "CreateChannelFactory");
                throw;
            }
        }

        private static void AddEndpointBehavior(ChannelFactory factory, string behaviorXml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(behaviorXml);
                var endpointBehaviorElement = new EndpointBehaviorElement();
                ConfigHelper.Deserialize(doc.OuterXml, endpointBehaviorElement);
                foreach (var item in endpointBehaviorElement)
                {
                    ConfigHelper.SetBehavior(factory.Endpoint.Behaviors, item);
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfServiceClientFactory", "AddEndpointBehavior");
                throw;
            }
        }

        private static WcfClientEndpointConfig GetWcfClientEndpointConfiguration(Type serviceContractType)
        {
            var version = serviceContractType.Assembly.GetName().Version;
            var versionstring = version.Major + "." + version.Minor;
            try
            {
                using (var scf = WcfServiceClientFactory.CreateServiceClient<IWcfConfigService>())
                {
                    return scf.Channel.GetWcfClientEndpoint(serviceContractType.FullName, versionstring, CommonConfiguration.MachineIP);
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfServiceClientFactory", "GetWcfClientEndpointConfiguration");
                throw;
            }
        }

        internal static WcfChannelWrapper<T> CreateServiceClient<T>() where T : class
        {
            try
            {
                string typeName = typeof(T).FullName;
                ChannelFactory cf;

                if (!channelFactoryCache.TryGetValue(typeName, out cf))
                {
                    lock (cacheLocker)
                    {
                        if (!channelFactoryCache.TryGetValue(typeName, out cf))
                        {
                            if (typeof(T) == typeof(IWcfConfigService))
                            {
                                var configServiceAddress = CommonConfiguration.GetConfig().WcfConfigServiceAddress;
                                var binding = new NetTcpBinding();
                                binding.Security.Mode = SecurityMode.None;
                                var address = string.Format("net.tcp://{0}", configServiceAddress);
                                cf = new ChannelFactory<IWcfConfigService>(binding, address);
                            }
                            else if (typeof(T) == typeof(IConfigServer))
                            {
                                var configServiceAddress = CommonConfiguration.GetConfig().ConfigServiceAddress;
                                var binding = new NetTcpBinding();
                                binding.Security.Mode = SecurityMode.None;
                                binding.MaxReceivedMessageSize = 6553600;
                                //var address = string.Format("net.tcp://{0}", configServiceAddress);
                                cf = new ChannelFactory<IConfigServer>(binding, configServiceAddress);
                            }
                            else
                            {
                                var endpoint = GetWcfClientEndpointConfiguration(typeof(T));
                                cf = CreateChannelFactory<T>(endpoint);

                                WcfSettingManager.Init<T>();

                            }

                            if (cf != null)
                                channelFactoryCache[typeName] = cf;
                        }
                    }
                }

                return new WcfChannelWrapper<T>((cf as ChannelFactory<T>).CreateChannel());
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfServiceClientFactory", "CreateServiceClient");
                throw;
            }
        }
    }
}
