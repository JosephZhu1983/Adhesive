
using System;
using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;
using Adhesive.Common.FastReflection;

namespace Adhesive.AppInfoCenter.Imp
{
    public class BaseService : AbstractService
    {
        protected static List<IInfoProvider> infoProviderList = new List<IInfoProvider>();

        static BaseService()
        {
            infoProviderList.Add(new LocationInfoProvider());
            infoProviderList.Add(new EnvironmentInfoProvider());
            infoProviderList.Add(new HttpContextInfoProvider());
            infoProviderList.Add(new AbstractInfoProvider());
        }

        public static void RegisterExternalInfoProvider(IInfoProvider provider)
        {
            infoProviderList.Add(provider);
        }

        protected static void ProcessInfo(AbstractInfo info)
        {
            var strategy = GetIncludeInfoStrategy(info);
            infoProviderList.Each(provider =>
            {
                try
                {
                    if (strategy == null)
                        throw new Exception("ProcessInfo strategy == null");
                    if (info == null)
                        throw new Exception("ProcessInfo info == null");
                        
                    provider.ProcessInfo(strategy, info);
                    
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error(string.Format("处理数据出错！步骤名：{0}，异常信息：{1}", provider.GetType().Name, ex.ToString()));
                }
            });
        }


        private static IncludeInfoStrategy GetIncludeInfoStrategyByName(string name)
        {
            var strategy = AppInfoCenterConfiguration.GetConfig().IncludeInfoStrategys.Values.FirstOrDefault(s => s.Name == name);
            if (strategy == null)
            {
                //AppInfoCenterService.LoggingService.Warning(AppInfoCenterService.ModuleName, "BaseService", "GetIncludeInfoStrategyByName", string.Format("没取到名为 {0} 的包含信息策略！", name));
                return new IncludeInfoStrategy();
            }
            return strategy;
        }

        private static IncludeInfoStrategy GetIncludeInfoStrategy(object item)
        {
            var type = item.GetType();
            var allconfigs = AppInfoCenterConfiguration.GetConfig().IncludeInfoStrategyConfigurations;
            var configs = allconfigs.Values.Where(c => c.TypeFullName == type.FullName).ToList();
            if (configs.Count == 1)
            {
                var config = configs.First();
                return GetIncludeInfoStrategyByName(config.IncludeInfoStrategyName);
            }
            else
            {
                foreach (var config in configs.OrderByDescending(c => c.Conditions.Count))
                {
                    bool match = true;
                    foreach (var condition in config.Conditions)
                    {
                        if (condition.Value == null) continue;

                        var property = type.GetProperty(condition.Key);
                        if (property != null)
                        {
                            var value = property.FastGetValue(item);
                            if (value != null && condition.Value.ToString() != value.ToString())
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (match)
                    {
                        return GetIncludeInfoStrategyByName(config.IncludeInfoStrategyName);
                    }
                }
            }
            //AppInfoCenterService.LoggingService.Warning(AppInfoCenterService.ModuleName, "BaseService", string.Format("AdhesiveFramework.BaseService.GetIncludeInfoStrategy({0}) 没找到匹配的策略！", type.FullName));
            return new IncludeInfoStrategy();
        }
    }
}
