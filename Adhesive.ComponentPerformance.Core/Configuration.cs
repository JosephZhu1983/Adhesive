using System;
using System.Collections.Generic;
using Adhesive.Common;
using Adhesive.Config;

namespace Adhesive.ComponentPerformance.Core
{
    internal class Configuration
    {
        private static IConfigService configService = LocalServiceLocator.GetService<IConfigService>();

        internal static ComponentConfigurationEntity GetConfig()
        {
            var defaultConfig = new ComponentConfigurationEntity
            {
#if DEBUG
                DatabaseUrlForComponentPerformanceMaster = "mongodb://192.168.129.142:20000",
                DatabaseUrlForComponentPerformanceSlave = "mongodb://192.168.129.142:20000",
                DatabaseUrlForGeneralPerformanceSlave = "mongodb://192.168.129.173:20000",
                MongodbList = new Dictionary<string, ComponentConfiguration>
                {
                    {"192_168_129_142_20000", new MongodbConfiguration { Url = "mongodb://192.168.129.142:20000/?slaveok=true", IsMaster = true } },
                    {"192_168_129_172_20000", new MongodbConfiguration { Url = "mongodb://192.168.129.172:20000/?slaveok=true", IsMaster = true } },
                },
                MemcachedList = new Dictionary<string, ComponentConfiguration>
                {
                    {"192_168_135_221_12000", new MemcachedConfiguration { Url = "192.168.135.221:12000" } },
                    {"192_168_135_221_12001", new MemcachedConfiguration { Url = "192.168.135.221:12001" } },
                },
                KTList = new Dictionary<string, ComponentConfiguration>
                {
                },
                RedisList = new Dictionary<string, ComponentConfiguration>
                {
                    {"192_168_129_175_6379", new RedisConfiguration { Url = "192.168.129.175:6379" } },
                }
#else
                DatabaseUrlForComponentPerformanceMaster = "mongodb://192.168.2.127:30000",
                DatabaseUrlForComponentPerformanceSlave = "mongodb://192.168.2.128:30000/?slaveok=true",
                DatabaseUrlForGeneralPerformanceSlave = "mongodb://192.168.2.130:30000/?slaveok=true",
                MongodbList = new Dictionary<string, ComponentConfiguration>
                {
                    {"192_168_2_127_10000", new MongodbConfiguration { Url = "mongodb://192.168.2.127:10000/?slaveok=true", IsMaster = true } },
                    {"192_168_2_128_10000", new MongodbConfiguration { Url = "mongodb://192.168.2.128:10000/?slaveok=true" } },
                    {"192_168_2_129_10000", new MongodbConfiguration { Url = "mongodb://192.168.2.129:10000/?slaveok=true", IsMaster = true } },
                    {"192_168_2_130_10000", new MongodbConfiguration { Url = "mongodb://192.168.2.130:10000/?slaveok=true" } },
                    {"192_168_2_127_20000", new MongodbConfiguration { Url = "mongodb://192.168.2.127:20000/?slaveok=true" } },
                    {"192_168_2_128_20000", new MongodbConfiguration { Url = "mongodb://192.168.2.128:20000/?slaveok=true" } },
                    {"192_168_2_129_20000", new MongodbConfiguration { Url = "mongodb://192.168.2.129:20000/?slaveok=true" } },
                    {"192_168_2_130_20000", new MongodbConfiguration { Url = "mongodb://192.168.2.130:20000/?slaveok=true" } },
                    {"192_168_2_219_10000", new MongodbConfiguration { Url = "mongodb://192.168.2.219:10000/?slaveok=true" , IsMaster = true} },
                    {"192_168_2_226_10000", new MongodbConfiguration { Url = "mongodb://192.168.2.226:10000/?slaveok=true" } },
                    {"192_168_2_127_30000", new MongodbConfiguration { Url = "mongodb://192.168.2.127:30000/?slaveok=true", IsMaster = true } },
                    {"192_168_2_128_30000", new MongodbConfiguration { Url = "mongodb://192.168.2.128:30000/?slaveok=true"} },
                    {"192_168_2_129_30000", new MongodbConfiguration { Url = "mongodb://192.168.2.129:30000/?slaveok=true", IsMaster = true } },
                    {"192_168_2_130_30000", new MongodbConfiguration { Url = "mongodb://192.168.2.130:30000/?slaveok=true" } },
                    {"192_168_2_219_30000", new MongodbConfiguration { Url = "mongodb://192.168.2.219:30000/?slaveok=true" } },
                    {"192_168_2_226_30000", new MongodbConfiguration { Url = "mongodb://192.168.2.226:30000/?slaveok=true" } },
                },
                MemcachedList = new Dictionary<string, ComponentConfiguration>
                {
                      {"192_168_2_219_11211", new MemcachedConfiguration { Url = "192.168.2.219:11211" } },
                      {"192_168_2_219_11212", new MemcachedConfiguration { Url = "192.168.2.219:11212" } },
                      {"192_168_2_226_11211", new MemcachedConfiguration { Url = "192.168.2.226:11211" } },
                      {"192_168_2_226_11212", new MemcachedConfiguration { Url = "192.168.2.226:11212" } },
                      {"192_168_0_41_11212", new MemcachedConfiguration { Url = "192.168.0.41:11212" } },
                      {"192_168_0_42_11212", new MemcachedConfiguration { Url = "192.168.0.42:11212" } },
                      {"192_168_2_61_11217", new MemcachedConfiguration { Url = "192.168.2.61:11217" } },
                      {"192_168_2_62_11217", new MemcachedConfiguration { Url = "192.168.2.62:11217" } },
                },
                KTList = new Dictionary<string, ComponentConfiguration>
                {
                      {"192_168_0_39_11211", new KTConfiguration { Url = "192.168.0.39:11211" } },
                      {"192_168_0_40_11211", new KTConfiguration { Url = "192.168.0.40:11211" } },
                      {"192_168_0_41_11211", new KTConfiguration { Url = "192.168.0.41:11211" } },
                      {"192_168_0_42_11211", new KTConfiguration { Url = "192.168.0.42:11211" } },
                      {"192_168_2_129_11211", new KTConfiguration { Url = "192.168.2.129:11211" } },
                      {"192_168_2_219_20000", new KTConfiguration { Url = "192.168.2.219:20000" } },
                      {"192_168_2_226_20000", new KTConfiguration { Url = "192.168.2.226:20000" } },
                      {"192_168_2_219_22222", new KTConfiguration { Url = "192.168.2.219:22222" } },
                      {"192_168_2_226_22222", new KTConfiguration { Url = "192.168.2.226:22222" } },
                      {"192_168_0_41_22222", new KTConfiguration { Url = "192.168.0.41:22222" } },
                      {"192_168_0_42_22222", new KTConfiguration { Url = "192.168.0.42:22222" } },
                },
                RedisList = new Dictionary<string, ComponentConfiguration>
                {
                    {"192_168_0_61_6379", new RedisConfiguration { Url = "192.168.0.61:6379" } },
                    {"192_168_0_63_6379", new RedisConfiguration { Url = "192.168.0.63:6379" } },
                }

#endif
            };
#if DEBUG
            //return configService.GetConfigItemValue<ComponentConfigurationEntity>(false, "ComponentPerformance", defaultConfig, arg => Service.Reset());
            return defaultConfig;
#else
            return defaultConfig;
            //return configService.GetConfigItemValue<ComponentConfigurationEntity>(false, "ComponentPerformance", defaultConfig, arg => Service.Reset());
#endif

        }
    }

