
using System;
using System.Collections.Generic;
using System.Text;
using Adhesive.DistributedComponentClient.Utility;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class SetRequestPackage : AbstractRequestPackage
    {
        private TimeSpan expireSpan;
        private byte[] valueBytes;
        private ulong version;

        public override Opcode Opcode
        {
            get { return Opcode.Set; }
        }

        internal SetRequestPackage(string key, byte[] valueBytes, TimeSpan expireSpan, ulong version)
            : base(key)
        {
            if (expireSpan > TimeSpan.FromDays(30))
                throw new ArgumentOutOfRangeException("过期时间不能超过30天!");
            this.expireSpan = expireSpan;
            this.valueBytes = valueBytes;
            this.version = version;
        }

        internal SetRequestPackage(string key, string value, TimeSpan expireSpan, ulong version)
            : this(key, Encoding.UTF8.GetBytes(value), expireSpan, version)
        {
        }

        internal SetRequestPackage(string key, string value, ulong version)
            : this(key, Encoding.UTF8.GetBytes(value), TimeSpan.FromDays(30), version)
        {
        }

        internal SetRequestPackage(string key, byte[] valueBytes, ulong version)
            : this(key, valueBytes, TimeSpan.FromDays(30), version)
        {
        }

        protected override ulong GetVersion()
        {
            return version;
        }

        protected override byte[] GetExtraBytes()
        {
            var extraBytes = new List<byte>();
            uint flag = 0xdeadbeef;
            extraBytes.AddRange(flag.GetBigEndianBytes());
            uint expire = Convert.ToUInt32(expireSpan.TotalSeconds);
            extraBytes.AddRange(expire.GetBigEndianBytes());
            return extraBytes.ToArray();
        }

        protected override byte[] GetValueBytes()
        {
            return valueBytes;
        }
    }
}
