

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Adhesive.AppInfoCenter;
    using Adhesive.Common;
    using Adhesive.Config;

    public class WcfSettingManager
    {
        private static Dictionary<Type, WcfSetting> wcfSettings = new Dictionary<Type, WcfSetting>();
        private static object locker = new object();
        private static Timer updateSettingTimer;

        static WcfSettingManager()
        {
            updateSettingTimer = new Timer(state =>
            {
                try
                {
                    lock (locker)
                    {
                        foreach (var key in wcfSettings.Keys.ToList())
                        {
                            var setting = GetWcfSetting(key);
                            if (setting != null)
                            {
                                wcfSettings[key] = setting;
                                //LocalLoggingService.Debug(string.Format("WcfSettingManager 更新了一次 {0} 的配置", key.FullName));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.Handle(WcfLogProvider.ModuleName, "WcfSettingManager", "WcfSettingManager", "WcfSettingManager的更新设置线程出错");
                }
            }, null, 60 * 1000, 60 * 1000);
        }

        public static WcfServerSetting CurrentServerSetting<T>()
        {
            return Current(typeof(T)) as WcfServerSetting;
        }

        public static WcfClientSetting CurrentClientSetting<T>()
        {
            return Current(typeof(T)) as WcfClientSetting;
        }

        public static WcfServerSetting CurrentServerSetting(Type type)
        {
            return Current(type) as WcfServerSetting;
        }

        public static WcfClientSetting CurrentClientSetting(Type type)
        {
            return Current(type) as WcfClientSetting;
        }

        private static WcfSetting Current<T>()
        {
            return Current(typeof(T));
        }

        private static WcfSetting Current(Type type)
        {
            WcfSetting setting = null;
            if (!wcfSettings.ContainsKey(type))
            {
                AppInfoCenterService.LoggingService.Warning(WcfLogProvider.ModuleName, "WcfSettingManager", "Current", 
                    string.Format("WcfSettingManager.Current 没有获取到 {0} 的配置", type.FullName));
                Init(type);

                if (type.IsInterface)
                {
                    setting = new WcfClientSetting
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
                                Enabled = false,
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
                }
                else
                {
                    setting = new WcfServerSetting
                    {
                        WcfCoreSetting = new WcfServerCoreSetting
                        {
                            EnableUnity = true,
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
                }
            }
            else
            {
                setting = wcfSettings[type] as WcfSetting;
            }
            return setting;
        }

        private static WcfSetting GetWcfSetting(Type type)
        {
            WcfSetting setting = null;
            try
            {
                if (type.IsInterface)
                {
                    using (var client = WcfServiceClientFactory.CreateServiceClient<IWcfConfigService>())
                    {
                        setting = client.Channel.GetClientSetting(type.FullName, CommonConfiguration.MachineIP);
                    }
                }
                else
                {
                    using (var client = WcfServiceClientFactory.CreateServiceClient<IWcfConfigService>())
                    {
                        setting = client.Channel.GetServerSetting(type.FullName, CommonConfiguration.MachineIP);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "WcfSettingManager", "GetWcfSetting");
            }
            return setting;
        }


        public static void Init(Type type)
        {
            lock (locker)
            {
                if (wcfSettings.ContainsKey(type))
                    wcfSettings.Remove(type);

                var setting = GetWcfSetting(type);
                if (setting != null)
                    wcfSettings.Add(type, setting);
            }
        }

        public static void Init<T>()
        {
            Init(typeof(T));
        }
    }
}