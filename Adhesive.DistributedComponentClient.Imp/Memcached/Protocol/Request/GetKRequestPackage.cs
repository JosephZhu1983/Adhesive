
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class GetKRequestPackage : GetRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.GetK; }
        }

        internal GetKRequestPackage(string key)
            : base(key)
        {

        }
    }
}
