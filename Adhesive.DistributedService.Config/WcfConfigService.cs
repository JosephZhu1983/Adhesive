

namespace Adhesive.DistributedService.Config
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using Adhesive.AppInfoCenter;
    using Adhesive.DistributedService.Imp;
    using Adhesive.Common;
    using System.Collections.Generic;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        Namespace = "WcfExtension")]
    public class WcfConfigService : IWcfConfigService
    {
        private static readonly string ModuleName = "WCF服务配置模块";

        private void Error(string categoryName, string subcategoryName, string message)
        {
            if (subcategoryName.Contains("Adhesive"))
                LocalLoggingService.Error(string.Format("categoryName : {0} subcategoryName : {1} message : {2} ", categoryName, subcategoryName, message));
            else
                AppInfoCenterService.LoggingService.Error(ModuleName, categoryName, subcategoryName, message);
        }

        private void Warning(string categoryName, string subcategoryName, string message)
        {
            if (subcategoryName.Contains("Adhesive"))
                LocalLoggingService.Warning(string.Format("categoryName : {0} subcategoryName : {1} message : {2} ", categoryName, subcategoryName, message));
            else
                AppInfoCenterService.LoggingService.Warning(ModuleName, categoryName, subcategoryName, message);
        }

        public WcfServiceConfig GetWcfService(string serviceType, string serviceContractVersion, string machineIP)
        {
            try
            {
                var wcfServices = WcfConfig.GetServices().Where(s => s.ServiceTypeName == serviceType).ToList();
                var wcfService = wcfServices.Where(s => machineIP.Contains(s.ServerMachineIP) || s.ServerMachineIP == "*").FirstOrDefault();
                if (wcfService == null)
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfService", string.Format("GetWcfService没获取到Service信息，参数为{0}，{1}，{2}", serviceType, serviceContractVersion, machineIP));
                    return null;
                }

                var service = new WcfServiceConfig
                {
                    ServiceType = serviceType,
                    ServiceBehaviorXml = wcfService.ServiceBehaviorXml != null ? wcfService.ServiceBehaviorXml.ToString() : "",
                };

                var endpoints = (from ep in WcfConfig.GetServiceEndpoints()
                                 where ep.ServiceTypeName == serviceType
                                 && (ep.ContractVersion == serviceContractVersion || ep.ContractVersion == "*")
                                 && (ep.ServerMachineIP == wcfService.ServerMachineIP || ep.ServerMachineIP == "*")
                                 select new WcfServiceEndpointConfig
                                 {
                                     EndpointBehaviorXml = ep.ServiceEndpointBehaviorXml != null ? ep.ServiceEndpointBehaviorXml.ToString() : "",
                                     EndpointBindingName = ep.ServiceEndpointBindingName,
                                     EndpointName = ep.ServiceEndpointName,
                                     EndpointPort = ep.ServiceEndpointPort,
                                     ServiceContractType = ep.ContractTypeName
                                 }).ToList();

                if (endpoints == null)
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfService", string.Format("GetWcfService没获取到Endpoint信息，参数为{0}，{1}，{2}", serviceType, serviceContractVersion, machineIP));
                    return null;
                }
                service.Endpoints = endpoints;
                foreach (var endpoint in service.Endpoints)
                {
                    var binding = WcfConfig.GetBindings().Single(b => b.BindingName == endpoint.EndpointBindingName);
                    if (binding == null)
                    {
                        Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfService", string.Format("GetWcfService没获取到Binding信息，参数为{0}，{1}，{2}", serviceType, serviceContractVersion, machineIP));
                        return null;
                    }
                    else
                    {
                        endpoint.EndpointBindingType = binding.BindingType;
                        endpoint.EndpointBindingXml = binding.BindingXml != null ? binding.BindingXml.ToString() : "";
                        endpoint.EndpointProtocol = binding.BindingProtocol;
                    }
                }

                return service;
            }
            catch (Exception ex)
            {
                Error("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfService", string.Format("GetWcfService出错：{0}，参数为{1}，{2}，{3}", ex.ToString(), serviceType, serviceContractVersion, machineIP));
                return null;
            }
        }

        public WcfClientEndpointConfig GetWcfClientEndpoint(string serviceContractType, string serviceContractVersion, string machineIP)
        {
            try
            {
                var wcfClientEndpoint = (from ep in (WcfConfig.GetServiceEndpoints()
                                         .Where(s => s.ContractTypeName == serviceContractType).ToList())
                                         let binding = WcfConfig.GetBindings().Single(b => b.BindingName == ep.ServiceEndpointBindingName)
                                         where (ep.ContractVersion == "*" || float.Parse(ep.ContractVersion) >= float.Parse(serviceContractVersion))
                                         orderby ep.ContractVersion ascending, binding.BindingPriority ascending
                                         select new WcfClientEndpointConfig
                                         {
                                             EndpointName = ep.ServiceEndpointName,
                                             EndpointPort = ep.ServiceEndpointPort,
                                             ServiceContractType = ep.ContractTypeName,
                                             EndpointBindingType = binding.BindingType,
                                             EndpointBindingXml = binding.BindingXml != null ? binding.BindingXml.ToString() : "",
                                             EndpointProtocol = binding.BindingProtocol,
                                             ServiceType = ep.ServiceTypeName,
                                         }).FirstOrDefault();

                if (wcfClientEndpoint == null)
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfClientEndpoint", string.Format("GetWcfClientEndpoint没获取到ClientEndpoint信息，参数为{0}，{1}，{2}", serviceContractType, serviceContractVersion, machineIP));
                    return null;
                }
                var wcfService = WcfConfig.GetServices().Where(s => s.ServiceTypeName == wcfClientEndpoint.ServiceType).FirstOrDefault();
                if (wcfService == null)
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfClientEndpoint", string.Format("GetWcfClientEndpoint没获取到Service信息，参数为{0}，{1}，{2}", serviceContractType, serviceContractVersion, machineIP));
                    return null;
                }

                var wcfServerFarm = WcfConfig.GetServerFarms().FirstOrDefault(f => f.ServerFarmName == wcfService.ServerFarmName && machineIP.Contains(f.ClientMachineIP));
                if (wcfServerFarm == null)
                    wcfServerFarm = WcfConfig.GetServerFarms().FirstOrDefault(f => f.ServerFarmName == wcfService.ServerFarmName && f.ClientMachineIP == "*");
                if (wcfServerFarm == null)
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfClientEndpoint", string.Format("GetWcfClientEndpoint没获取到ServerFarm信息，参数为{0}，{1}，{2}", serviceContractType, serviceContractVersion, machineIP));
                    return null;
                }
                wcfClientEndpoint.EndpointAddress = wcfServerFarm.ServerFarmAddress;
                wcfClientEndpoint.ServerFarmName = wcfServerFarm.ServerFarmName;

                var allowFarmNames = WcfConfig.GetAllowClientAccesses().Where(acc => acc.ClientMachineIP == "*" || machineIP.Contains(acc.ClientMachineIP)).Select(a => a.AccessServerFarmName).ToList();
                if (!allowFarmNames.Contains("*") && !allowFarmNames.Contains(wcfClientEndpoint.ServerFarmName))
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfClientEndpoint", string.Format("GetWcfClientEndpoint没访问ServerFarm的权限，原因是不在允许列表中，参数为{0}，{1}，{2}", serviceContractType, serviceContractVersion, machineIP));
                    return null;
                }

                var denyFarmNames = WcfConfig.GetDenyClientAccesses().Where(acc => acc.ClientMachineIP == "*" || machineIP.Contains(acc.ClientMachineIP)).Select(a => a.AccessServerFarmName).ToList();
                if (denyFarmNames.Contains("*") || denyFarmNames.Contains(wcfClientEndpoint.ServerFarmName))
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfClientEndpoint", string.Format("GetWcfClientEndpoint没访问ServerFarm的权限，原因是在拒绝列表中，参数为{0}，{1}，{2}", serviceContractType, serviceContractVersion, machineIP));
                    return null;
                }

                var query =
                    (from ce in WcfConfig.GetClientEndpoints()
                     where ce.ContractTypeName == wcfClientEndpoint.ServiceContractType
                     select ce.ClientEndpointBehaviorXml).FirstOrDefault();

                wcfClientEndpoint.EndpointBehaviorXml = query != null ? query.ToString() : "";

                return wcfClientEndpoint;

            }
            catch (Exception ex)
            {
                Error("Adhesive.DistributedService.Config", "WcfConfigService.GetWcfClientEndpoint", string.Format("GetWcfClientEndpoint出错：{0}，参数为{1}，{2}，{3}", ex.ToString(), serviceContractType, serviceContractVersion, machineIP));
                return null;
            }
        }

        public WcfClientSetting GetClientSetting(string serviceContractType, string machineIP)
        {
            var setting = new WcfClientSetting
            {
                WcfCoreSetting = new WcfClientCoreSetting
                {

                },
                WcfPerformanceServiceSetting = new WcfPerformanceServiceSetting
                {
                    Enabled = true,
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
                        Enabled = false,
                    },
                    StartInfoSetting = new StartInfoSetting
                    {
                        Enabled = true,
                    },
                    MessageInfoSetting = new MessageInfoSetting
                    {
                        Enabled = false,
                    }
                },
                WcfSecuritySetting = new WcfSecuritySetting
                {
                    PasswordCheck = new PasswordCheck
                    {
                        Enable = false,
                    }
                }
            };

            try
            {
                var endpoints = WcfConfig.GetClientEndpoints().Where(ep => ep.ContractTypeName == serviceContractType).ToList();
                var e = endpoints.Where(endpoint => machineIP.Contains(endpoint.ClientMachineIP) || endpoint.ClientMachineIP == "*").FirstOrDefault();
                if (e == null)
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetClientSetting", string.Format("没有获取到ClientEndpoint，参数为{0}，{1}", serviceContractType, machineIP));
                }
                else
                {
                    var clientConfig = e.ClientSetting;
                    if (clientConfig != null)
                    {
                        setting.WcfCoreSetting = clientConfig.WcfCoreSetting;
                        setting.WcfLogSetting = clientConfig.WcfLogSetting;
                    }
                    else
                    {
                        Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetClientSetting", string.Format("没有获取到ClientSetting，参数为{0}，{1}", serviceContractType, machineIP));
                    }
                    var serviceEndpoint = WcfConfig.GetServiceEndpoints().Where(se => se.ContractTypeName == serviceContractType).FirstOrDefault();
                    if (serviceEndpoint != null)
                    {
                        var service = WcfConfig.GetServices().Where(s => s.ServiceTypeName == serviceEndpoint.ServiceTypeName).FirstOrDefault();
                        if (service != null)
                        {
                            var serviceConfig = service.ServerSetting;
                            if (serviceConfig != null)
                            {
                                setting.WcfSecuritySetting = serviceConfig.WcfSecuritySetting;
                            }
                            else
                            {
                                Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetClientSetting", string.Format("没有获取到ServerSetting，参数为{0}，{1}", serviceContractType, machineIP));
                            }
                        }
                        else
                        {
                            Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetClientSetting", string.Format("没有获取到Service，参数为{0}，{1}", serviceContractType, machineIP));
                        }
                    }
                    else
                    {
                        Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetClientSetting", string.Format("没有获取到ServiceEndpoint，参数为{0}，{1}", serviceContractType, machineIP));
                    }
                }
            }
            catch (Exception ex)
            {
                Error("Adhesive.DistributedService.Config", "WcfConfigService.GetClientSetting", string.Format("GetClientSetting出错：{0}，参数为{1}，{2}", ex.ToString(), serviceContractType, machineIP));
            }

            return setting;
        }

        public WcfServerSetting GetServerSetting(string serviceType, string machineIP)
        {
            var setting = new WcfServerSetting
            {
                WcfCoreSetting = new WcfServerCoreSetting
                {
                    EnableUnity = false,
                },
                WcfPerformanceServiceSetting = new WcfPerformanceServiceSetting
                {
                    Enabled = true,
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
                        Enabled = false,
                    },
                    StartInfoSetting = new StartInfoSetting
                    {
                        Enabled = true,
                    },
                    MessageInfoSetting = new MessageInfoSetting
                    {
                        Enabled = false,
                    }
                },
                WcfSecuritySetting = new WcfSecuritySetting
                {
                    PasswordCheck = new PasswordCheck
                    {
                        Enable = false,
                    }
                }
            };

            try
            {
                var services = WcfConfig.GetServices().Where(service => service.ServiceTypeName == serviceType).ToList();
                var s = services.Where(service => machineIP.Contains(service.ServerMachineIP) || service.ServerMachineIP == "*").FirstOrDefault();
                if (s == null)
                {
                    Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetServerSetting", string.Format("没有获取到Service，参数为{0}，{1}", serviceType, machineIP));
                }
                else
                {
                    var config = s.ServerSetting;
                    if (config != null)
                    {
                        setting = config;
                    }
                    else
                    {
                        Warning("Adhesive.DistributedService.Config", "WcfConfigService.GetServerSetting", string.Format("没有获取到ServerSetting，参数为{0}，{1}", serviceType, machineIP));
                    }
                }

            }
            catch (Exception ex)
            {
                Error("Adhesive.DistributedService.Config", "WcfConfigService.GetServerSetting", string.Format("GetServerSetting出错：{0}，参数为{1}，{2}", ex.ToString(), serviceType, machineIP));
            }

            return setting;
        }
    }
}