
namespace Adhesive.Mongodb
{
    public enum MongodbIndexOption
    {
        /// <summary>
        /// 不作索引
        /// </summary>
        None = 0,
        /// <summary>
        /// 递增索引
        /// </summary>
        Ascending = 1,
        /// <summary>
        /// 递减索引
        /// </summary>
        Descending = 2,
        /// <summary>
        /// 唯一的递增索引
        /// </summary>
        AscendingAndUnique = 3,
        /// <summary>
        /// 唯一的递减索引
        /// </summary>
        DescendingAndUnique = 4,
    }
}
