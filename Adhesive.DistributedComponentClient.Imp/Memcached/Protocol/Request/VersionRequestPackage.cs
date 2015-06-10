
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class VersionRequestPackage : AbstractRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Version; }
        }

        internal VersionRequestPackage()
            : base(null)
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
