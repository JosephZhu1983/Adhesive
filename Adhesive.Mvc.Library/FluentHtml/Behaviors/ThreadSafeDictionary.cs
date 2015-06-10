using System;
using System.Collections.Generic;
using System.Threading;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    public class ThreadSafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public TValue GetOrAdd(TKey key, Func<TValue> defaultValueDelegate)
        {
            cacheLock.EnterReadLock();
            try
            {
                if (ContainsKey(key))
                {
                    return this[key];
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
            cacheLock.EnterWriteLock();
            try
            {
                if (!ContainsKey(key))
                {
                    Add(key, defaultValueDelegate());
                }
                return this[key];
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }
    }
}