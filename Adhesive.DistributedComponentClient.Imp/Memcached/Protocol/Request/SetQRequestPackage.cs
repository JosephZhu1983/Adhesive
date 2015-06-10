
using System;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class SetQRequestPackage : SetRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.SetQ; }
        }

        internal SetQRequestPackage(string key, byte[] valueBytes, TimeSpan expireSpan, ulong version)
            : base(key, valueBytes, expireSpan, version)
        {
        }

        internal SetQRequestPackage(string key, string value, TimeSpan expireSpan, ulong version)
            : base(key, value, expireSpan, version)
        {
        }

        internal SetQRequestPackage(string key, string value, ulong version)
            : base(key, value, version)
        {
        }

        internal SetQRequestPackage(string key, byte[] valueBytes, ulong version)
            : base(key, valueBytes, version)
        {
        }
    }
}
