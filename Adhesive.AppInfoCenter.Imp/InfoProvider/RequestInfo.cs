
using System.Collections.Generic;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class RequestInfo
    {
        [MongodbPersistenceItem(ColumnName = "AT")]
        [MongodbPresentationItem(DisplayName = "客户端支持的 MIME 接受类型的字符串数组")]
        public string AcceptTypes { get; set; }

        [MongodbPersistenceItem(ColumnName = "AI")]
        [MongodbPresentationItem(DisplayName = "该用户的匿名标识符")]
        public string AnonymousID { get; set; }

        [MongodbPersistenceItem(ColumnName = "AP")]
        [MongodbPresentationItem(DisplayName = "服务器上 ASP.NET 应用程序的虚拟应用程序根路径")]
        public string ApplicationPath { get; set; }

        [MongodbPersistenceItem(ColumnName = "ARC")]
        [MongodbPresentationItem(DisplayName = "应用程序根的虚拟路径")]
        public string AppRelativeCurrentExecutionFilePath { get; set; }

        [MongodbPersistenceItem(ColumnName = "B")]
        [MongodbPresentationItem(DisplayName = "有关正在请求的客户端的浏览器功能的信息")]
        public string Browser { get; set; }

        [MongodbPersistenceItem(ColumnName = "CE")]
        [MongodbPresentationItem(DisplayName = "实体主体的字符集")]
        public string ContentEncoding { get; set; }

        [MongodbPersistenceItem(ColumnName = "CL")]
        [MongodbPresentationItem(DisplayName = "客户端发送的内容长度")]
        public int? ContentLength { get; set; }

        [MongodbPersistenceItem(ColumnName = "CT")]
        [MongodbPresentationItem(DisplayName = "传入请求的 MIME 内容类型")]
        public string ContentType { get; set; }

        [MongodbPersistenceItem(ColumnName = "C")]
        [MongodbPresentationItem(DisplayName = "客户端发送的 cookie 的集合")]
        public Dictionary<string, string> Cookies { get; set; }

        [MongodbPersistenceItem(ColumnName = "CEF")]
        [MongodbPresentationItem(DisplayName = "当前请求的虚拟路径")]
        public string CurrentExecutionFilePath { get; set; }

        [MongodbPersistenceItem(ColumnName = "FP")]
        [MongodbPresentationItem(DisplayName = "当前请求的虚拟路径")]
        public string FilePath { get; set; }

        [MongodbPersistenceItem(ColumnName = "F")]
        [MongodbPresentationItem(DisplayName = "窗体变量集合")]
        public Dictionary<string, string> Forms { get; set; }

        [MongodbPersistenceItem(ColumnName = "H")]
        [MongodbPresentationItem(DisplayName = "HTTP 头集合")]
        public Dictionary<string, string> Headers { get; set; }

        [MongodbPersistenceItem(ColumnName = "HM")]
        [MongodbPresentationItem(DisplayName = "客户端使用的 HTTP 数据传输方法")]
        public string HttpMethod { get; set; }

        [MongodbPersistenceItem(ColumnName = "IA")]
        [MongodbPresentationItem(DisplayName = "是否验证了请求")]
        public bool? IsAuthenticated { get; set; }

        [MongodbPersistenceItem(ColumnName = "IL")]
        [MongodbPresentationItem(DisplayName = "该请求是否来自本地计算机")]
        public bool? IsLocal { get; set; }

        [MongodbPersistenceItem(ColumnName = "ISC")]
        [MongodbPresentationItem(DisplayName = "HTTP 连接是否使用安全套接字")]
        public bool? IsSecureConnection { get; set; }

        [MongodbPersistenceItem(ColumnName = "LUI")]
        [MongodbPresentationItem(DisplayName = "当前用户的 WindowsIdentity 类型")]
        public string LogonUserIdentity { get; set; }

        [MongodbPersistenceItem(ColumnName = "P")]
        [MongodbPresentationItem(DisplayName = "当前请求的虚拟路径")]
        public string Path { get; set; }

        [MongodbPersistenceItem(ColumnName = "PI")]
        [MongodbPresentationItem(DisplayName = "具有 URL 扩展名的资源的附加路径信息")]
        public string PathInfo { get; set; }

        [MongodbPersistenceItem(ColumnName = "PA")]
        [MongodbPresentationItem(DisplayName = "当前正在执行的服务器应用程序的根目录的物理文件系统路径")]
        public string PhysicalApplicationPath { get; set; }

        [MongodbPersistenceItem(ColumnName = "PH")]
        [MongodbPresentationItem(DisplayName = "与请求的 URL 相对应的物理文件系统路径")]
        public string PhysicalPath { get; set; }

        [MongodbPersistenceItem(ColumnName = "QS")]
        [MongodbPresentationItem(DisplayName = "HTTP 查询字符串变量集合")]
        public Dictionary<string, string> QueryStrings { get; set; }

        [MongodbPersistenceItem(ColumnName = "RT")]
        [MongodbPresentationItem(DisplayName = "客户端使用的 HTTP 数据传输方法")]
        public string RequestType { get; set; }

        [MongodbPersistenceItem(ColumnName = "TB")]
        [MongodbPresentationItem(DisplayName = "当前输入流中的字节数")]
        public int? TotalBytes { get; set; }

        [MongodbPersistenceItem(ColumnName = "U")]
        [MongodbPresentationItem(DisplayName = "有关当前请求的 URL 的信息")]
        public string Url { get; set; }

        [MongodbPersistenceItem(ColumnName = "UR")]
        [MongodbPresentationItem(DisplayName = "有关客户端上次请求的 URL 的信息")]
        public string UrlReferrer { get; set; }

        [MongodbPersistenceItem(ColumnName = "UA")]
        [MongodbPresentationItem(DisplayName = "客户端浏览器的原始用户代理信息")]
        public string UserAgent { get; set; }

        [MongodbPersistenceItem(ColumnName = "UHA")]
        [MongodbPresentationItem(DisplayName = "远程客户端的 IP 主机地址")]
        public string UserHostAddress { get; set; }

        [MongodbPersistenceItem(ColumnName = "UHN")]
        [MongodbPresentationItem(DisplayName = "远程客户端的 DNS 名称")]
        public string UserHostName { get; set; }

        [MongodbPersistenceItem(ColumnName = "UL")]
        [MongodbPresentationItem(DisplayName = "客户端语言首选项的排序字符串")]
        public string UserLanguages { get; set; }
    }
}
