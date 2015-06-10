
using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;
using Adhesive.Config;
using Adhesive.DistributedService.Imp;

namespace Adhesive.DistributedService.Config
{
    internal class WcfConfig
    {
        private static IConfigService configService = LocalServiceLocator.GetService<IConfigService>();

        private static WcfClientSetting GetDefaultWcfClientSetting(bool detail, bool performance)
        {
            return new WcfClientSetting
            {
                WcfCoreSetting = new WcfClientCoreSetting
                {

                },
                WcfPerformanceServiceSetting = new WcfPerformanceServiceSetting
                {
                    Enabled = performance,
                    ReportStateIntervalMilliSeconds = 10000,
                    AllowMethods = new List<string> { "*" },
                    DenyMethods = new List<string>(),
                },
                WcfLogSetting = new WcfLogSetting
                {
                    Enabled = true,
                    ExceptionInfoSetting = new ExceptionInfoSetting
                    {
                        Enabled = true,
                    },
                    InvokeInfoSetting = new InvokeInfoSetting
                    {
                        Enabled = detail,
                    },
                    StartInfoSetting = new StartInfoSetting
                    {
                        Enabled = true,
                    },
                    MessageInfoSetting = new MessageInfoSetting
                    {
                        Enabled = detail,
                        MessageDirection = MessageDirection.Both,
                    }
                },
            };
        }

        private static WcfServerSetting GetDefaultWcfServerSetting(bool detail, bool performance)
        {
            return new WcfServerSetting
            {
                WcfCoreSetting = new WcfServerCoreSetting
                {
                    EnableUnity = false,
                },
                WcfPerformanceServiceSetting = new WcfPerformanceServiceSetting
                {
                    Enabled = performance,
                    ReportStateIntervalMilliSeconds = 10000,
                    AllowMethods = new List<string> { "*" },
                    DenyMethods = new List<string>(),
                },
                WcfLogSetting = new WcfLogSetting
                {
                    Enabled = true,
                    ExceptionInfoSetting = new ExceptionInfoSetting
                    {
                        Enabled = true,
                    },
                    InvokeInfoSetting = new InvokeInfoSetting
                    {
                        Enabled = detail,
                    },
                    StartInfoSetting = new StartInfoSetting
                    {
                        Enabled = true,
                    },
                    MessageInfoSetting = new MessageInfoSetting
                    {
                        Enabled = detail,
                        MessageDirection = MessageDirection.Both,
                    }
                },
                WcfSecuritySetting = new WcfSecuritySetting
                {
                    PasswordCheck = new PasswordCheck
                    {
                        Enable = true,
                        Direction = OperationDirection.Both,
                        Password = "//5173@#",
                    }
                }
            };
        }

