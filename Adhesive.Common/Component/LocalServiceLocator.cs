
using System;
using Microsoft.Practices.Unity;

namespace Adhesive.Common
{
    public class LocalServiceLocator
    {
        public static T GetService<T>()
        {
            if (AdhesiveFramework.Status == AdhesiveFrameworkStatus.NotStarted)
            {
                throw new Exception("AdhesiveFramework尚未启动！");
            }
            else
            {
                try
                {
                    T instance = AdhesiveFramework.Container.Resolve<T>();
                    return instance;
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error("AdhesiveFramework.LocalServiceLocator 不能解析 '{0}'，异常信息：{1}", typeof(T).FullName, ex.ToString());
                    throw;
                }
            }
        }
    }
}
