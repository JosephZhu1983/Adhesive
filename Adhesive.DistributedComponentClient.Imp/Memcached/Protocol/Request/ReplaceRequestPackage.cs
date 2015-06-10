
using System;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class ReplaceRequestPackage : SetRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Replace; }
        }

        internal ReplaceRequestPackage(string key, byte[] valueBytes, TimeSpan expireSpan, ulong version)
            : base(key, valueBytes, expireSpan, version)
        {
        }

        internal ReplaceRequestPackage(string key, string value, TimeSpan expireSpan, ulong version)
            : base(key, value, expireSpan, version)
        {
        }

        internal ReplaceRequestPackage(string key, string value, ulong version)
            : base(key, value, version)
        {
        }

        internal ReplaceRequestPackage(string key, byte[] valueBytes, ulong version)
            : base(key, valueBytes, version)
        {
        }
    }
}
