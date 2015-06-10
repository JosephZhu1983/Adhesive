
using System;

namespace Adhesive.Mongodb
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MongodbPersistenceItemAttribute : Attribute
    {
        /// <summary>
        /// 索引选项
        /// </summary>
        public MongodbIndexOption MongodbIndexOption { get; set; }

        /// <summary>
        /// 是否是主键列
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 列的值是否变为表名
        /// </summary>
        public bool IsTableName { get; set; }

        /// <summary>
        /// 是否忽略不保存到数据库中
        /// </summary>
        public bool IsIgnore { get; set; }

        /// <summary>
        /// 是否是时间列
        /// </summary>
        public bool IsTimeColumn { get; set; }

        /// <summary>
        /// 是否是跨库的上下文列
        /// </summary>
        public bool IsContextIdentityColumn { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        public MongodbPersistenceItemAttribute()
        {
            IsTableName = false;
            IsPrimaryKey = false;
            IsIgnore = false;
            MongodbIndexOption = MongodbIndexOption.None;
        }
    }
}
