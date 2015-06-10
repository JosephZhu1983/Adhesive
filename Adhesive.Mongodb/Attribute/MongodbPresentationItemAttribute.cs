
using System;

namespace Adhesive.Mongodb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MongodbPresentationItemAttribute : Attribute
    {
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 列描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否显示在列表视图中
        /// </summary>
        public bool ShowInTableView { get; set; }

        /// <summary>
        /// 过滤选项
        /// </summary>
        public MongodbFilterOption MongodbFilterOption { get; set; }

        /// <summary>
        /// 级联过滤选项
        /// </summary>
        public MongodbCascadeFilterOption MongodbCascadeFilterOption { get; set; }

        /// <summary>
        /// 排序选项
        /// </summary>
        public MongodbSortOption MongodbSortOption { get; set; }

        public MongodbPresentationItemAttribute()
        {
            MongodbFilterOption = MongodbFilterOption.None;
            MongodbCascadeFilterOption = MongodbCascadeFilterOption.None;
            MongodbSortOption = MongodbSortOption.None;
            ShowInTableView = false;
        }
    }
}
