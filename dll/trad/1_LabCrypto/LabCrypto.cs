using System;
using System.Security.Cryptography;
using System.Text;

namespace LabCrypto
{
    public static class Protector
    {
        private static readonly byte[] Key = DeriveKey("LabPassphrase-2026-Demo");

        private static byte[] DeriveKey(string s)
        {
            using (SHA256 h = SHA256.Create())
                return h.ComputeHash(Encoding.UTF8.GetBytes(s));
        }

        public static string Encrypt(string plain)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = Key;
                aes.GenerateIV();
                using (ICryptoTransform enc = aes.CreateEncryptor())
                {
                    byte[] data = Encoding.UTF8.GetBytes(plain);
                    byte[] cipher = enc.TransformFinalBlock(data, 0, data.Length);

                    byte[] all = new byte[aes.IV.Length + cipher.Length];
                    Buffer.BlockCopy(aes.IV, 0, all, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipher, 0, all, aes.IV.Length, cipher.Length);
                    return Convert.ToBase64String(all);
                }
            }
        }

        public static string Decrypt(string b64)
        {
            byte[] all = Convert.FromBase64String(b64);
            byte[] iv = new byte[16];
            byte[] cipher = new byte[all.Length - 16];
            Buffer.BlockCopy(all, 0, iv, 0, 16);
            Buffer.BlockCopy(all, 16, cipher, 0, cipher.Length);

            using (AesManaged aes = new AesManaged())
            {
                aes.Key = Key;
                aes.IV = iv;
                using (ICryptoTransform dec = aes.CreateDecryptor())
                {
                    byte[] plain = dec.TransformFinalBlock(cipher, 0, cipher.Length);
                    return Encoding.UTF8.GetString(plain);
                }
            }
        }
    }
}
