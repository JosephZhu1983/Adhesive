
using System;

namespace Adhesive.Mongodb
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public class MongodbPersistenceEntityAttribute : Attribute
    {
        private int expiredays;

        /// <summary>
        /// 过期时间（超过则删除）
        /// </summary>
        public int ExpireDays
        {
            get
            {
                return expiredays;
            }
            set
            {
                expiredays = value;
            }
        }

        private string name;
        /// <summary>
        /// 实体名
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value.Contains(".") || value.Contains("__"))
                    throw new ArgumentException("请确保参数不包含'.'和'__'!", "Name");

                name = value;
            }
        }

        /// <summary>
        /// 分类名
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }

        public MongodbPersistenceEntityAttribute(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new ArgumentException("无效的参数!", "categoryName");
            if (categoryName.Contains(".") || categoryName.Contains("__"))
                throw new ArgumentException("请确保参数不包含'.'和'__'!", "categoryName");
            CategoryName = categoryName;
            expiredays = -1;
        }
    }
}
