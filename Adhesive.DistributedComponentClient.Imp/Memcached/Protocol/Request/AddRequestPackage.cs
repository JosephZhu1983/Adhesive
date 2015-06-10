
using System;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class AddRequestPackage : SetRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Add; }
        }

        internal AddRequestPackage(string key, byte[] valueBytes, TimeSpan expireSpan, ulong version)
            : base(key, valueBytes, expireSpan, version)
        {
        }

        internal AddRequestPackage(string key, string value, TimeSpan expireSpan, ulong version)
            : base(key, value, expireSpan, version)
        {
        }

        internal AddRequestPackage(string key, string value, ulong version)
            : base(key, value, version)
        {
        }

        internal AddRequestPackage(string key, byte[] valueBytes, ulong version)
            : base(key, valueBytes, version)
        {
        }
    }
}
