
namespace Adhesive.DistributedComponentClient.Memcached.Protocol.Response
{
    internal enum ResponseStatus : ushort
    {
        NoError = 0x0000,
        KeyNotFound = 0x0001,
        KeyExists = 0x0002,
        ValueTooLarge = 0x0003,
        InvalidArguments = 0x0004,
        ItemNotStored = 0x0005,
        IncrDecrOnNonNnumericValue = 0x0006,
    }
}