    public class ComponentConfigurationEntity
    {
        public string DatabaseUrlForComponentPerformanceMaster { get; set; }

        public string DatabaseUrlForComponentPerformanceSlave { get; set; }

        public string DatabaseUrlForGeneralPerformanceSlave { get; set; }

        public Dictionary<string, ComponentConfiguration> MongodbList { get; set; }

        public Dictionary<string, ComponentConfiguration> MemcachedList { get; set; }

        public Dictionary<string, ComponentConfiguration> KTList { get; set; }

        public Dictionary<string, ComponentConfiguration> RedisList { get; set; }
    }

    public class MongodbConfiguration : ComponentConfiguration
    {
        public bool IsMaster { get; set; } 

        public MongodbConfiguration()
        {
            ComponentItems = Mongodb.GetComponentItems();
        }

        public override Dictionary<string, object> GetCurrentStatus()
        {
            return Mongodb.GetCurrentStatus(base.Url);
        }
    }


    public class MemcachedConfiguration : ComponentConfiguration
    {
        public MemcachedConfiguration()
        {
            ComponentItems = Memcached.GetComponentItems();
        }

        public override Dictionary<string, object> GetCurrentStatus()
        {
            return Memcached.GetCurrentStatus(base.Url);
        }
    }

    public class KTConfiguration : ComponentConfiguration
    {
        public KTConfiguration()
        {
            ComponentItems = KT.GetComponentItems();
        }

        public override Dictionary<string, object> GetCurrentStatus()
        {
            return KT.GetCurrentStatus(base.Url);
        }
    }

    public class RedisConfiguration : ComponentConfiguration
    {
        public RedisConfiguration()
        {
            ComponentItems = Redis.GetComponentItems();
        }

        public override Dictionary<string, object> GetCurrentStatus()
        {
            return Redis.GetCurrentStatus(base.Url);
        }
    }


    public enum ItemValueType
    {
        TextValue,
        StateValue,
        TotalValue,
        ExpressionValue,
    }

    public class ComponentItem
    {
        public string GroupName { get; set; }

        public string Name { get; set; }

        public ItemValueType ItemValueType { get; set; }

        public ComponentItem()
        {
        }

        public ComponentItem(string name)
            : this("", name, ItemValueType.StateValue)
        {

        }

        public ComponentItem(string groupName, string name)
            : this(groupName, name, ItemValueType.StateValue)
        {

        }

        public ComponentItem(string groupName, string name, ItemValueType itemValueType)
        {
            GroupName = groupName;
            Name = name;
            ItemValueType = itemValueType;
        }
    }

    public abstract class ComponentConfiguration
    {
        public string Url { get; set; }

        public TimeSpan CollectSpan { get; set; }

        public Dictionary<string, TimeSpan> AggregateSpans { get; set; }

        internal Dictionary<string, ComponentItem> ComponentItems { get; set; }

        public ComponentConfiguration()
        {
            CollectSpan = TimeSpan.FromSeconds(10);
            AggregateSpans = new Dictionary<string, TimeSpan> 
            {
                { "2m", TimeSpan.FromMinutes(2) },
                { "20m", TimeSpan.FromMinutes(20) },
                { "1h", TimeSpan.FromHours(1) },
                { "3h", TimeSpan.FromHours(3) },
                { "12h", TimeSpan.FromHours(12) },
            };
        }

        public abstract Dictionary<string, object> GetCurrentStatus();
    }
}
