
using System;
using System.Linq;
using System.Web;
using Adhesive.Common;
using Adhesive.Mongodb;
using System.Text;

namespace Adhesive.AppInfoCenter.Imp
{
    public class ExceptionService : BaseService, IExceptionService
    {
        #region internal
        internal void AppDomainUnhandledException(Exception exception, bool isTerminating)
        {
            InternalHandle(new AppDomainUnhandledExceptionInfo { IsTerminating = isTerminating },
                exception,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null);
        }

        internal void WebSiteUnhandledException(Exception exception)
        {
            InternalHandle(new WebSiteUnhandledExceptionInfo(),
                exception,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null);
        }
        #endregion

        #region public

        public void MvcUnhandledException(Exception exception, string controllerTypeName)
        {
            InternalHandle(new MvcUnhandledExceptionInfo() { ControllerTypeName = controllerTypeName },
                exception,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null);
        }

        public void WcfUnhandledClientException(Exception exception, string contractName, string serverExceptionId, string identity)
        {
            InternalHandle(new WcfUnhandledClientExceptionInfo()
            {
                ContractName = contractName,
                ServerExceptionID = serverExceptionId,
                ContextIdentity = identity
            },
            exception,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
             null);
        }

        public void WcfUnhandledServerException(Exception exception, string serviceName, string serverExceptionId, string identity)
        {
            InternalHandle(new WcfUnhandledServerExceptionInfo()
            {
                ID = serverExceptionId,
                ServiceName = serviceName,
                ContextIdentity = identity
            },
            exception,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            null);
        }

        public void Handle(Exception exception)
        {
            HandledException(exception, string.Empty, string.Empty, string.Empty, string.Empty, null);
        }

        public void Handle(string moduleName, Exception exception)
        {
            HandledException(exception, moduleName, string.Empty, string.Empty, string.Empty, null);
        }

        public void Handle(string categoryName, string subcategoryName, Exception exception)
        {
            HandledException(exception, string.Empty, categoryName, subcategoryName, string.Empty, null);
        }

        public void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception)
        {
            HandledException(exception, moduleName, categoryName, subcategoryName, string.Empty, null);
        }

        public void Handle(Exception exception, string description)
        {
            HandledException(exception, string.Empty, string.Empty, string.Empty, description, null);
        }

        public void Handle(string moduleName, Exception exception, string description)
        {
            HandledException(exception, string.Empty, string.Empty, string.Empty, description, null);
        }

        public void Handle(string categoryName, string subcategoryName, Exception exception, string description)
        {
            HandledException(exception, string.Empty, categoryName, subcategoryName, description, null);
        }

