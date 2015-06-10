
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Adhesive.Common;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class StateService : AbstractService, IStateService
    {
        private Thread reportStateThread;
        private StateServiceConfiguration configuration;

        public void Init(StateServiceConfiguration configuration)
        {
            try
            {
                this.configuration = configuration;
                Init();
                LocalLoggingService.Debug("AdhesiveFramework.StateService 初始化 '{0}' 成功!", configuration.TypeFullName);
            }
            catch (Exception ex)
            {
                ex.Handle(AppInfoCenterService.ModuleName, ServiceName, "Init", string.Format("初始化 '{0}' 出错", configuration.TypeFullName));
            }
        }

        private void Init()
        {
            if (reportStateThread == null)
            {
                reportStateThread = new Thread(Report)
                {
                    Name = string.Format("{0}_{1}", ServiceName, configuration.TypeFullName),
                    IsBackground = true,
                };
                reportStateThread.Start();
            }
        }

        private StateServiceConfigurationItem GetStateServiceConfigurationItem(string typeFullName)
        {
            var allconfigs = AppInfoCenterConfiguration.GetConfig().StateServiceConfig.StateServiceConfigurationItems;
            var config = allconfigs.Values.FirstOrDefault(c => c.TypeFullName == typeFullName);
            if (config == null)
            {
                AppInfoCenterService.LoggingService.Warning(AppInfoCenterService.ModuleName, ServiceName, "GetStateServiceConfigurationItem", string.Format("没取到状态服务配置！参数：{0}", typeFullName));
                return new StateServiceConfigurationItem();
            }
            return config;
        }

        private void Report()
        {
            Thread.Sleep(10 * 1000);

            while (Enabled)
            {
                StateServiceConfigurationItem stateServiceConfig;
                try
                {
                    stateServiceConfig = GetStateServiceConfigurationItem(configuration.TypeFullName);
                    Thread.Sleep(Math.Max(stateServiceConfig.ReportStateIntervalMilliSeconds, 500));
                }
                catch (Exception ex)
                {
                    ex.Handle(AppInfoCenterService.ModuleName, ServiceName, "Report", "GetStateServiceConfigurationItem出错");
                    Thread.Sleep(10 * 1000);
                    continue;
                }
                IEnumerable<BaseInfo> states = null;
                if (stateServiceConfig != null && stateServiceConfig.Enabled)
                {
                    try
                    {
                        states = configuration.ReportStateFunc();
                    }
                    catch (Exception ex)
                    {
                        ex.Handle(AppInfoCenterService.ModuleName, ServiceName, "Report", string.Format("调用汇报状态回调方法出错，类型：{0}", configuration.TypeFullName));
                    }
                }

                if (states != null)
                {
                    states.Each(state =>
                    {
                        try
                        {
                            state.ID = Guid.NewGuid().ToString();
                            MongodbService.MongodbInsertService.Insert(state);

                        }
                        catch (Exception ex)
                        {
                            ex.Handle(AppInfoCenterService.ModuleName, ServiceName, "Report", string.Format("状态服务插入数据出错，类型：{0}", configuration.TypeFullName));
                        }
                    });
                }
            }
        }
    }
}
