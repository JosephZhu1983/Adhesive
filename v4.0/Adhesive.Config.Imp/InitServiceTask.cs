﻿
﻿using System;
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.Config.Imp
{
    public class InitServiceTask : InitServiceBootstrapperTask
    {
        public override int Order
        {
            get
            {
                return 3;
            }
        }
        private static IConfigService _configService;
        public InitServiceTask(IUnityContainer container)
            : base(container)
        {
        }
        public override TaskContinuation Execute()
        {
            _configService = LocalServiceLocator.GetService<IConfigService>();
            LocalLoggingService.Info("开始初始化配置服务");
            try
            {
                ((ConfigService)_configService).Init();
                LocalLoggingService.Info("结束初始化配置服务");
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("初始化配置服务失败，异常信息：{0}", ex);
                return TaskContinuation.Break;
            }
            return TaskContinuation.Continue;
        }
    }
}
