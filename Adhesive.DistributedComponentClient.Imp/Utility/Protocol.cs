
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Adhesive.DistributedComponentClient.Utility
{
    internal static class Protocol
    {
        internal static byte[] GetBigEndianBytes(this ushort v)
        {
            return BitConverter.GetBytes(v.SwapEndian());
        }

        internal static byte[] GetBigEndianBytes(this uint v)
        {
            return BitConverter.GetBytes(v.SwapEndian());
        }

        internal static byte[] GetBigEndianBytes(this ulong v)
        {
            return BitConverter.GetBytes(v.SwapEndian());
        }

        internal static ushort GetLittleEndianUInt16(this byte[] data)
        {
            return data.GetLittleEndianUInt16(0);
        }

        internal static uint GetLittleEndianUInt32(this byte[] data)
        {
            return data.GetLittleEndianUInt32(0);
        }

        internal static ulong GetLittleEndianUInt64(this byte[] data)
        {
            return data.GetLittleEndianUInt64(0);
        }

        internal static ushort GetLittleEndianUInt16(this byte[] data, int index)
        {
            return BitConverter.ToUInt16(data, index).SwapEndian();
        }

        internal static uint GetLittleEndianUInt32(this byte[] data, int index)
        {
            return BitConverter.ToUInt32(data, index).SwapEndian();
        }

        internal static ulong GetLittleEndianUInt64(this byte[] data, int index)
        {
            return BitConverter.ToUInt64(data, index).SwapEndian();
        }

        internal static T BytesToStruct<T>(this byte[] rawData)
        {
            T result = default(T);
            RespectEndianness(typeof(T), rawData);
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
            }
            finally
            {
                handle.Free();
            }
            return result;
        }

        internal static byte[] StructToBytes<T>(this T data)
        {
            byte[] rawData = new byte[Marshal.SizeOf(data)];
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(data, rawDataPtr, false);
            }
            finally
            {
                handle.Free();
            }
            RespectEndianness(typeof(T), rawData);
            return rawData;
        }

        private static void RespectEndianness(Type type, byte[] data)
        {
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Select(field => new
            {
                Field = field,
                Offset = Marshal.OffsetOf(type, field.Name).ToInt32()
            }).ToList();

            fields.ForEach(item => Array.Reverse(data, item.Offset, Marshal.SizeOf(item.Field.FieldType)));
        }

        private static ushort SwapEndian(this ushort v)
        {
            return (ushort)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
        }

        private static uint SwapEndian(this uint v)
        {
            return (uint)(((SwapEndian((ushort)v) & 0xffff) << 0x10) |
                           (SwapEndian((ushort)(v >> 0x10)) & 0xffff));
        }

        internal static ulong SwapEndian(this ulong v)
        {
            return (ulong)(((SwapEndian((uint)v) & 0xffffffffL) << 0x20) |
                            (SwapEndian((uint)(v >> 0x20)) & 0xffffffffL));
        }
    }
}
