
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adhesive.Common;
using Adhesive.DistributedComponentClient.Utility;

namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Response
{
    internal class ResponsePackageCreator
    {
        private static Dictionary<Type, ResponsePackageAttribute> cmdTable = new Dictionary<Type, ResponsePackageAttribute>();

        static ResponsePackageCreator()
        {
            try
            {
                var assembly = typeof(ResponsePackageCreator).Assembly;
                var packages = assembly.GetTypes().Where(type => typeof(GeneralResponsePackage).IsAssignableFrom(type)).ToList();
                packages.ForEach(type =>
                {
                    var attribute = type.GetCustomAttributes(typeof(ResponsePackageAttribute), false).FirstOrDefault();
                    if (attribute != null)
                        cmdTable.Add(type, (ResponsePackageAttribute)attribute);
                });
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error(ex.ToString());
            }
        }

        internal static GeneralResponsePackage GetPackage(ClientSocket socket)
        {
            try
            {
                var headerBytes = socket.Read(24);
                if (headerBytes == null) return null;
                var header = headerBytes.BytesToStruct<Header>();
                var responseStatus = (ResponseStatus)header.Reserved;

                var extraBytes = header.ExtraLength > 0 ? socket.Read(header.ExtraLength) : null;
                var keyBytes = header.KeyLength > 0 ? socket.Read(header.KeyLength) : null;

                int valueCount = Convert.ToInt32(header.TotalBodyLength - header.KeyLength - header.ExtraLength);
                var valueBytes = valueCount > 0 ? socket.Read(valueCount) : null;

                Type t;
                List<KeyValuePair<Type, ResponsePackageAttribute>> pair = cmdTable.Where(item => Convert.ToByte(item.Value.Opcode) == header.Opcode).ToList();
                if (pair.Count > 0)
                {
                    t = pair.First().Key;
                }
                else
                {
                    t = typeof(GeneralResponsePackage);
                }
                var package = Activator.CreateInstance(t) as GeneralResponsePackage;
                if (keyBytes != null)
                    package.Key = Encoding.UTF8.GetString(keyBytes);
                package.ResponseStatus = responseStatus;
                package.Opcode = (Opcode)header.Opcode;
                package.ValueBytes = valueBytes;
                package.Version = header.Version;
                return package;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error(ex.ToString());
                return null;
            }
        }
    }
}
