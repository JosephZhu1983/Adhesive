
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Adhesive.Persistence.Imp
{
    internal sealed class CryptoUtil
    {
        private static string _iv = "#kRz4rK^Z#kLMgQ^!ZHsY0u6";
        private static string _key = "S()%s@z^";
        /// <summary>          
        /// DES加密偏移量，必须是>=8位长的字符串          
        /// </summary>          
        public string IV
        {
            get { return _iv; }
            set { _iv = value; }
        }
        /// <summary>          
        /// DES加密的私钥，必须是8位长的字符串          
        /// </summary>          
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        /// <summary>          
        /// 对字符串进行DES加密          
        /// </summary>          
        /// <param name="sourceString">待加密的字符串</param>          
        /// <returns>加密后的BASE64编码的字符串</returns>          
        public static string Encrypt(string sourceString)
        {
            byte[] btKey = Encoding.Default.GetBytes(_key);
            byte[] btIV = Encoding.Default.GetBytes(_iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Encoding.Default.GetBytes(sourceString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>          
        /// 对DES加密后的字符串进行解密          
        /// </summary>          
        /// <param name="encryptedString">待解密的字符串</param>          
        /// <returns>解密后的字符串</returns>          
        public static string Decrypt(string encryptedString)
        {
            byte[] btKey = Encoding.Default.GetBytes(_key);
            byte[] btIV = Encoding.Default.GetBytes(_iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.Default.GetString(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
