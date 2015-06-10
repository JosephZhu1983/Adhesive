
namespace Adhesive.Mongodb
{
    public enum MongodbCascadeFilterOption
    {
        /// <summary>
        /// 不作为级联过滤
        /// </summary>
        None = 0,
        /// <summary>
        /// 级联过滤的第一级
        /// </summary>
        LevelOne = 1,
        /// <summary>
        /// 级联过滤的第二级
        /// </summary>
        LevelTwo = 2,
        /// <summary>
        /// 级联过滤的第三级
        /// </summary>
        LevelThree = 3,
    }
}
