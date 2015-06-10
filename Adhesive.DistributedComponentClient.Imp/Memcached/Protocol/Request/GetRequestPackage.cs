
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class GetRequestPackage : AbstractRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Get; }
        }

        internal GetRequestPackage(string key)
            : base(key)
        {

        }

        protected override byte[] GetExtraBytes()
        {
            return null;
        }

        protected override byte[] GetValueBytes()
        {
            return null;
        }
    }
}
