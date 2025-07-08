using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;

namespace OctO.DAL
{
   public class EncryptDecrypt
    {
        public static string Decrypt(string cryptedString, string strKey)
        {
            if (string.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }
            if (string.IsNullOrEmpty(strKey))
            {
                throw new ArgumentNullException("The key can not be null.");
            }
            strKey = strKey + 3.1415926535897931.ToString();
            strKey = strKey.Substring(0, 8);
            byte[] bytes = Encoding.ASCII.GetBytes(strKey);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(stream2);
            return reader.ReadToEnd();
        }

        public static string Encrypt(string originalString, string strKey)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }
            if (string.IsNullOrEmpty(strKey))
            {
                throw new ArgumentNullException("The key can not be null.");
            }
            strKey = strKey + 3.1415926535897931.ToString();
            strKey = strKey.Substring(0, 8);
            byte[] bytes = Encoding.ASCII.GetBytes(strKey);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(stream2);
            writer.Write(originalString);
            writer.Flush();
            stream2.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
        }

        public static string MD5Encrypt(string originalPassword)
        {
            byte[] bytes = Encoding.Default.GetBytes(originalPassword);
            return Regex.Replace(BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)), "-", "");
        }
    }
}
