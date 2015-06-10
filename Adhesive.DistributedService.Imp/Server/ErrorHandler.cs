

namespace Adhesive.DistributedService.Imp
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using Adhesive.AppInfoCenter;
    using Adhesive.AppInfoCenter.Imp;

    internal class ErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            try
            {
                if (OperationContext.Current.GetCurrentServiceDescription() != null)
                {
                    var logSetting = WcfSettingManager.CurrentServerSetting(OperationContext.Current.GetCurrentServiceDescription().ServiceType).WcfLogSetting;
                    if (logSetting.Enabled && logSetting.ExceptionInfoSetting.Enabled)
                    {
                        var exceptionID = "";
                        if (error.Data.Contains("id"))
                            exceptionID = error.Data["id"].ToString();
                        ((ExceptionService)AppInfoCenterService.ExceptionService).WcfUnhandledServerException(error,
                            OperationContext.Current.GetCurrentServiceDescription().ServiceType.FullName,
                            exceptionID, ServerApplicationContext.Current.RequestIdentity);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "ErrorHandler", "HandleError");
            }
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            try
            {
                var errorid = Guid.NewGuid().ToString();
                error.Data.Add("id", errorid);
                ServerApplicationContext.Current.ServerExceptionID = errorid;
                FaultException fe = new FaultException(new FaultReason(error.Message));
                MessageFault messagefault = fe.CreateMessageFault();
                fault = Message.CreateMessage(version, messagefault, "http://www.5173.com");
            }
            catch (Exception ex)
            {
                ex.Handle(WcfLogProvider.ModuleName, "ErrorHandler", "ProvideFault");
            }
        }
    }
}