
using System.Collections.Generic;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class HttpContextInfo
    {
        [MongodbPersistenceItem(ColumnName = "H")]
        [MongodbPresentationItem(DisplayName = "处理程序名字")]
        public string Handler { get; set; }

        [MongodbPersistenceItem(ColumnName = "IT")]
        [MongodbPresentationItem(DisplayName = "上下文信息")]
        public Dictionary<string, string> Items { get; set; }

        [MongodbPersistenceItem(ColumnName = "SE")]
        [MongodbPresentationItem(DisplayName = "会话信息")]
        public Dictionary<string, string> Sessions { get; set; }

        [MongodbPersistenceItem(ColumnName = "CE")]
        [MongodbPresentationItem(DisplayName = "是否开启自定义错误")]
        public bool? IsCustomErrorEnabled { get; set; }

        [MongodbPersistenceItem(ColumnName = "DE")]
        [MongodbPresentationItem(DisplayName = "是否开启Debug")]
        public bool? IsDebuggingEnabled { get; set; }

        [MongodbPersistenceItem(ColumnName = "SA")]
        [MongodbPresentationItem(DisplayName = "是否跳过验证")]
        public bool? SkipAuthorization { get; set; }

        [MongodbPersistenceItem(ColumnName = "RES")]
        [MongodbPresentationItem(DisplayName = "响应信息")]
        public ResponseInfo ResponseInfo { get; set; }

        [MongodbPersistenceItem(ColumnName = "REQ")]
        [MongodbPresentationItem(DisplayName = "请求信息")]
        public RequestInfo RequestInfo { get; set; }
    }
}
