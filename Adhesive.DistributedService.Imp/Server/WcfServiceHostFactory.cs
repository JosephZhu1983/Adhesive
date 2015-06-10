

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.Xml;
    using Adhesive.AppInfoCenter;
    using Adhesive.Common;
    using Adhesive.Config;
    using Adhesive.Mongodb;

    public class WcfServiceHostFactory : ServiceHostFactory
    {
        private static Dictionary<string, WcfServerPerformanceService> performanceServices = new Dictionary<string, WcfServerPerformanceService>();

        internal static WcfServerPerformanceService GetWcfServerPerformanceService(string name)
        {
            if (performanceServices.ContainsKey(name)) return performanceServices[name];
            return null;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return CreateServiceHost(serviceType);
        }

        public static ServiceHost CreateServiceHost<T>()
        {
            return CreateServiceHost(typeof(T));
        }

        public static ServiceHost CreateServiceHost(Type serviceType)
        {
            var serviceHost = new ServiceHost(serviceType);
            if (!typeof(IWcfConfigService).IsAssignableFrom(serviceType) && !typeof(IConfigServer).IsAssignableFrom(serviceType))
            {
                WcfSettingManager.Init(serviceType);

                if (serviceHost.Description.Behaviors.Find<ServiceErrorBehavior>() == null)
                    serviceHost.Description.Behaviors.Add(new ServiceErrorBehavior());
                if (serviceHost.Description.Behaviors.Find<ActionInterceptBehavior>() == null)
                    serviceHost.Description.Behaviors.Add(new ActionInterceptBehavior());

                if (WcfSettingManager.CurrentServerSetting(serviceType).WcfCoreSetting.EnableUnity)
                {
                    if (serviceHost.Description.Behaviors.Find<UnityServiceBehavior>() == null)
                        serviceHost.Description.Behaviors.Add(new UnityServiceBehavior());
                }

                serviceHost.Description.Endpoints.Clear();
                var wcfService = GetWcfServiceConfiguration(serviceType);
                if (wcfService == null)
                    throw new Exception("不能找到Wcf服务端配置,请检查配置数据库!");

                var bindingCache = new Dictionary<string, Binding>();

                foreach (var ep in wcfService.Endpoints)
                {
                    string address = ConfigHelper.CreateAddress(
                        ep.EndpointProtocol,
                        Environment.MachineName,
                        ep.EndpointPort,
                        ep.EndpointName);

                    Binding binding;
                    if (!bindingCache.TryGetValue(address, out binding))
                    {
                        binding = ConfigHelper.CreateBinding(ep.EndpointBindingType, ep.EndpointBindingXml);
                        bindingCache[address] = binding;
                    }

                    serviceHost.AddServiceEndpoint(ep.ServiceContractType, binding, address);

                    if (!string.IsNullOrEmpty(ep.EndpointBehaviorXml))
                    {
                        AddEndPointBehavior(serviceHost.Description.Endpoints.Last(), ep.EndpointBehaviorXml);
                    }
                }

                foreach (var ep in serviceHost.Description.Endpoints)
                {
                    ep.Behaviors.Add(new MessageInspectorEndpointBehavior());
                }

                if (!string.IsNullOrEmpty(wcfService.ServiceBehaviorXml))
                    AddServiceBehavior(serviceHost, wcfService.ServiceBehaviorXml);

                serviceHost.Opened += (sender, o) =>
                {
                    if (WcfSettingManager.CurrentServerSetting(serviceType).WcfLogSetting.StartInfoSetting.Enabled)
                    {
                        var log = WcfLogProvider.GetServerStartInfo(serviceType.FullName, "WcfServiceHostFactory.CreateServiceHost", wcfService);
                        MongodbService.MongodbInsertService.Insert(log);
                    }
                };

                lock(performanceServices)
                {
                    if (!performanceServices.ContainsKey(serviceType.FullName))
                    {
                        var p = new WcfServerPerformanceService(serviceType.FullName, WcfSettingManager.CurrentServerSetting(serviceType).WcfPerformanceServiceSetting);
                        performanceServices.Add(serviceType.FullName, p);
                    }
                }
            }

            return serviceHost;
        }

        private static void AddEndPointBehavior(ServiceEndpoint serviceEndpoint, string behavior)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(behavior);
                var endpointBehaviorElement = new EndpointBehaviorElement();
                ConfigHelper.Deserialize(doc.OuterXml, endpointBehaviorElement);
                foreach (var item in endpointBehaviorElement)
                {
                    ConfigHelper.SetBehavior(serviceEndpoint.Behaviors, item);
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfServiceHostFactory", "AddEndPointBehavior");
                throw;
            }
        }

        private static void AddServiceBehavior(ServiceHost serviceHost, string behavior)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(behavior);
                var serviceBehaviorElement = new ServiceBehaviorElement();
                ConfigHelper.Deserialize(doc.OuterXml, serviceBehaviorElement);
                foreach (var item in serviceBehaviorElement)
                {
                    ConfigHelper.SetBehavior(serviceHost.Description.Behaviors, item);
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfServiceHostFactory", "AddServiceBehavior");
                throw;
            }
        }

        private static WcfServiceConfig GetWcfServiceConfiguration(Type serviceType)
        {
            var version = serviceType.GetInterfaces().Where(type => !type.Assembly.GlobalAssemblyCache).First().Assembly.GetName().Version;
            try
            {
                using (var scf = WcfServiceClientFactory.CreateServiceClient<IWcfConfigService>())
                {
                    return scf.Channel.GetWcfService(serviceType.FullName, version.Major + "." + version.Minor, CommonConfiguration.MachineIP);
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfServiceHostFactory", "GetWcfServiceConfiguration");
                throw;
            }
        }
    }
}