        private static WcfConfigEntity GetDefaultConfig()
        {
            return new WcfConfigEntity
            {
                Bindings = new Dictionary<string, Binding>
                {
                    {"UnsecuredTcp", new Binding
                        {
                            BindingName = "UnsecuredTcp",
                            BindingPriority = 1,
                            BindingProtocol = "net.tcp",
                            BindingType = "NetTcpBinding",
                            BindingXml = "<binding name=\"tcp_Unsecured\" maxReceivedMessageSize=\"655360\" receiveTimeout=\"00:10:00\" sendTimeout=\"00:10:00\"><security mode=\"None\" /><readerQuotas maxStringContentLength=\"8192000\" /></binding>",
                        }
                    },
                },

                ServerFarms = new Dictionary<string, ServerFarm>
                {
#if DEBUG
                    {"AdhesiveFarm_192_168_129_143", new ServerFarm
                        {
                            ServerFarmName = "AdhesiveFarm",
                            ServerFarmAddress = "192.168.129.143",
                            ClientMachineIP = "*",
                        }
                    },
                    {"AdhesiveFarm_127_0_0_1_OnlyForZhuye", new ServerFarm
                        {
                            ServerFarmName = "AdhesiveFarm",
                            ServerFarmAddress = "127.0.0.1",
                            ClientMachineIP = "192.168.134.187",
                        }
                    },
                    {"GeneralPerformance_192_168_129_143", new ServerFarm
                        {
                            ServerFarmName = "GeneralPerformance",
                            ServerFarmAddress = "192.168.129.143",
                            ClientMachineIP = "*",
                        }
                    },
                    {"GeneralPerformance_127_0_0_1_OnlyForZhuye", new ServerFarm
                        {
                            ServerFarmName = "GeneralPerformance",
                            ServerFarmAddress = "127.0.0.1",
                            ClientMachineIP = "192.168.134.187",
                        }
                    },
#else
                     {"AdhesiveFarm_Online", new ServerFarm
                        {
                            ServerFarmName = "AdhesiveFarm",
                            ServerFarmAddress = "192.168.206.34",
                            ClientMachineIP = "*",
                        }
                    },
                     {"GeneralPerformance_Online", new ServerFarm
                        {
                            ServerFarmName = "GeneralPerformance",
                            ServerFarmAddress = "192.168.2.215",
                            ClientMachineIP = "*",
                        }
                    },
#endif
                },

                AllowClientAccesses = new Dictionary<string, ClientAccess>
                {
                    {"AdhesiveFarm_All", new ClientAccess
                        {
                            AccessServerFarmName = "AdhesiveFarm",
                            ClientMachineIP = "*",
                           
                        }
                    },
                    {"GeneralPerformance_All", new ClientAccess
                        {
                            AccessServerFarmName = "GeneralPerformance",
                            ClientMachineIP = "*",
                           
                        }
                    },
                },

                DenyClientAccesses = new Dictionary<string, ClientAccess>
                {
                    {"AdhesiveFarm_None", new ClientAccess
                        {
                            AccessServerFarmName = "AdhesiveFarm",
                            ClientMachineIP = "0.0.0.0",                           
                        }
                    },
                    {"GeneralPerformance_None", new ClientAccess
                        {
                            AccessServerFarmName = "GeneralPerformance",
                            ClientMachineIP = "0.0.0.0",                           
                        }
                    },
                },

                ClientEndpoints = new Dictionary<string, ClientEndpoint>
                {
                    {"Adhesive.DistributedService.TestService.IFuckService_*", new ClientEndpoint
                        {
                            ContractTypeName = "Adhesive.DistributedService.TestService.IFuckService",
                            ClientMachineIP = "*",
                            ClientEndpointBehaviorXml = "<behavior name=\"myBehavior\" />",
                            ClientSetting = GetDefaultWcfClientSetting(true, true),
                        }
                    },
                    {"Adhesive.Mongodb.Server.IMongodbServer_*", new ClientEndpoint
                        {
                            ContractTypeName = "Adhesive.Mongodb.Server.IMongodbServer",
                            ClientMachineIP = "*",
                            ClientSetting = GetDefaultWcfClientSetting(false, true),
                        }
                    },
                    {"Adhesive.GeneralPerformance.Common.IService_*", new ClientEndpoint
                        {
                            ContractTypeName = "Adhesive.GeneralPerformance.Common.IService",
                            ClientMachineIP = "*",
                            ClientSetting = GetDefaultWcfClientSetting(false, false),
                        }
                    },
                },

                Services = new Dictionary<string, Service>
                {
                    {"Adhesive.DistributedService.TestService.Host.FuckService_*", new Service
                        {
                            ServiceTypeName = "Adhesive.DistributedService.TestService.Host.FuckService",
                            ServerFarmName = "AdhesiveFarm",
                            ServerMachineIP = "*",
                            ServiceBehaviorXml = "<behavior name=\"ServieBehavior\"><serviceMetadata httpGetEnabled=\"false\" /><serviceThrottling maxConcurrentCalls=\"200\" maxConcurrentInstances=\"200\" maxConcurrentSessions=\"200\" /></behavior>",
                            ServerSetting = GetDefaultWcfServerSetting(true, true),
                        }
                    },
                    {"Adhesive.Mongodb.Server.Imp.MongodbServer_*", new Service
                        {
                            ServiceTypeName = "Adhesive.Mongodb.Server.Imp.MongodbServer",
                            ServerFarmName = "AdhesiveFarm",
                            ServerMachineIP = "*",
                            ServerSetting = GetDefaultWcfServerSetting(false, true),
                        }
                    },
                    {"Adhesive.GeneralPerformance.Core.Service_*", new Service
                        {
                            ServiceTypeName = "Adhesive.GeneralPerformance.Core.Service",
                            ServerFarmName = "GeneralPerformance",
                            ServerMachineIP = "*",
                            ServerSetting = GetDefaultWcfServerSetting(false, true),
                        }
                    },
                },

                ServiceEndpoints = new Dictionary<string, ServiceEndpoint>
                {
                    {"Adhesive.DistributedService.TestService.Host.FuckService_*_*", new ServiceEndpoint
                        {
                            ContractTypeName = "Adhesive.DistributedService.TestService.IFuckService",
                            ContractVersion = "*",
                            ServerMachineIP = "*",
                            ServiceTypeName = "Adhesive.DistributedService.TestService.Host.FuckService",
                            ServiceEndpointBindingName = "UnsecuredTcp",
                            ServiceEndpointName = "Adhesive.DistributedService.TestService.Host.FuckService",
                            ServiceEndpointPort = 12346,
                            ServiceEndpointBehaviorXml = "<behavior name=\"ServiceEndpointBehavior\"><dataContractSerializer maxItemsInObjectGraph=\"10000\" /></behavior>"
                        }
                    },
                    {"Adhesive.Mongodb.Server.Imp.MongodbServer_*_*", new ServiceEndpoint
                        {
                            ContractTypeName = "Adhesive.Mongodb.Server.IMongodbServer",
                            ContractVersion = "*",
                            ServerMachineIP = "*",
                            ServiceTypeName = "Adhesive.Mongodb.Server.Imp.MongodbServer",
                            ServiceEndpointBindingName = "UnsecuredTcp",
                            ServiceEndpointName = "Adhesive.Mongodb.Server.Imp.MongodbServer",
                            ServiceEndpointPort = 12345,
                           
                        }
                    },
                     {"Adhesive.GeneralPerformance.Common.IService_*_*", new ServiceEndpoint
                        {
                            ContractTypeName = "Adhesive.GeneralPerformance.Common.IService",
                            ContractVersion = "*",
                            ServerMachineIP = "*",
                            ServiceTypeName = "Adhesive.GeneralPerformance.Core.Service",
                            ServiceEndpointBindingName = "UnsecuredTcp",
                            ServiceEndpointName = "Adhesive.GeneralPerformance.Core.Service",
                            ServiceEndpointPort = 12347,
                           
                        }
                    },
                },
            };
        }

