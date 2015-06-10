
using System;
using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient.Memcached
{
    public partial class MemcachedClient : AbstractClient<MemcachedClient>
    {
        #region Flush
        public Dictionary<string, bool> Flush(TimeSpan expire)
        {
            return InternalFlush(expire);
        }

        public Dictionary<string, bool> Flush()
        {
            return InternalFlush(TimeSpan.MaxValue);
        }
        #endregion

        #region Version

        public Dictionary<string, string> Version()
        {
            return InternalVersion();
        }
        #endregion

        #region Stat

        public Dictionary<string, Dictionary<string, string>> Stat(StatType statType)
        {
            return InternalStat(statType);
        }

        public Dictionary<string, Dictionary<string, string>> Stat()
        {
            var dic = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in Stat(StatType.Setting))
            {
                dic.Add(item.Key, item.Value);
            }
            foreach (var item in Stat(StatType.General))
            {
                foreach (var subItem in item.Value)
                {
                    if (!dic[item.Key].ContainsKey(subItem.Key))
                        dic[item.Key].Add(subItem.Key, subItem.Value);
                }
            }
            foreach (var item in Stat(StatType.Item))
            {
                foreach (var subItem in item.Value)
                {
                    if (!dic[item.Key].ContainsKey(subItem.Key))
                        dic[item.Key].Add(subItem.Key, subItem.Value);
                }
            }
            return dic;
        }

        #endregion

        #region GetList

        public List<string> GetList(string listKey, int pageSize, int pageIndex)
        {
            var listItemKeys = Get<List<string>>(listKey);
            if (listItemKeys != null)
            {
                if (pageSize > 0)
                    listItemKeys = listItemKeys.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                var data = GetMultiple(listItemKeys);
                if (data != null)
                    return data.Select(item => item.Value).ToList();
            }
            return null;
        }

        public List<string> GetList(string listKey)
        {
            return GetList(listKey, -1, -1);
        }

        public List<T> GetList<T>(string listKey, int pageSize, int pageIndex)
        {
            var dic = GetList(listKey, pageSize, pageIndex);
            var objDic = dic.Select(item => item.JsonToObject<T>()).ToList();
            return objDic;
        }
        public List<T> GetList<T>(string listKey)
        {
            return GetList<T>(listKey, -1, -1);
        }

        #endregion

        #region SetListItem

        public bool SetListItem(string listKey, string itemKey, string itemValue)
        {
            return SetListItem(listKey, itemKey, itemValue, TimeSpan.MaxValue);
        }

        public bool SetListItem(string listKey, string itemKey, string itemValue, TimeSpan expire)
        {
            ulong version = 0;
            var maxTryCount = 100;

            var result = false;
            List<string> listItemKeys = null;
            var tryCount = 0;

            do
            {
                listItemKeys = Get<List<string>>(listKey, out version);
                if (listItemKeys == null)
                {
                    listItemKeys = new List<string>() { itemKey };
                    result = Add(listKey, listItemKeys, version);
                }
                if (!listItemKeys.Contains(itemKey))
                {
                    listItemKeys.Add(itemKey);
                    result = Replace(listKey, listItemKeys, version);
                }
                tryCount++;
                if (tryCount > maxTryCount)
                    return false;
            } while (!result);

            tryCount = 0;
            do
            {
                result = Set(itemKey, itemValue, expire);
                tryCount++;
                if (tryCount > maxTryCount)
                    return false;
            } while (!result);

            return true;
        }

        public bool SetListItem<T>(string listKey, string itemKey, T itemValue, TimeSpan expire)
        {
            return SetListItem(listKey, itemKey, itemValue.ObjectToJson(), expire);
        }

        public bool SetListItem<T>(string listKey, string itemKey, T itemValue)
        {
            return SetListItem<T>(listKey, itemKey, itemValue, TimeSpan.MaxValue);
        }

        #endregion

        #region DeleteListItem
        public bool DeleteListItem(string listKey, string itemKey)
        {
            var listItemKeys = Get<List<string>>(listKey);
            if (listItemKeys != null && listItemKeys.Remove(itemKey))
                Set(listKey, listItemKeys);
            return Delete(itemKey);
        }
        #endregion

        #region GetAndSet

        public string GetAndSet(string key, Func<string> getValue, TimeSpan expire)
        {
            var value = Get(key);
            if (value == null)
            {
                value = getValue();
                Set(key, value, expire);
            }
            return value;
        }

        public string GetAndSet(string key, Func<string> getValue)
        {
            return GetAndSet(key, getValue, TimeSpan.MaxValue);
        }

        public T GetAndSet<T>(string key, Func<T> getValue, TimeSpan expire)
        {
            ulong version;
            var value = Get<T>(key, out version);
            if (value == null)
            {
                value = getValue();
                if (value != null)
                {
                    Set<T>(key, value, expire, version);
                }
                else
                {
                    LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "GetAndSet", 
                        string.Format("GetAndSet回调方法没获取到值，Key为 {0} ", key));
                }
            }
            return value;
        }

        public T GetAndSet<T>(string key, Func<T> getValue)
        {
            return GetAndSet<T>(key, getValue, TimeSpan.MaxValue);
        }

        public string GetAndSetWhen(string key, Func<string> getValue, TimeSpan exipre, Func<bool> condition)
        {
            if (condition())
                return GetAndSet(key, getValue, exipre);
            else
                return getValue();
        }

        public string GetAndSetWhen(string key, Func<string> getValue, Func<bool> condition)
        {
            if (condition())
                return GetAndSet(key, getValue);
            else
                return getValue();
        }

        public T GetAndSetWhen<T>(string key, Func<T> getValue, Func<bool> condition)
        {
            if (condition())
                return GetAndSet<T>(key, getValue);
            else
                return getValue();
        }

        public T GetAndSetWhen<T>(string key, Func<T> getValue, TimeSpan expire, Func<bool> condition)
        {
            if (condition())
            {
                return GetAndSet<T>(key, getValue, expire);
            }
            else
            {
                return getValue();
            }
        }

        #endregion

        #region Get

        public string Get(string key)
        {
            ulong version = 0;
            return Get(key, out version);
        }

        public T Get<T>(string key)
        {
            ulong version = 0;
            return Get<T>(key, out version);
        }

        public string Get(string key, out ulong version)
        {
            return InternalGet(key, out version);
        }

        public T Get<T>(string key, out ulong version)
        {
            var json = Get(key, out version);
            try
            {
                return json.JsonToObject<T>();
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "MemcachedClient", "Get", 
                    string.Format("在反序列化对象的时候出现异常，Key：{0}，Value：{1}，类型：{2}，异常信息：{3}", key, json, typeof(T).GetType().FullName, ex.ToString()));
                Delete(key);
                return default(T);
            }
        }
        #endregion

        #region GetMultiple

        public Dictionary<string, string> GetMultiple(IList<string> keys)
        {
            return InternalGetMultiple(keys);
        }

        public Dictionary<string, T> GetMultiple<T>(IList<string> keys)
        {
            var dic = GetMultiple(keys);
            if (dic == null) return null;
            var objDic = dic.Where(item => item.Key != null && item.Value != null)
                .ToDictionary(item => item.Key, item => item.Value.JsonToObject<T>());
            return objDic;
        }

        #endregion

        #region Append

        public bool Append(string key, string value)
        {
            return InternalAppend(key, value);
        }

        #endregion

        #region Prepend

        public bool Prepend(string key, string value)
        {
            return InternalPrepend(key, value);
        }
        #endregion

        #region Delete

        public bool Delete(string key)
        {
            return InternalDelete(key);
        }

        public void FastDelete(string key)
        {
            InternalFastDelete(key);
        }

        #endregion

        #region Set

        public bool Set(string key, string value)
        {
            return InternalSet(key, value, TimeSpan.MaxValue, 0);
        }

        public bool Set(string key, string value, ulong version)
        {
            return InternalSet(key, value, TimeSpan.MaxValue, version);
        }

        public bool Set(string key, string value, TimeSpan expire)
        {
            return InternalSet(key, value, expire, 0);
        }

        public bool Set(string key, string value, TimeSpan expire, ulong version)
        {
            return InternalSet(key, value, expire, version);
        }

        public bool Set<T>(string key, T value)
        {
            return Set(key, value.ObjectToJson());
        }

        public bool Set<T>(string key, T value, ulong version)
        {
            return Set(key, value.ObjectToJson(), version);
        }

        public bool Set<T>(string key, T value, TimeSpan expire)
        {
            return Set(key, value.ObjectToJson(), expire);
        }

        public bool Set<T>(string key, T value, TimeSpan expire, ulong version)
        {
            return Set(key, value.ObjectToJson(), expire, version);
        }

        #endregion

        #region Replace
        public bool Replace(string key, string value)
        {
            return InternalReplace(key, value, TimeSpan.MaxValue, 0);
        }

        public bool Replace(string key, string value, ulong version)
        {
            return InternalReplace(key, value, TimeSpan.MaxValue, version);
        }

        public bool Replace(string key, string value, TimeSpan expire)
        {
            return InternalReplace(key, value, expire, 0);
        }

        public bool Replace(string key, string value, TimeSpan expire, ulong version)
        {
            return InternalReplace(key, value, expire, version);
        }

        public bool Replace<T>(string key, T value)
        {
            return Replace(key, value.ObjectToJson());
        }

        public bool Replace<T>(string key, T value, ulong version)
        {
            return Replace(key, value.ObjectToJson(), version);
        }

        public bool Replace<T>(string key, T value, TimeSpan expire)
        {
            return Replace(key, value.ObjectToJson(), expire);
        }

        public bool Replace<T>(string key, T value, TimeSpan expire, ulong version)
        {
            return Replace(key, value.ObjectToJson(), expire, version);
        }
        #endregion

        #region Exists

        public bool Exists(string key)
        {
            return this.Add(key, "");
        }

        #endregion

        #region Add

        public bool Add(string key, string value)
        {
            return InternalAdd(key, value, TimeSpan.MaxValue, 0);
        }

        public bool Add(string key, string value, ulong version)
        {
            return InternalAdd(key, value, TimeSpan.MaxValue, version);
        }

        public bool Add(string key, string value, TimeSpan expire)
        {
            return InternalAdd(key, value, expire, 0);
        }

        public bool Add(string key, string value, TimeSpan expire, ulong version)
        {
            return InternalAdd(key, value, expire, version);
        }

        public bool Add<T>(string key, T value)
        {
            return Add(key, value.ObjectToJson());
        }

        public bool Add<T>(string key, T value, ulong version)
        {
            return Add(key, value.ObjectToJson(), version);
        }

        public bool Add<T>(string key, T value, TimeSpan expire)
        {
            return Add(key, value.ObjectToJson(), expire);
        }

        public bool Add<T>(string key, T value, TimeSpan expire, ulong version)
        {
            return Add(key, value.ObjectToJson(), expire, version);
        }

        #endregion

        #region FastSet

        public void FastSet(string key, string value)
        {
            InternalFastSet(key, value, TimeSpan.MaxValue, 0);
        }

        public void FastSet(string key, string value, ulong version)
        {
            InternalFastSet(key, value, TimeSpan.MaxValue, version);
        }

        public void FastSet(string key, string value, TimeSpan expire)
        {
            InternalFastSet(key, value, expire, 0);
        }

        public void FastSet(string key, string value, TimeSpan expire, ulong version)
        {
            InternalFastSet(key, value, expire, version);
        }

        public void FastSet<T>(string key, T value)
        {
            FastSet(key, value.ObjectToJson());
        }

        public void FastSet<T>(string key, T value, ulong version)
        {
            FastSet(key, value.ObjectToJson(), version);
        }

        public void FastSet<T>(string key, T value, TimeSpan expire)
        {
            FastSet(key, value.ObjectToJson(), expire);
        }

        public void FastSet<T>(string key, T value, TimeSpan expire, ulong version)
        {
            FastSet(key, value.ObjectToJson(), expire, version);
        }
        #endregion

        #region Increment

        public ulong? Increment(string key, ulong amount)
        {
            return InternalIncrement(key, null, TimeSpan.MaxValue, amount, 0);
        }

        public ulong? Increment(string key, ulong amount, ulong version)
        {
            return InternalIncrement(key, null, TimeSpan.MaxValue, amount, version);
        }

        public ulong? IncrementWithInit(string key, ulong seed, TimeSpan expire, ulong amount)
        {
            return InternalIncrement(key, seed, expire, amount, 0);
        }

        public ulong? IncrementWithInit(string key, ulong seed, ulong amount)
        {
            return InternalIncrement(key, seed, TimeSpan.MaxValue, amount, 0);
        }

        public ulong? IncrementWithInit(string key, ulong seed, TimeSpan expire, ulong amount, ulong version)
        {
            return InternalIncrement(key, seed, expire, amount, version);
        }

        public ulong? IncrementWithInit(string key, ulong seed, ulong amount, ulong version)
        {
            return InternalIncrement(key, seed, TimeSpan.MaxValue, amount, version);
        }

        #endregion

        #region Decrement

        public ulong? Decrement(string key, ulong amount)
        {
            return InternalDecrement(key, null, TimeSpan.MaxValue, amount, 0);
        }

        public ulong? Decrement(string key, ulong amount, ulong version)
        {
            return InternalDecrement(key, null, TimeSpan.MaxValue, amount, version);
        }

        public ulong? DecrementWithInit(string key, ulong seed, TimeSpan expire, ulong amount)
        {
            return InternalDecrement(key, seed, expire, amount, 0);
        }

        public ulong? DecrementWithInit(string key, ulong seed, ulong amount)
        {
            return InternalDecrement(key, seed, TimeSpan.MaxValue, amount, 0);
        }

        public ulong? DecrementWithInit(string key, ulong seed, TimeSpan expire, ulong amount, ulong version)
        {
            return InternalDecrement(key, seed, expire, amount, version);
        }

        public ulong? DecrementWithInit(string key, ulong seed, ulong amount, ulong version)
        {
            return InternalDecrement(key, seed, TimeSpan.MaxValue, amount, version);
        }
        #endregion

        #region Locker

        public IDisposable AcquireLock(string key, TimeSpan timeOut)
        {
            return new MemcachedLocker(this, key, timeOut);
        }

        #endregion
    }
}
