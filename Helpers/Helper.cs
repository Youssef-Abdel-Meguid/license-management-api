using System.Security.Cryptography;
using System.Text;

namespace GenerateEncryptedFile.Helpers
{
    public class Helper : IHelper
    {
        public string DecryptBytesToString(byte[] encryptedBytes, string password)
        {
            string decryptedString;

            using (Aes aes = Aes.Create())
            {
                byte[] salt = new byte[8];
                using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(aes.KeySize / 8);
                }

                byte[] iv = new byte[aes.BlockSize / 8];
                Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);

                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] decryptedBytes = memoryStream.ToArray();
                        decryptedString = Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }

            return decryptedString;
        }

        public byte[] EncryptStringToBytes(string plainText, string password)
        {
            byte[] encryptedBytes;
            byte[] iv;

            using (RijndaelManaged? rijndael = new RijndaelManaged())
            {
                byte[] salt = new byte[8];
                using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    byte[] keyBytes = keyDerivationFunction.GetBytes(32);
                    rijndael.Key = keyBytes;
                }

                rijndael.GenerateIV();
                byte[] key = rijndael.Key;
                iv = rijndael.IV;

                using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, iv))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            encryptedBytes = memoryStream.ToArray();
                        }
                    }
                }
            }

            byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, combinedBytes, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

            return combinedBytes;
        }
    }
}