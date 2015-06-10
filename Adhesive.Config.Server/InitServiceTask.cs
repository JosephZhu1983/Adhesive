﻿

using System;
using System.ServiceModel;
using Adhesive.Common;
using Microsoft.Practices.Unity;

namespace Adhesive.Config.Server
{
    public class InitServiceTask : BootstrapperTask
    {
        public override int Order
        {
            get
            {
                return 2;
            }
        }
        private static ServiceHost _host;
        public InitServiceTask(IUnityContainer container)
            : base(container)
        {
        }
        public override TaskContinuation Execute()
        {
            LocalLoggingService.Info("开始启动通用配置服务");
            try
            {
                _host = new ServiceHost(typeof(ConfigServer));
                _host.Open();
                LocalLoggingService.Info("完成启动通用配置服务");
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("通用配置服务启动失败，异常信息：{0}", ex);
                return TaskContinuation.Break;
            }
            return TaskContinuation.Continue;
        }
        protected override void InternalDispose()
        {
            LocalLoggingService.Info("开始结束通用配置服务");
            _host.Close();
        }
    }
}