        private static WcfConfigEntity GetConfig()
        {
            var defaultConfig = GetDefaultConfig();
            var config = configService.GetConfigItemValue(true, "DistributedServiceConfiguration", defaultConfig);
            return config;
        }

        internal static List<Binding> GetBindings()
        {
            return GetConfig().Bindings.Select(item => item.Value).ToList();
        }

        internal static List<ClientAccess> GetAllowClientAccesses()
        {
            return GetConfig().AllowClientAccesses.Select(item => item.Value).ToList();
        }

        internal static List<ClientAccess> GetDenyClientAccesses()
        {
            var dic = GetConfig().DenyClientAccesses;
            return dic != null ? dic.Select(item => item.Value).ToList() : new List<ClientAccess>();
        }

        internal static List<ClientEndpoint> GetClientEndpoints()
        {
            return GetConfig().ClientEndpoints.Select(item => item.Value).ToList();
        }

        internal static List<Service> GetServices()
        {
            return GetConfig().Services.Select(item => item.Value).ToList();
        }

        internal static List<ServerFarm> GetServerFarms()
        {
            return GetConfig().ServerFarms.Select(item => item.Value).ToList();
        }

        internal static List<ServiceEndpoint> GetServiceEndpoints()
        {
            return GetConfig().ServiceEndpoints.Select(item => item.Value).ToList();
        }
    }
}
