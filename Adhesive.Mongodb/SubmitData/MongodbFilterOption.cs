
namespace Adhesive.Mongodb
{
    public enum MongodbFilterOption
    {
        /// <summary>
        /// 不作为过滤条件
        /// </summary>
        None = 0,
        /// <summary>
        /// 下拉框单选过滤
        /// </summary>
        DropDownListFilter = 1,
        /// <summary>
        /// 复选框多选过滤
        /// </summary>
        CheckBoxListFilter = 2,
        /// <summary>
        /// 文本框搜索过滤
        /// </summary>
        TextBoxFilter = 3,
    }
}
