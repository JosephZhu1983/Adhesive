using System.Collections.Generic;
using Adhesive.Mongodb.Silverlight.Service;
using System.Runtime.Serialization;

namespace Adhesive.Mongodb.Silverlight
{
    public class Data
    {
        internal static string IP;

        internal static MongodbAdminConfigurationItem AdminConfigurationItem; //权限数据

        internal static List<Category> CategoryData; //分类数据
    }

    [KnownType(typeof(DetailCondition))]
    [KnownType(typeof(StateCondition))]
    [KnownType(typeof(SearchCondition))]
    public class UrlData
    {
        public string Type { get; set; }
        
        public object Condition { get; set; }
    }
}
