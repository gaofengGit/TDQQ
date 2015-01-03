
using System.Security.Cryptography;
using System.Text;

namespace TDQQ.License
{
    class Security
    {
        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="strText">待加密的字符串</param>
        /// <returns>加密后字符串</returns>
        public static string Md5Encrypt(string strText)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bytValue = Encoding.UTF8.GetBytes(strText);

            byte[] bytHash = md5.ComputeHash(bytValue);

            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToUpper();
        }
    }
}
