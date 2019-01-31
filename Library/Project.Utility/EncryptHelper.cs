using System.Text;

namespace System.Security.Cryptography
{
    public sealed class EncryptHelper
    {
        public static string EncryptByMd5(string plainText)
        {
            plainText = plainText.Trim();

            using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
            {
                byte[] hashedData = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                var sb = new StringBuilder();
                for (int i = 0; i < hashedData.Length; i++)
                {
                    sb.Append(hashedData[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string EncryptPassword(string plainText)
        {
            return plainText;
        }
    }
}
