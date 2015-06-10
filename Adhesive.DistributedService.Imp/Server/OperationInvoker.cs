

using Adhesive.Common.Utility;

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using Adhesive.AppInfoCenter;
    using Adhesive.Mongodb;

    internal class OperationInvoker : IOperationInvoker
    {
        IOperationInvoker invoker;

        public OperationInvoker(IOperationInvoker oldInvoker)
        {
            invoker = oldInvoker;
        }

        public virtual object[] AllocateInputs()
        {
            return invoker.AllocateInputs();
        }

        protected void PostInvoke(object[] inputs, object returnedValue, object[] outputs, Exception exception, Stopwatch sw)
        {
            try
            {
                var results = new List<string>();
                if (returnedValue != null)
                    results.Add(returnedValue.ToString());
                results.AddRange(outputs.Select(o => o.ToString()).ToList());

                var logSetting = WcfSettingManager.CurrentServerSetting(OperationContext.Current.GetCurrentServiceDescription().ServiceType).WcfLogSetting;
                if (logSetting.Enabled && logSetting.InvokeInfoSetting.Enabled)
                {
                    var log = WcfLogProvider.GetServerInvokeInfo(
                    "OperationInvoker.PostInvoke",
                    sw.ElapsedMilliseconds,
                    exception == null ? true : false, IsSynchronous,
                    OperationContext.Current.GetCurrentOperationDescription().SyncMethod.Name,
                    ServerApplicationContext.Current,
                    inputs.Select(i =>
                        {
                            if (i == null)
                                return "null";
                            else
                                return i.ToString();
                        }).ToList(), results);
                    MongodbService.MongodbInsertService.Insert(log);
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "OperationInvoker", "PostInvoke");
            }
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            object returnedValue = null;
            Exception exception = null;

            Stopwatch sw = Stopwatch.StartNew();
            var p = WcfServiceHostFactory.GetWcfServerPerformanceService(OperationContext.Current.GetCurrentServiceDescription().ServiceType.FullName);
            var url = string.Format("{0}__{1}", OperationContext.Current.GetCurrentServiceDescription().ServiceType.FullName, OperationContext.Current.GetCurrentOperationDescription().SyncMethod.Name);
            try
            {               
                returnedValue = invoker.Invoke(instance, inputs, out outputs);
                if (p != null)
                {
                    p.EndInvoke(url, sw.ElapsedMilliseconds, true);
                }
            }
            catch (Exception ex)
            {
                if (p != null)
                {
                    p.EndInvoke(url, sw.ElapsedMilliseconds, false);
                }
                exception = ex;
                throw;
            }

            PostInvoke(inputs, returnedValue, outputs, exception, sw);

            return returnedValue;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            Stopwatch sw = Stopwatch.StartNew();
            return invoker.InvokeBegin(instance, inputs, callback, new Tuple<object[], Stopwatch>(inputs, sw));
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            object returnedValue = null;
            Exception exception = null;

            try
            {
                returnedValue = invoker.InvokeEnd(instance, out outputs, result);
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }

            PostInvoke((result.AsyncState as Tuple<object[], Stopwatch>).Item1, returnedValue, outputs, exception, (result.AsyncState as Tuple<object[], Stopwatch>).Item2);

            return returnedValue;
        }

        public bool IsSynchronous
        {
            get
            {
                return invoker.IsSynchronous;
            }
        }
    }
}
