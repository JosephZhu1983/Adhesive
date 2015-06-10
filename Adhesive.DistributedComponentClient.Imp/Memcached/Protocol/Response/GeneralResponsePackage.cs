
using System.Text;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Response
{
    internal class GeneralResponsePackage
    {
        internal Opcode Opcode { get; set; }

        internal ResponseStatus ResponseStatus { get; set; }

        internal string Key { get; set; }

        internal byte[] ValueBytes { get; set; }

        internal ulong Version { get; set; }

        internal string Value
        {
            get
            {
                if (ValueBytes != null)
                {
                    return Encoding.UTF8.GetString(ValueBytes);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
