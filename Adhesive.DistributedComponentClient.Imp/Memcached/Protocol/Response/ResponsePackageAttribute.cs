
using System;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Response
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ResponsePackageAttribute : Attribute
    {
        internal Opcode Opcode { get; private set; }

        internal ResponsePackageAttribute(Opcode opcode)
        {
            this.Opcode = opcode;
        }
    }
}
