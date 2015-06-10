
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace Adhesive.Common
{
    public static class JsonExtensions
    {
        private static JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

        [DebuggerStepThrough]
        public static string ObjectToJson(this object obj)
        {
            return javaScriptSerializer.Serialize(obj);
        }

        [DebuggerStepThrough]
        public static T JsonToObject<T>(this string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);
            return javaScriptSerializer.Deserialize<T>(json);
        }
    }
}
