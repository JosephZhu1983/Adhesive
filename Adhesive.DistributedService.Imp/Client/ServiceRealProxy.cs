

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using Adhesive.AppInfoCenter;
    using Adhesive.AppInfoCenter.Imp;
    using Adhesive.Common;
    using Adhesive.Config;
    using Adhesive.Mongodb;

    internal class ServiceRealProxy<T> : RealProxy where T : class
    {
        private bool safe;

        private WcfSetting WcfSetting
        {
            get
            {
                return WcfSettingManager.CurrentClientSetting<T>();
            }
        }

        private WcfLogSetting WcfLogSetting
        {
            get
            {
                return WcfSetting.WcfLogSetting;
            }
        }

        public ServiceRealProxy() : this(false) { }

        public ServiceRealProxy(bool safe)
            : base(typeof(T))
        {
            this.safe = safe;
        }

        public override IMessage Invoke(IMessage msg)
        {
            using (var client = WcfServiceClientFactory.CreateServiceClient<T>())
            {
                var channel = client.Channel;
                IMethodCallMessage methodCall = (IMethodCallMessage)msg;
                IMethodReturnMessage methodReturn = null;
                object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
                methodCall.Args.CopyTo(copiedArgs, 0);

                bool isSuccessuful = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    //WcfClientStateService.BeginInvoke(typeof(T).FullName);
                    object returnValue = methodCall.MethodBase.Invoke(channel, copiedArgs);

                    methodReturn = new ReturnMessage(returnValue,
                                                    copiedArgs,
                                                    copiedArgs.Length,
                                                    methodCall.LogicalCallContext,
                                                    methodCall);
                    isSuccessuful = true;
                    //WcfClientStateService.EndInvoke(typeof(T).FullName, stopwatch.ElapsedMilliseconds, true);
                }
                catch (Exception ex)
                {
                    try
                    {
                        //WcfClientStateService.EndInvoke(typeof(T).FullName, stopwatch.ElapsedMilliseconds, false);
                        var exception = ex;
                        if (ex.InnerException != null)
                            exception = ex.InnerException;
                        if (typeof(T) != typeof(IWcfConfigService) && typeof(T) != typeof(IConfigServer))
                        {
                            if (ClientApplicationContext.Current != null)
                            {
                                var exceptionID = ClientApplicationContext.Current.ServerExceptionID ?? "";
                                exception.HelpLink = "服务端异常Id：" + exceptionID;
                                if (WcfLogSetting.Enabled && WcfLogSetting.ExceptionInfoSetting.Enabled)
                                {
                                    ((ExceptionService)AppInfoCenterService.ExceptionService).WcfUnhandledClientException(exception, typeof(T).FullName,
                                        exceptionID, ClientApplicationContext.Current.RequestIdentity);
                                }
                            }
                            else
                            {
                                if (WcfLogSetting.Enabled && WcfLogSetting.ExceptionInfoSetting.Enabled)
                                {
                                    ((ExceptionService)AppInfoCenterService.ExceptionService).WcfUnhandledClientException(exception, typeof(T).FullName,
                                        "", "");
                                }
                            }

                        }

                        if (safe)
                        {
                            methodReturn = new ReturnMessage(null,
                                                         copiedArgs,
                                                         copiedArgs.Length,
                                                         methodCall.LogicalCallContext,
                                                         methodCall);

                        }
                        else
                        {
                            methodReturn = new ReturnMessage(exception, methodCall);
                        }
                    }
                    catch (Exception exx)
                    {
                        LocalLoggingService.Error("ServiceRealProxy.Invoke.catch出现异常：{0}", exx.ToString());
                    }
                }
                finally
                {
                    if (typeof(T) != typeof(IWcfConfigService) && typeof(T) != typeof(IConfigServer))
                    {
                        if (WcfLogSetting.Enabled && WcfLogSetting.InvokeInfoSetting.Enabled)
                        {
                            var log = WcfLogProvider.GetClientInvokeLog(
                               typeof(T).FullName,
                               "ServiceRealProxy.Invoke",
                               stopwatch.ElapsedMilliseconds,
                               isSuccessuful,
                               methodCall.MethodName,
                               ClientApplicationContext.Current);
                            MongodbService.MongodbInsertService.Insert(log);
                        }
                    }
                }
                return methodReturn;
            }
        }
    }
}
