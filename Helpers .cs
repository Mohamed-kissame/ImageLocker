using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageLocker
{
    internal class Helpers
    {


        public static byte[] GenerateSalt()
        {

            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {

                rng.GetBytes(salt);

            }

            return salt;
        }


        public static byte[] GenerateIV()
        {
           
            using (Aes aesAlg = Aes.Create())
            {
                return aesAlg.IV;
            }

           
        }

        public static byte[] DeriveKey(string password, byte[] salt)
        {

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations: 100000))
            {
                return pbkdf2.GetBytes(32);
            }

        }

    }
}