        public void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception, string description)
        {
            HandledException(exception, moduleName, categoryName, subcategoryName, description, null);
        }


        public void Handle(Exception exception, ExtraInfo extraInfo)
        {
            HandledException(exception, string.Empty, string.Empty, string.Empty, string.Empty, extraInfo);
        }

        public void Handle(string moduleName, Exception exception, ExtraInfo extraInfo)
        {
            HandledException(exception, moduleName, string.Empty, string.Empty, string.Empty, extraInfo);
        }

        public void Handle(string categoryName, string subcategoryName, Exception exception, ExtraInfo extraInfo)
        {
            HandledException(exception, string.Empty, categoryName, subcategoryName, string.Empty, extraInfo);
        }

        public void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception, ExtraInfo extraInfo)
        {
            HandledException(exception, moduleName, categoryName, subcategoryName, string.Empty, extraInfo);
        }

        public void Handle(Exception exception, string description, ExtraInfo extraInfo)
        {
            HandledException(exception, string.Empty, string.Empty, string.Empty, description, extraInfo);
        }

        public void Handle(string moduleName, Exception exception, string description, ExtraInfo extraInfo)
        {
            HandledException(exception, moduleName, string.Empty, string.Empty, description, extraInfo);
        }

        public void Handle(string categoryName, string subcategoryName, Exception exception, string description, ExtraInfo extraInfo)
        {
            HandledException(exception, string.Empty, categoryName, subcategoryName, description, extraInfo);
        }

        public void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception, string description, ExtraInfo extraInfo)
        {
            HandledException(exception, moduleName, categoryName, subcategoryName, description, extraInfo);
        }

        #endregion

        #region private
        private AppException MapException(Exception ex)
        {
            if (ex == null) return null;
            var exception = new AppException
            {
                HelpLink = ex.HelpLink,
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                TargetSite = ex.TargetSite != null ? ex.TargetSite.ToString() : string.Empty,
                ExceptionTypeName = ex.GetType().FullName,
            };
            exception.InnerException = MapException(ex.InnerException);
            return exception;
        }

        private void LocalLog(ExceptionInfo info, Exception exception, string moduleName, string categoryName, string subcategoryName, string description, ExtraInfo extraInfo)
        {
            var message = new StringBuilder();
            if (info != null)
                message.Append(string.Format("异常类型：{0} ", info.GetType().Name));
            if (!string.IsNullOrEmpty(moduleName))
                message.Append(string.Format("模块名：{0} ", moduleName));
            if (!string.IsNullOrEmpty(categoryName))
                message.Append(string.Format("大类：{0} ", categoryName));
            if (!string.IsNullOrEmpty(subcategoryName))
                message.Append(string.Format("小类：{0} ", subcategoryName));
            if (!string.IsNullOrEmpty(description))
                message.Append(string.Format("描述：{0} ", description));
            if (extraInfo != null)
                message.Append(string.Format("额外信息：{0} ", extraInfo));
            message.Append(string.Format("异常信息：{0} ", exception.ToString()));
            LocalLoggingService.Error(message.ToString());
        }

        private ExceptionStrategy GetExceptionStrategy(string moduleName, string exceptionInfoTypeName, string exceptionTypeName)
        {
            var strategy = AppInfoCenterConfiguration.GetConfig().ExceptionServiceConfig.StrategyList.Values.FirstOrDefault
                (s => (s.ModuleName == moduleName || s.ModuleName == AbstractInfoProvider.DefaultModuleName)
                    && s.ExceptionInfoTypeName == exceptionInfoTypeName && (string.IsNullOrEmpty(s.ExceptionTypeName)
                    || s.ExceptionTypeName == exceptionTypeName));
            if (strategy == null)
            {
                if (AppInfoCenterService.ModuleName != moduleName)
                    AppInfoCenterService.LoggingService.Warning(AppInfoCenterService.ModuleName, ServiceName, "GetExceptionStrategy",
                        string.Format("没取到错误策略！参数：{0}，{1}，{2}", moduleName, exceptionInfoTypeName, exceptionTypeName));
                return new ExceptionStrategy();
            }
            return strategy;
        }

        private void InternalHandle(ExceptionInfo info, Exception exception, string moduleName, string categoryName, string subcategoryName, string description, ExtraInfo extraInfo)
        {
            if (exception == null)
                return;
            try
            {
                if (AppInfoCenterConfiguration.GetConfig().ExceptionServiceConfig.Enabled)
                {
                    var strategy = GetExceptionStrategy(moduleName, info.GetType().Name, exception.GetType().Name);
                    if (strategy != null)
                    {
                        if (strategy.LocalLog)
                            LocalLog(info, exception, moduleName, categoryName, subcategoryName, description, extraInfo);
                        if (strategy.RemoteLog)
                        {
                            info.Exception = MapException(exception);
                            info.Description = description;
                            info.ExceptionTypeName = exception.GetType().FullName;
                            info.ExtraInfo = extraInfo;
                            info.ModuleName = moduleName;
                            info.CategoryName = categoryName;
                            info.SubCategoryName = subcategoryName;
                            info.ExceptionMessage = exception.Message;
                            ProcessInfo(info);
                            MongodbService.MongodbInsertService.Insert(info);
                        }

                        if (info is WebSiteUnhandledExceptionInfo && HttpContext.Current != null)
                        {
                            HttpContext.Current.Response.Clear();

                            //HttpContext.Current.Response.StatusCode = strategy.ResponseStatusCode;

                            if (!HttpContext.Current.Request.IsLocal && strategy.ClearException)
                            {
                                HttpContext.Current.Server.ClearError();

                                if (!string.IsNullOrEmpty(strategy.RedirectUrl))
                                {
                                    HttpContext.Current.Response.Redirect(string.Format("{0}/?ID={1}", strategy.RedirectUrl.TrimEnd('/'), info.ID), false);
                                }
                                else
                                {
                                    HttpContext.Current.Response.Write(string.Format(AppInfoCenterConfiguration.GetConfig().ExceptionServiceConfig.UnhandledExceptionMessage, info.ID));
                                    HttpContext.Current.Response.End();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("InternalHandle出现错误，异常信息为：" + ex.ToString());
            }
        }

        private void HandledException(Exception exception, string moduleName, string categoryName, string subcategoryName, string description, ExtraInfo extraInfo)
        {
            InternalHandle(new HandledExceptionInfo(),
                exception, moduleName, categoryName, subcategoryName, description, extraInfo);
        }
        #endregion
    }
}
