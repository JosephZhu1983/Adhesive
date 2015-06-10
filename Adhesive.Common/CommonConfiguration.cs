
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Adhesive.Common
{
    public class CommonConfiguration
    {
        public static readonly string MachineIP = string.Join(" / ", Dns.GetHostAddresses(Dns.GetHostName())
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork).Select(add => add.ToString()).ToArray());

        public static readonly string MachineName = Environment.MachineName;

        public static AdhesiveConfig GetConfig()
        {
            var config = LocalConfigService.GetConfig(new AdhesiveConfig
            {
                ApplicationName = "",
#if DEBUG
                ConfigServiceAddress = "localhost:18989/ConfigService",
                WcfConfigServiceAddress = "localhost:18888/WcfConfigService",
                MongodbServiceAddress = "localhost:12345/MongodbService",
#else
                ConfigServiceAddress = "192.168.206.33:18989/ConfigService",
                WcfConfigServiceAddress = "192.168.206.36:18888/WcfConfigService",
                MongodbServiceAddress="192.168.206.34:12345/MongodbService",
#endif
                LocalLoggingServiceLevel = LogLevel.Info,
                StateServiceDefaultReportStateIntervalMilliSeconds = 1000 * 10,
                ClearLocalLogWhenStart = false,
            });

            if (string.IsNullOrEmpty(config.ApplicationName))
                throw new Exception("请在Config/AdhesiveConfig.config中配置ApplicationName节点的值为应用程序的名字！");
            if (string.IsNullOrEmpty(config.ConfigServiceAddress))
                throw new Exception("请在Config/AdhesiveConfig.config中配置ConfigServiceAddress节点的值为配置服务的地址！");
            if (string.IsNullOrEmpty(config.WcfConfigServiceAddress))
                throw new Exception("请在Config/AdhesiveConfig.config中配置WcfConfigServiceAddress节点的值为Wcf配置服务的地址！");
            if (config.StateServiceDefaultReportStateIntervalMilliSeconds < 500)
                config.StateServiceDefaultReportStateIntervalMilliSeconds = 500;

            return config;
        }
    }
}
