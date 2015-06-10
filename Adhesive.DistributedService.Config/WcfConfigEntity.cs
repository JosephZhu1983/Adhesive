
using System.Collections.Generic;
using Adhesive.Config;
using Adhesive.DistributedService.Imp;

namespace Adhesive.DistributedService.Config
{
    [ConfigEntity(FriendlyName = "WCF服务配置")]
    class WcfConfigEntity
    {
        [ConfigItem(FriendlyName = "绑定配置")]
        public Dictionary<string, Binding> Bindings { get; set; }

        [ConfigItem(FriendlyName = "允许客户端配置")]
        public Dictionary<string, ClientAccess> AllowClientAccesses { get; set; }

        [ConfigItem(FriendlyName = "禁止客户端配置")]
        public Dictionary<string, ClientAccess> DenyClientAccesses { get; set; }

        [ConfigItem(FriendlyName = "客户端端点配置")]
        public Dictionary<string, ClientEndpoint> ClientEndpoints { get; set; }

        [ConfigItem(FriendlyName = "服务配置")]
        public Dictionary<string, Service> Services { get; set; }

        [ConfigItem(FriendlyName = "服务集群配置")]
        public Dictionary<string, ServerFarm> ServerFarms { get; set; }

        [ConfigItem(FriendlyName = "服务端点配置")]
        public Dictionary<string, ServiceEndpoint> ServiceEndpoints { get; set; }
    }

    [ConfigEntity(FriendlyName = "绑定配置")]
    class Binding
    {
        [ConfigItem(FriendlyName = "绑定名")]
        public string BindingName { get; set; }

        [ConfigItem(FriendlyName = "绑定类型", Description = "比如NetTcpBinding")]
        public string BindingType { get; set; }

        [ConfigItem(FriendlyName = "绑定优先级", Description = "数字越小优先级越高")]
        public int BindingPriority { get; set; }

        [ConfigItem(FriendlyName = "绑定行为")]
        public string BindingXml { get; set; }

        [ConfigItem(FriendlyName = "绑定协议", Description = "比如net.tcp")]
        public string BindingProtocol { get; set; }
    }

    [ConfigEntity(FriendlyName = "报警服务配置")]
    class ClientAccess
    {
        [ConfigItem(FriendlyName = "客户端机器IP", Description = "*表示所有")]
        public string ClientMachineIP { get; set; }

        [ConfigItem(FriendlyName = "服务集群名", Description = "*表示所有")]
        public string AccessServerFarmName { get; set; }
    }

    [ConfigEntity(FriendlyName = "客户端端点配置")]
    class ClientEndpoint
    {
        [ConfigItem(FriendlyName = "契约类型名")]
        public string ContractTypeName { get; set; }

        [ConfigItem(FriendlyName = "客户端机器IP", Description = "*表示所有")]
        public string ClientMachineIP { get; set; }

        [ConfigItem(FriendlyName = "客户端端点行为XML")]
        public string ClientEndpointBehaviorXml { get; set; }

        [ConfigItem(FriendlyName = "客户端设置")]
        public WcfClientSetting ClientSetting { get; set; }
    }

    [ConfigEntity(FriendlyName = "服务配置")]
    class Service
    {
        [ConfigItem(FriendlyName = "服务类型名")]
        public string ServiceTypeName { get; set; }

        [ConfigItem(FriendlyName = "服务行为XML")]
        public string ServiceBehaviorXml { get; set; }

        [ConfigItem(FriendlyName = "服务端机器IP", Description = "*表示所有")]
        public string ServerMachineIP { get; set; }

        [ConfigItem(FriendlyName = "服务集群名")]
        public string ServerFarmName { get; set; }

        [ConfigItem(FriendlyName = "服务端设置")]
        public WcfServerSetting ServerSetting { get; set; }
    }

    [ConfigEntity(FriendlyName = "服务集群配置")]
    class ServerFarm
    {
        [ConfigItem(FriendlyName = "服务集群名")]
        public string ServerFarmName { get; set; }

        [ConfigItem(FriendlyName = "服务集群地址", Description = "负载均衡的地址")]
        public string ServerFarmAddress { get; set; }

        [ConfigItem(FriendlyName = "客户端机器IP", Description = "*表示所有")]
        public string ClientMachineIP { get; set; }
    }

    [ConfigEntity(FriendlyName = "服务端点配置")]
    class ServiceEndpoint
    {
        [ConfigItem(FriendlyName = "服务类型名")]
        public string ServiceTypeName { get; set; }

        [ConfigItem(FriendlyName = "契约类型名")]
        public string ContractTypeName { get; set; }

        [ConfigItem(FriendlyName = "契约版本")]
        public string ContractVersion { get; set; }

        [ConfigItem(FriendlyName = "服务器IP", Description = "*表示所有")]
        public string ServerMachineIP { get; set; }

        [ConfigItem(FriendlyName = "服务端点行为XML")]
        public string ServiceEndpointBehaviorXml { get; set; }

        [ConfigItem(FriendlyName = "服务端端点绑定名")]
        public string ServiceEndpointBindingName { get; set; }

        [ConfigItem(FriendlyName = "服务端口")]
        public int ServiceEndpointPort { get; set; }

        [ConfigItem(FriendlyName = "服务端点名")]
        public string ServiceEndpointName { get; set; }
    }
}
