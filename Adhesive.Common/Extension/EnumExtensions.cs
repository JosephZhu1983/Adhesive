
using System;
using System.Diagnostics;

namespace Adhesive.Common
{
    public static class EnumExtensions
    {
        [DebuggerStepThrough]
        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return ((int)(object)type & (int)(object)value) == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        [DebuggerStepThrough]
        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        [DebuggerStepThrough]
        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type | (int)(object)value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("不能为枚举 '{0}' 添加值 '{1}'！", typeof(T).Name, value), ex);
            }
        }

        [DebuggerStepThrough]
        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type & ~(int)(object)value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("不能从枚举 '{0}' 移除值 '{1}'！", typeof(T).Name, value), ex);
            }
        }
    }
}
