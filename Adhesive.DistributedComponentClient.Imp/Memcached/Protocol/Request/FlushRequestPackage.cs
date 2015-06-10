
using System;
using System.Collections.Generic;
using Adhesive.DistributedComponentClient.Utility;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class FlushRequestPackage : AbstractRequestPackage
    {
        private TimeSpan delaySpan;
        public override Opcode Opcode
        {
            get { return Opcode.Flush; }
        }

        internal FlushRequestPackage(TimeSpan delaySpan)
            : base(null)
        {
            this.delaySpan = delaySpan;
        }

        internal FlushRequestPackage()
            : this(TimeSpan.MaxValue)
        {
        }

        protected override byte[] GetExtraBytes()
        {
            var extraBytes = new List<byte>();
            if (delaySpan != TimeSpan.MaxValue)
            {
                uint expire = Convert.ToUInt32(delaySpan.TotalSeconds);
                extraBytes.AddRange(expire.GetBigEndianBytes());
            }
            return extraBytes.ToArray();
        }

        protected override byte[] GetValueBytes()
        {
            return null;
        }
    }
}
