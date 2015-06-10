
using System;

namespace Adhesive.Mongodb
{
    public interface IMongodbInsertService : IDisposable
    {
        void Insert(object item);
    }
}
