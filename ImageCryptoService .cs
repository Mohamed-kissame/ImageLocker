using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageLocker
{
    internal class ImageCryptoService
    {
        private const int SaltSize = 16;
        private const int IvSize = 16;


        public static void EncryptFile(string inputFile, string outputFile, string password) {

            byte[] salt = Helpers.GenerateSalt();
            byte[] iv = Helpers.GenerateIV();
            byte[] key = Helpers.DeriveKey(password, salt);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                {
                    
                    fsOut.Write(salt, 0, salt.Length);

                    
                    fsOut.Write(iv, 0, iv.Length);

                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(fsOut, encryptor, CryptoStreamMode.Write))
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        fsIn.CopyTo(cryptoStream);
                    }
                }
            }



        }

        public static void DecryptFile(string inputFile, string outputFile, string password) {


            using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
            {
               
                byte[] salt = new byte[SaltSize];
                fsIn.Read(salt, 0, SaltSize);

                
                byte[] iv = new byte[IvSize];
                fsIn.Read(iv, 0, IvSize);

                byte[] key = Helpers.DeriveKey(password, salt);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(fsIn, decryptor, CryptoStreamMode.Read))
                    using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                    {
                        cryptoStream.CopyTo(fsOut);
                    }
                }
            }

        }

    }
}
