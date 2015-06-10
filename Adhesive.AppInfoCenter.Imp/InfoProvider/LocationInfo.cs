
using Adhesive.Mongodb;
namespace Adhesive.AppInfoCenter.Imp
{
    public class LocationInfo
    {
        [MongodbPersistenceItem(ColumnName = "MTI")]
        [MongodbPresentationItem(DisplayName = "托管线程Id")]
        public int? ManagedThreadId { get; set; }

        [MongodbPersistenceItem(ColumnName = "ADN")]
        [MongodbPresentationItem(DisplayName = "应用程序域名")]
        public string AppDomainName { get; set; }

        [MongodbPersistenceItem(ColumnName = "AN")]
        [MongodbPresentationItem(DisplayName = "程序集名")]
        public string AssemblyName { get; set; }

        [MongodbPersistenceItem(ColumnName = "MN")]
        [MongodbPresentationItem(DisplayName = "模块名")]
        public string ModuleName { get; set; }

        [MongodbPersistenceItem(ColumnName = "TE")]
        [MongodbPresentationItem(DisplayName = "类型名")]
        public string TypeName { get; set; }

        [MongodbPersistenceItem(ColumnName = "METN")]
        [MongodbPresentationItem(DisplayName = "方法名")]
        public string MethodName { get; set; }

        [MongodbPersistenceItem(ColumnName = "ST")]
        [MongodbPresentationItem(DisplayName = "堆栈")]
        public string StackTrace { get; set; }
    }
}
