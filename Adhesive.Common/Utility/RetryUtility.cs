using System;
using System.Threading;

namespace Adhesive.Common
{
    /// <summary>
    /// 操作重试工具
    /// </summary>
    public static class RetryUtility
    {
        /// <summary>
        /// 重试方法
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">方法</param>
        /// <param name="numRetries">重试次数</param>
        /// <param name="retryTimeout">延时多长时间后重试，单位毫秒</param>
        /// <param name="throwIfFail">经过几轮重试操作后依然发生异常时是否将异常抛出</param>
        /// <param name="onFailureAction">操作失败执行的方法</param>
        /// <returns></returns>
        public static T RetryAction<T>(Func<T> action, int numRetries, int retryTimeout, bool throwIfFail, Action onFailureAction)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            T retVal = default(T);
            do
            {
                try
                {
                    retVal = action();
                    return retVal;
                }
                catch
                {
                    if (onFailureAction != null)
                        onFailureAction();
                    if (numRetries <= 0 && throwIfFail)
                        throw;
                    if (retryTimeout > 0)
                        Thread.Sleep(retryTimeout);
                }
            } while (numRetries-- > 0);
            return retVal;
        }
    }
}
