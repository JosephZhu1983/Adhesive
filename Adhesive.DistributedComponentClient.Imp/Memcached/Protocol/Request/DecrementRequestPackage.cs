
using System;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class DecrementRequestPackage : IncrementRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Decrement; }
        }

        internal DecrementRequestPackage(string key, ulong amount, ulong version)
            : base(key, amount, version)
        {

        }

        internal DecrementRequestPackage(string key, ulong amount, ulong seed, ulong version)
            : base(key, amount, seed, version)
        {

        }

        internal DecrementRequestPackage(string key, ulong amount, ulong seed, TimeSpan expireSpan, ulong version)
            : base(key, amount, seed, expireSpan, version)
        {
        }
    }
}
