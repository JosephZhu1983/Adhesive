
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class DeleteRequestPackage : GetRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.Delete; }
        }

        internal DeleteRequestPackage(string key)
            : base(key)
        {

        }
    }
}
