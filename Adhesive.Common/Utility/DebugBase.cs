
using System.Web.Script.Serialization;

namespace Adhesive.Common
{
    public class DebugBase
    {
        public override string ToString()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            return s.Serialize(this);
        }
    }
}
