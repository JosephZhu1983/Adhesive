
using System.Runtime.InteropServices;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Header
    {
        internal byte Magic;

        internal byte Opcode;

        internal ushort KeyLength;

        internal byte ExtraLength;

        internal byte DataType;

        internal ushort Reserved;

        internal uint TotalBodyLength;

        internal uint Opaque;

        internal ulong Version;
    }
}
