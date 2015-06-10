using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adhesive.Persistence.Imp
{
    /// <summary>
    /// GUID生成器，产生字符串表示的GUID。
    /// </summary>
    public class GuidStringGenerator : IIdGenerator
    {
        public object NewId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
