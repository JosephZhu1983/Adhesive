
using System;
using System.Collections.Generic;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class ResponseInfo
    {
        [MongodbPersistenceItem(ColumnName = "CC")]
        [MongodbPresentationItem(DisplayName = "网页的缓存策略")]
        public string CacheControl { get; set; }

        [MongodbPersistenceItem(ColumnName = "CH")]
        [MongodbPresentationItem(DisplayName = "输出流的 HTTP 字符集")]
        public string Charset { get; set; }

        [MongodbPersistenceItem(ColumnName = "CE")]
        [MongodbPresentationItem(DisplayName = "输出流的 HTTP 字符集")]
        public string ContentEncoding { get; set; }

        [MongodbPersistenceItem(ColumnName = "CT")]
        [MongodbPresentationItem(DisplayName = "输出流的 HTTP MIME 类型")]
        public string ContentType { get; set; }

        [MongodbPersistenceItem(ColumnName = "C")]
        [MongodbPresentationItem(DisplayName = "响应 Cookie 集合")]
        public Dictionary<string, string> Cookies { get; set; }

        [MongodbPersistenceItem(ColumnName = "EA")]
        [MongodbPresentationItem(DisplayName = "从缓存中移除缓存信息的绝对日期和时间")]
        public DateTime? ExpiresAbsolute { get; set; }

        [MongodbPersistenceItem(ColumnName = "HE")]
        [MongodbPresentationItem(DisplayName = "当前标头输出流的编码")]
        public string HeaderEncoding { get; set; }

        [MongodbPersistenceItem(ColumnName = "IC")]
        [MongodbPresentationItem(DisplayName = "客户端是否仍连接在服务器上")]
        public bool? IsClientConnected { get; set; }

        [MongodbPersistenceItem(ColumnName = "IRB")]
        [MongodbPresentationItem(DisplayName = "客户端是否正在被传输到新的位置")]
        public bool? IsRequestBeingRedirected { get; set; }

        [MongodbPersistenceItem(ColumnName = "RL")]
        [MongodbPresentationItem(DisplayName = "Http“位置”标头的值")]
        public string RedirectLocation { get; set; }

        [MongodbPersistenceItem(ColumnName = "S")]
        [MongodbPresentationItem(DisplayName = "返回到客户端的 Status ")]
        public string Status { get; set; }

        [MongodbPersistenceItem(ColumnName = "SC")]
        [MongodbPresentationItem(DisplayName = "返回给客户端的输出的 HTTP 状态代码")]
        public int? StatusCode { get; set; }

        [MongodbPersistenceItem(ColumnName = "SD")]
        [MongodbPresentationItem(DisplayName = "返回给客户端的输出的 HTTP 状态字符串")]
        public string StatusDescription { get; set; }

        [MongodbPersistenceItem(ColumnName = "SCT")]
        [MongodbPresentationItem(DisplayName = "是否将 HTTP 内容发送到客户端")]
        public bool? SuppressContent { get; set; }

        [MongodbPersistenceItem(ColumnName = "TS")]
        [MongodbPresentationItem(DisplayName = "是否禁用 IIS 7.0 自定义错误")]
        public bool? TrySkipIisCustomErrors { get; set; }
    }

}
