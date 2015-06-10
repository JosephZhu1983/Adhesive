
using System;

namespace Adhesive.Persistence.Imp
{
    /// <summary>
    /// GUID生成器，产生字符串表示的连续的GUID。
    /// </summary>
    public class SequentialGuidGenerator : IIdentityGenerator
    {
        public object Empty
        {
            get { return Guid.Empty.ToString("N"); }
        }

        public object NewId()
        {
            return IdentityGenerator.NewSequentialGuid().ToString("N");
        }
    }
}
