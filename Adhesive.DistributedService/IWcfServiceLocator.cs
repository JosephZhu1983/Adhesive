namespace Adhesive.DistributedService
{
    public interface IWcfServiceLocator
    {
        /// <summary>
        /// 调用出错会抛异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>() where T : class;

        /// <summary>
        /// 调用出错不会抛异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSafeService<T>() where T : class;
    }
}
