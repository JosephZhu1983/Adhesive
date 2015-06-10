
using System.Text;
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class AppendRequestPackage : AbstractRequestPackage
    {
        private byte[] valueBytes;

        public override Opcode Opcode
        {
            get { return Opcode.Append; }
        }

        internal AppendRequestPackage(string key, byte[] valueBytes)
            : base(key)
        {
            this.valueBytes = valueBytes;
        }

        internal AppendRequestPackage(string key, string value)
            : this(key, Encoding.UTF8.GetBytes(value))
        {
        }

        protected override byte[] GetExtraBytes()
        {
            return null;
        }

        protected override byte[] GetValueBytes()
        {
            return valueBytes;
        }
    }
}
