

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
    /// ������ֵ����ö��
    /// </summary>
    public enum ValueTypeEnum
    {
        /// <summary>
        /// ��������
        /// </summary>
        Underlying = 1,
        /// <summary>
        /// �����б�
        /// </summary>
        List = 2,
        /// <summary>
        /// ���ⷺ���б��б��item������Ϊobject
        /// </summary>
        ObjectItemList = 3,
        /// <summary>
        /// �ֵ�
        /// </summary>
        Dictionary = 4,
        /// <summary>
        /// ���ⷺ���ֵ䣬�ֵ��item������Ϊobject
        /// </summary>
        ObjectItemDictionary = 5,
        /// <summary>
        /// �Զ���ʵ��
        /// </summary>
        Entity = 6
    }
    /// <summary>
    /// ���ð�����
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// ��ȡ�������ֵ�����͵��ַ��������ʽ
        /// </summary>
        /// <param name="valType">������ֵ����</param>
        /// <returns></returns>
        public static string GetConfigItemValueType(Type valType)
        {
          return valType.Assembly.GlobalAssemblyCache ? string.Format("{0}", valType.FullName) : string.Format("{0}, {1}", valType.FullName, valType.Assembly.GetName().Name);
        }
        /// <summary>
        /// ��ȡ�������ֵ�����͵�ö��
        /// </summary>
        /// <param name="valType">������ֵ����</param>
        /// <returns></returns>
        public static string GetConfigItemValueTypeEnum(Type valType)
        {
            //�б�
            if (typeof(IList).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length == 1 && valType.IsGenericType && genericArguments.First() == typeof(object))
                    return ValueTypeEnum.ObjectItemList.ToString();
                return ValueTypeEnum.List.ToString();
            }
            //�ֵ�
            if (typeof(IDictionary).IsAssignableFrom(valType))
            {
                var genericArguments = valType.GetGenericArguments();
                if (genericArguments.Length == 2 && genericArguments.First() == typeof(string) && valType.IsGenericType && genericArguments.Last() == typeof(object))
                    return ValueTypeEnum.ObjectItemDictionary.ToString();
                return ValueTypeEnum.Dictionary.ToString();
            }
            //��������
            if (IsUnderlyingType(valType))
            {
                return ValueTypeEnum.Underlying.ToString();
            }
            //�Զ�������ʵ��
            return ValueTypeEnum.Entity.ToString();
        }
        public static string GetValueTypeEnumFriendlyName(string valTypeEnum)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic[ValueTypeEnum.List.ToString()] = "�б�";
            dic[ValueTypeEnum.ObjectItemList.ToString()] = "�����б�";
            dic[ValueTypeEnum.Dictionary.ToString()] = "�ֵ�";
            dic[ValueTypeEnum.ObjectItemDictionary.ToString()] = "�����ֵ�";
            dic[ValueTypeEnum.Entity.ToString()] = "�Զ���ʵ��";
            dic[ValueTypeEnum.Underlying.ToString()] = "��������";
            if (valTypeEnum != null && dic.ContainsKey(valTypeEnum))
                return dic[valTypeEnum];
            return "��";
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
                LocalLoggingService.Warning("����ת��ʧ�ܣ�val:{0},valType:{1},defVal:{2},�쳣��Ϣ��{3}", val, valType, defVal, ex);
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
