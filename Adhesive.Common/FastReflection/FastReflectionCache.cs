using System.Collections.Generic;

namespace Adhesive.Common.FastReflection
{
    public abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue>
    {
        private Dictionary<TKey, TValue> m_cache = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key)
        {
            if (!m_cache.ContainsKey(key))
            {
                lock (m_cache)
                {
                    if (!m_cache.ContainsKey(key))
                    {
                        m_cache[key] = Create(key);
                    }
                }
            }

            return m_cache[key];
        }

        protected abstract TValue Create(TKey key);
    }
}
