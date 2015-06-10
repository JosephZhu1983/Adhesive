

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Adhesive.Common;

namespace Adhesive.Config
{
    /// <summary>
    /// 配置项值类型枚举
    /// </summary>
    public enum ValueTypeEnum
    {
        /// <summary>
        /// 基础类型
        /// </summary>
        Underlying = 1,
        /// <summary>
        /// 泛型列表
        /// </summary>
        List = 2,
        /// <summary>
        /// 特殊泛型列表，列表的item的类型为object
        /// </summary>
        ObjectItemList = 3,
        /// <summary>
        /// 字典
        /// </summary>
        Dictionary = 4,
        /// <summary>
        /// 特殊泛型字典，字典的item的类型为object
        /// </summary>
        ObjectItemDictionary = 5,
        /// <summary>
        /// 自定义实体
        /// </summary>
        Entity = 6
    }
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 获取配置项的值的类型的字符串表达形式
        /// </summary>
        /// <param name="valType">配置项值类型</param>
        /// <returns></returns>
        public static string GetConfigItemValueType(Type valType)
        {
          return valType.Assembly.GlobalAssemblyCache ? string.Format("{0}", valType.FullName) : string.Format("{0}, {1}", valType.FullName, valType.Assembly.GetName().Name);
        }
        /// <summary>
        /// 获取配置项的值的类型的枚举
        /// </summary>
        /// <param name="valType">配置项值类型</param>
        /// <returns></returns>
        public static string GetConfigItemValueTypeEnum(Type valType)
        {
            //列表
            if (typeof(IList).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length == 1 && valType.IsGenericType && genericArguments.First() == typeof(object))
                    return ValueTypeEnum.ObjectItemList.ToString();
                return ValueTypeEnum.List.ToString();
            }
            //字典
            if (typeof(IDictionary).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length == 2 && genericArguments.First() == typeof(string) && valType.IsGenericType && genericArguments.Last() == typeof(object))
                    return ValueTypeEnum.ObjectItemDictionary.ToString();
                return ValueTypeEnum.Dictionary.ToString();
            }
            //基础类型
            if (IsUnderlyingType(valType))
            {
                return ValueTypeEnum.Underlying.ToString();
            }
            //自定义配置实体
            return ValueTypeEnum.Entity.ToString();
        }
        public static string GetValueTypeEnumFriendlyName(string valTypeEnum)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic[ValueTypeEnum.List.ToString()] = "列表";
            dic[ValueTypeEnum.ObjectItemList.ToString()] = "特殊列表";
            dic[ValueTypeEnum.Dictionary.ToString()] = "字典";
            dic[ValueTypeEnum.ObjectItemDictionary.ToString()] = "特殊字典";
            dic[ValueTypeEnum.Entity.ToString()] = "自定义实体";
            dic[ValueTypeEnum.Underlying.ToString()] = "基础类型";
            if (valTypeEnum != null && dic.ContainsKey(valTypeEnum))
                return dic[valTypeEnum];
            return "无";
        }
        public static object ChangeType(string val, Type valType, object defVal)
        {
            if (val == null)
                return defVal;
            if (valType == typeof(string))
                return val;
            if (valType == typeof(bool))
            {
                if (val.ToLower() == "true" || val == "1" || val == "on")
                    return true;
                return false;
            }
            if (valType.IsEnum)
            {
                try
                {
                    var flagsAttribute = valType.GetCustomAttributes(false).OfType<FlagsAttribute>().SingleOrDefault();
                    if (flagsAttribute != null)
                        return Enum.Parse(valType, val, true);
                    else if(Enum.GetNames(valType).Select(e => e.ToLower()).Contains(val.ToLower()) ||
                        Enum.GetValues(valType).Cast<int>().Select(e => e.ToString()).Contains(val))
                        return Enum.Parse(valType, val, true);
                    return defVal;
                }
                catch (Exception ex)
                {
                    return defVal;
                }
            }
            if (valType == typeof(Type))
            {
                try
                {
                    return Type.GetType(val);
                }
                catch (Exception ex)
                {
                    return defVal;
                }
            }
            if (valType == typeof(XmlDocument))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(val);
                    return xmlDoc;
                }
                catch (Exception ex)
                {
                    return defVal;
                }
            }
            if (valType == typeof(TimeSpan))
            {
                try
                {
                    return TimeSpan.Parse(val);
                }
                catch (Exception ex)
                {
                    return defVal;
                }
            }
            if (valType == typeof(TimeSpanEx))
            {
                try
                {
                    return (TimeSpanEx)TimeSpan.Parse(val);
                }
                catch (Exception ex)
                {
                    return defVal;
                }
            }
            try
            {
                return Convert.ChangeType(val, valType);
            }
            catch (Exception ex)
            {
                LocalLoggingService.Warning("类型转换失败！val:{0},valType:{1},defVal:{2},异常信息：{3}", val, valType, defVal, ex);
                return defVal;
            }
        }
        public static string GenerateConfigItemId(string appName, params string[] pathItemNames)
        {
            if (pathItemNames == null)
                throw new ArgumentNullException("pathItemNames");
            StringBuilder sb = new StringBuilder();
            sb.Append(appName);
            foreach (string itemName in pathItemNames)
            {
                sb.AppendFormat(".{0}", itemName);
            }
            return ComputeHash(sb.ToString());
        }

        public static string ComputeHash(string input)
        {
            StringBuilder sb = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
        public static bool IsUnderlyingType(Type type)
        {
            if (type.IsValueType || type == typeof(string))
                return true;
            return false;
        }
        public static bool IsCompositeValue(Type valType)
        {
            FlagsAttribute flagsAttribute = valType.GetCustomAttributes(false).OfType<FlagsAttribute>().SingleOrDefault();
            if (flagsAttribute != null)
                return true;
            return false;
        }
    }
}
