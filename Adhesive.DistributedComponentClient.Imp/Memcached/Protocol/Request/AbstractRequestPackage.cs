
using System;
using System.IO;
using System.Text;
using Adhesive.Common;
using Adhesive.DistributedComponentClient.Utility;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Request
{
    internal abstract class AbstractRequestPackage
    {
        private Header header;
        private byte[] keyBytes;

        public abstract Opcode Opcode { get; }
        protected abstract byte[] GetExtraBytes();
        protected abstract byte[] GetValueBytes();

        protected virtual ulong GetVersion()
        {
            return 0;
        }

        internal AbstractRequestPackage(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                this.keyBytes = Encoding.UTF8.GetBytes(key);
                if (keyBytes.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException("key");
            }
            header = new Header
            {
                Magic = 0x80,
                Opcode = (byte)Opcode,
                KeyLength = keyBytes == null ? (ushort)0 : Convert.ToUInt16(keyBytes.Length),
            };
        }

        internal byte[] GetBytes()
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    var extraBytes = GetExtraBytes();
                    header.ExtraLength = extraBytes == null ? (byte)0 : Convert.ToByte(extraBytes.Length);
                    header.Version = GetVersion();
                    var valueBytes = GetValueBytes();
                    if (valueBytes != null && valueBytes.Length > 1024 * 1024)
                    {
                        LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "AbstractRequestPackage", "GetBytes",
                            string.Format("对于 {0} 操作的Value值太大：{1}，超过了1M", header.Opcode.ToString(), valueBytes.Length));
                        return null;
                    }
                    header.TotalBodyLength = Convert.ToUInt32(
                        (extraBytes == null ? 0 : extraBytes.Length) +
                        (keyBytes == null ? 0 : keyBytes.Length) +
                        (valueBytes == null ? 0 : valueBytes.Length));
                    var headerBytes = header.StructToBytes();
                    stream.Write(headerBytes, 0, headerBytes.Length);
                    if (extraBytes != null)
                        stream.Write(extraBytes, 0, extraBytes.Length);
                    if (keyBytes != null)
                        stream.Write(keyBytes, 0, keyBytes.Length);
                    if (valueBytes != null)
                        stream.Write(valueBytes, 0, valueBytes.Length);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error(ex.ToString());
                return null;
            }
        }
    }
}
