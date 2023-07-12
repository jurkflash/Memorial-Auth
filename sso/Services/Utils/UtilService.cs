using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Sso.Services.Utils
{
    public static class UtilService
    {
        private static readonly string _passwordHash = "Kv,fBG?~{z2gemj#C4RsWEn!`:Fy8c'^9%H<5LS_};6P>]ks";
        private static readonly string _saltKey = "Ha@DK;W[+dmGwBv=~rjV48^7-tCn/a`zZqSb!NJ3g* Lc'.fu";
        private static readonly string _viKey = "UJY8X)5:ae^7uR_>";

        public static string Encrypt(string txt)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(txt + System.DateTime.Now.Ticks.ToString());

            byte[] keyBytes = new Rfc2898DeriveBytes(_passwordHash, Encoding.ASCII.GetBytes(_saltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(_viKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string txt)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(txt);
            byte[] keyBytes = new Rfc2898DeriveBytes(_passwordHash, Encoding.ASCII.GetBytes(_saltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(_viKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            string s = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            return s.Substring(0, s.Length - 18);
        }
    }
}
