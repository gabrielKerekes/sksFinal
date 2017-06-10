using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SksChat.Lib.Security.Encryption
{
    // https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider(v=vs.110).aspx
    public class SksRsa
    {
        public static byte[] RSAEncrypt(byte[] dataToEncrypt, RSAParameters rsaKeyInfo, bool doOAEPPadding)
        {
            //try
            //{
                byte[] encryptedData;

                using (var RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(rsaKeyInfo);
                    encryptedData = RSA.Encrypt(dataToEncrypt, doOAEPPadding);
                }

                return encryptedData;
            //}
            //catch (CryptographicException e)
            //{
            //    L(e.Message);

            //    return null;
            //}

        }

        public static byte[] RSADecrypt(byte[] dataToDecrypt, RSAParameters rsaKeyInfo, bool doOAEPPadding)
        {
            //try
            //{
                byte[] decryptedData;
                using (var RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(rsaKeyInfo);
                    decryptedData = RSA.Decrypt(dataToDecrypt, doOAEPPadding);
                }
                return decryptedData;
            //}
            //catch (CryptographicException e)
            //{
            //    Console.WriteLine(e.ToString());

            //    return null;
            //}

        }
    }
}
