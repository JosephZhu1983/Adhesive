using System.Linq;
using Adhesive.Alarm.Common;

using Adhesive.Common;
using Adhesive.Config;

namespace Adhesive.Alarm
{
    public class AlarmConfiguration
    {
        private static IConfigService configService = LocalServiceLocator.GetService<IConfigService>();

        internal static AlarmConfigurationItemByStatistics GetAlarmConfigurationItemByStatistics(string name)
        {
            return GetConfig().AlarmConfigurationByStatistics.Values.FirstOrDefault(c => c.ConfigName == name);
        }

        internal static AlarmConfigurationItemByState GetAlarmConfigurationItemByState(string name)
        {
            return GetConfig().AlarmConfigurationByStates.Values.FirstOrDefault(c => c.ConfigName == name);
        }

        internal static AlarmConfigurationItemBase GetAlarmConfigurationItem(string name)
        {
            return GetConfig().AlarmConfigurationByStatistics.Values.FirstOrDefault(c => c.ConfigName == name) as AlarmConfigurationItemBase ??
                GetConfig().AlarmConfigurationByStates.Values.FirstOrDefault(c => c.ConfigName == name) as AlarmConfigurationItemBase; 
        }

        internal static AlarmConfigurationEntity GetConfig()
        {
            var defaultConfig = AlarmConfigurationBase.GetDefaultConfig();
            var config = configService.GetConfigItemValue(true, "AlarmConfiguration", defaultConfig, update => AlarmService.Init());
            return config;
        }
    }
}
