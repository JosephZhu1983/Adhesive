using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adhesive.GeneralPerformance.Core
{
    internal class Configuration
    {
        internal static ConfigurationEntity GetConfig()
        {
            return new ConfigurationEntity
            {
#if DEBUG
                DatabaseUrlForGeneralPerformanceMaster = "mongodb://192.168.129.173:20000",
#else
                DatabaseUrlForGeneralPerformanceMaster = "mongodb://192.168.2.129:30000",
#endif
                PagePerformance = new ItemConfigurationEntity
                {
                    Name = "PagePerformance",
                    CollectSpan = TimeSpan.FromSeconds(10),
                    Prefix = "PP",
                    AggregateSpans = new Dictionary<string, TimeSpan> 
                    {
                        { "2m", TimeSpan.FromMinutes(2) },
                        { "20m", TimeSpan.FromMinutes(20) },
                        { "1h", TimeSpan.FromHours(1) },
                        { "3h", TimeSpan.FromHours(3) },
                        { "12h", TimeSpan.FromHours(12) },
                    },
                },
                WcfClientPerformance = new ItemConfigurationEntity
                {
                    Name = "WcfClientPerformance",
                    CollectSpan = TimeSpan.FromSeconds(10),
                    Prefix = "WCP",
                    AggregateSpans = new Dictionary<string, TimeSpan> 
                    {
                        { "2m", TimeSpan.FromMinutes(2) },
                        { "20m", TimeSpan.FromMinutes(20) },
                        { "1h", TimeSpan.FromHours(1) },
                        { "3h", TimeSpan.FromHours(3) },
                        { "12h", TimeSpan.FromHours(12) },
                    },
                },
                WcfServerPerformance = new ItemConfigurationEntity
                {
                    Name = "WcfServerPerformance",
                    CollectSpan = TimeSpan.FromSeconds(10),
                    Prefix = "WSP",
                    AggregateSpans = new Dictionary<string, TimeSpan> 
                    {
                        { "2m", TimeSpan.FromMinutes(2) },
                        { "20m", TimeSpan.FromMinutes(20) },
                        { "1h", TimeSpan.FromHours(1) },
                        { "3h", TimeSpan.FromHours(3) },
                        { "12h", TimeSpan.FromHours(12) },
                    },
                },
            };
        }
    }

    internal class ConfigurationEntity
    {
        public string DatabaseUrlForGeneralPerformanceMaster { get; set; }

        public ItemConfigurationEntity PagePerformance { get; set; }

        public ItemConfigurationEntity WcfClientPerformance { get; set; }

        public ItemConfigurationEntity WcfServerPerformance { get; set; }
    }

    internal class ItemConfigurationEntity
    {
        public string Name { get; set; }

        public string Prefix { get; set; }

        public TimeSpan CollectSpan { get; set; }

        public Dictionary<string, TimeSpan> AggregateSpans { get; set; }
    }
}
