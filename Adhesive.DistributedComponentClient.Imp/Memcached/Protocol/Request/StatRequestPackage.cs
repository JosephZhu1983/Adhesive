
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class StatRequestPackage : AbstractRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Stat; }
        }

        internal StatRequestPackage(string statTypeCode)
            : base(statTypeCode)
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
