
using System.Text;
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class PrependRequestPackage : AppendRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Prepend; }
        }

        internal PrependRequestPackage(string key, byte[] valueBytes)
            : base(key, valueBytes)
        {
        }

        internal PrependRequestPackage(string key, string value)
            : this(key, Encoding.UTF8.GetBytes(value))
        {
        }
    }
}
