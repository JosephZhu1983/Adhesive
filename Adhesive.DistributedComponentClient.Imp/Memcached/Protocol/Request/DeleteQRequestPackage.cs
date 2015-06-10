
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal class DeleteQRequestPackage : GetRequestPackage
    {
        public override Opcode Opcode
        {
            get { return Opcode.DeleteQ; }
        }

        internal DeleteQRequestPackage(string key)
            : base(key)
        {

        }
    }
}
