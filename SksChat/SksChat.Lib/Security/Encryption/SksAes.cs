using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SksChat.Lib.Security.Encryption
{
    public class SksAes
    {
        public string KdcLongTermKey { get; set; }

        public static byte[] EncryptBytes_Aes(byte[] plainText, byte[] key, byte[] iv)
        {
            byte[] encrypted;
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    return null;

                aesAlg.Padding = PaddingMode.PKCS7;

                aesAlg.Key = key;
                aesAlg.IV = iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                //using (var msEncrypt = new MemoryStream())
                //{
                //    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                //    {
                //        using (var swEncrypt = new StreamWriter(csEncrypt))
                //        {
                //            swEncrypt.Write(plainText, 0, plainText.Length);
                //        }

                //        encrypted = msEncrypt.ToArray();
                //    }
                //}
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(plainText, 0, plainText.Length);
                        cs.Close();
                    }
                    return ms.ToArray();
                }
            }

            //return encrypted;
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
        {
            CheckEncryptionArguments(plainText, key, iv);

            byte[] encrypted;
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    return null;

                aesAlg.Padding = PaddingMode.PKCS7;

                aesAlg.Key = key;
                aesAlg.IV = iv;
                
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            
            return encrypted;
        }

        private static void CheckEncryptionArguments(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
        }

        public static byte[] DecryptBytesFromBytes_Aes(byte[] cryptBytes, byte[] key, byte[] iv)
        {
            byte[] clearBytes = null;

            using (Aes aesAlg = new AesManaged())
            {
                aesAlg.Padding = PaddingMode.PKCS7;

                aesAlg.Key = key;
                aesAlg.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptBytes, 0, cryptBytes.Length);
                        cs.Close();
                    }
                    clearBytes = ms.ToArray();
                }
            }
            return clearBytes;
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            CheckDecryptionArguments(cipherText, key, iv);
            
            string plaintext;
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    return null;

                aesAlg.Padding = PaddingMode.PKCS7;

                aesAlg.Key = key;
                aesAlg.IV = iv;
                
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }

        private static void CheckDecryptionArguments(byte[] plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
        }
    }
}
