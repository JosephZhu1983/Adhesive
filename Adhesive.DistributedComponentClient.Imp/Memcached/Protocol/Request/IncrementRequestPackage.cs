
using System;
using System.Collections.Generic;
using Adhesive.DistributedComponentClient.Utility;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class IncrementRequestPackage : AbstractRequestPackage
    {
        private TimeSpan expireSpan;
        private ulong amount;
        private ulong seed;
        private ulong version;

        public override Opcode Opcode
        {
            get { return Opcode.Increment; }
        }

        internal IncrementRequestPackage(string key, ulong amount, ulong version)
            : this(key, amount, 0, TimeSpan.FromSeconds(UInt32.MaxValue), version)
        {

        }

        internal IncrementRequestPackage(string key, ulong amount, ulong seed, ulong version)
            : this(key, amount, seed, TimeSpan.FromDays(30), version)
        {

        }

        internal IncrementRequestPackage(string key, ulong amount, ulong seed, TimeSpan expireSpan, ulong version)
            : base(key)
        {
            if (expireSpan != TimeSpan.FromSeconds(UInt32.MaxValue) && expireSpan > TimeSpan.FromDays(30))
                throw new ArgumentOutOfRangeException("过期时间不能超过30天!");
            this.amount = amount;
            this.seed = seed;
            this.expireSpan = expireSpan;
            this.version = version;
        }

        protected override byte[] GetExtraBytes()
        {
            var extraBytes = new List<byte>();
            extraBytes.AddRange(amount.GetBigEndianBytes());
            extraBytes.AddRange(seed.GetBigEndianBytes());
            uint expire = Convert.ToUInt32(expireSpan.TotalSeconds);
            extraBytes.AddRange(expire.GetBigEndianBytes());
            return extraBytes.ToArray();
        }

        protected override ulong GetVersion()
        {
            return version;
        }

        protected override byte[] GetValueBytes()
        {
            return null;
        }
    }
}
