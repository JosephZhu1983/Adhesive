

using System;
using System.Linq;
using Adhesive.Common;
using Adhesive.Mongodb;
using System.Text;

namespace Adhesive.AppInfoCenter.Imp
{
    public class LoggingService : BaseService, ILoggingService
    {
        public LoggingService()
        {
        }

        #region public

        public void Debug(string message)
        {
            InternalLog(LogLevel.Debug, string.Empty, string.Empty, string.Empty, message, null);
        }

        public void Debug(string moduleName, string message)
        {
            InternalLog(LogLevel.Debug, moduleName, string.Empty, string.Empty, message, null);
        }

        public void Debug(string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Debug, string.Empty, categoryName, subcategoryName, message, null);
        }

        public void Debug(string moduleName, string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Debug, moduleName, categoryName, subcategoryName, message, null);
        }

        public void Debug(string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Debug, message, string.Empty, string.Empty, message, extraInfo);
        }

        public void Debug(string moduleName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Debug, moduleName, string.Empty, string.Empty, message, extraInfo);
        }

        public void Debug(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Debug, string.Empty, categoryName, subcategoryName, message, extraInfo);
        }

        public void Debug(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Debug, moduleName, categoryName, subcategoryName, message, extraInfo);
        }


        public void Info(string message)
        {
            InternalLog(LogLevel.Info, string.Empty, string.Empty, string.Empty, message, null);
        }

        public void Info(string moduleName, string message)
        {
            InternalLog(LogLevel.Info, moduleName, string.Empty, string.Empty, message, null);
        }

        public void Info(string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Info, string.Empty, categoryName, subcategoryName, message, null);
        }

        public void Info(string moduleName, string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Info, moduleName, categoryName, subcategoryName, message, null);
        }

        public void Info(string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Info, message, string.Empty, string.Empty, message, extraInfo);
        }

        public void Info(string moduleName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Info, moduleName, string.Empty, string.Empty, message, extraInfo);
        }

        public void Info(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Info, string.Empty, categoryName, subcategoryName, message, extraInfo);
        }

        public void Info(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Info, moduleName, categoryName, subcategoryName, message, extraInfo);
        }


        public void Warning(string message)
        {
            InternalLog(LogLevel.Warning, string.Empty, string.Empty, string.Empty, message, null);
        }

        public void Warning(string moduleName, string message)
        {
            InternalLog(LogLevel.Warning, moduleName, string.Empty, string.Empty, message, null);
        }

        public void Warning(string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Warning, string.Empty, categoryName, subcategoryName, message, null);
        }

        public void Warning(string moduleName, string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Warning, moduleName, categoryName, subcategoryName, message, null);
        }

        public void Warning(string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Warning, message, string.Empty, string.Empty, message, extraInfo);
        }

        public void Warning(string moduleName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Warning, moduleName, string.Empty, string.Empty, message, extraInfo);
        }

        public void Warning(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Warning, string.Empty, categoryName, subcategoryName, message, extraInfo);
        }

        public void Warning(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Warning, moduleName, categoryName, subcategoryName, message, extraInfo);
        }


        public void Error(string message)
        {
            InternalLog(LogLevel.Error, string.Empty, string.Empty, string.Empty, message, null);
        }

        public void Error(string moduleName, string message)
        {
            InternalLog(LogLevel.Error, moduleName, string.Empty, string.Empty, message, null);
        }

        public void Error(string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Error, string.Empty, categoryName, subcategoryName, message, null);
        }

        public void Error(string moduleName, string categoryName, string subcategoryName, string message)
        {
            InternalLog(LogLevel.Error, moduleName, categoryName, subcategoryName, message, null);
        }

        public void Error(string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Error, message, string.Empty, string.Empty, message, extraInfo);
        }

        public void Error(string moduleName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Error, moduleName, string.Empty, string.Empty, message, extraInfo);
        }

        public void Error(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Error, string.Empty, categoryName, subcategoryName, message, extraInfo);
        }

        public void Error(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            InternalLog(LogLevel.Error, moduleName, categoryName, subcategoryName, message, extraInfo);
        }
        #endregion

        #region private
        private void LocalLog(LogLevel logLevel, string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            var msg = new StringBuilder();
            if (!string.IsNullOrEmpty(moduleName))
                msg.Append(string.Format("模块名：{0} ", moduleName));
            if (!string.IsNullOrEmpty(categoryName))
                msg.Append(string.Format("大类：{0} ", categoryName));
            if (!string.IsNullOrEmpty(subcategoryName))
                msg.Append(string.Format("小类：{0} ", subcategoryName));
            if (extraInfo != null)
                msg.Append(string.Format("额外信息：{0} ", extraInfo));
            msg.Append(string.Format("日志信息：{0} ", message));
            LocalLoggingService.Log(logLevel, msg.ToString());
        }

        private LogStrategy GetLogStrategy(string moduleName, LogLevel logLevel)
        {
            var strategy = AppInfoCenterConfiguration.GetConfig().LoggingServiceConfig.StrategyList.Values.FirstOrDefault(s => s.ModuleName == moduleName
                && s.LogLevel == logLevel);
            if (strategy == null)
                strategy = AppInfoCenterConfiguration.GetConfig().LoggingServiceConfig.StrategyList.Values.FirstOrDefault(s => s.ModuleName == AbstractInfoProvider.DefaultModuleName
               && s.LogLevel == logLevel);
            if (strategy == null)
            {
                if (AppInfoCenterService.ModuleName != moduleName)
                    AppInfoCenterService.LoggingService.Warning(AppInfoCenterService.ModuleName, ServiceName, "GetLogStrategy", string.Format("没取到日志策略！参数：{0}，{1}", moduleName, logLevel));
                return new LogStrategy();
            }
            return strategy;
        }

        private void InternalLog(LogLevel logLevel, string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo)
        {
            try
            {
                if (AppInfoCenterConfiguration.GetConfig().LoggingServiceConfig.Enabled)
                {
                    var strategy = GetLogStrategy(moduleName, logLevel);
                    if (strategy != null)
                    {
                        if (strategy.LocalLog)
                            LocalLog(logLevel, moduleName, categoryName, subcategoryName, message, extraInfo);
                        if (strategy.RemoteLog)
                        {
                            var info = new LogInfo();
                            info.LogLevel = logLevel;
                            info.ModuleName = moduleName;
                            info.CategoryName = categoryName;
                            info.SubCategoryName = subcategoryName;
                            info.Message = message;
                            info.ExtraInfo = extraInfo;
                            ProcessInfo(info);
                            MongodbService.MongodbInsertService.Insert(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("InternalLog出现错误，异常信息为：" + ex.ToString());
            }
        }
        #endregion
    }
}
