using System;
using System.IO;
using System.Security.Cryptography;

namespace SecuringApps.Presentation.Utilities
{
    public class Encryption
    {
        static string password = "alskdjflaskdjfals";
        static string salt = "alsdkjfalsdkfjalsdkasdfasdfasdffj";

        public static string SymmetricEncrypt(string str)
        {
            Rfc2898DeriveBytes myKeyGenerator = new Rfc2898DeriveBytes(password,
                 System.Text.Encoding.UTF8.GetBytes(salt));

            //key , iv

            Rijndael myAlg = Rijndael.Create();

            byte[] key = myKeyGenerator.GetBytes(myAlg.KeySize / 8);
            byte[] iv = myKeyGenerator.GetBytes(myAlg.BlockSize / 8);

            MemoryStream msIn = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(str)); //converting str >>> bytes
            msIn.Position = 0;


            CryptoStream cs = new CryptoStream(
                msIn,
                myAlg.CreateEncryptor(key, iv),
                 CryptoStreamMode.Read
                );

            MemoryStream msOut = new MemoryStream(); //this will be for my cipher
            cs.CopyTo(msOut);
            msOut.Position = 0; //pointer within the stream has been reset to position 0

            string output = Convert.ToBase64String(msOut.ToArray()); //converting bytes >> str
            return output;
        }

        public static string SymmetricDecrypt(string cipher)
        {
            Rfc2898DeriveBytes myKeyGenerator = new Rfc2898DeriveBytes(password,
                    System.Text.Encoding.UTF8.GetBytes(salt));

            //key , iv

            Rijndael myAlg = Rijndael.Create();

            byte[] key = myKeyGenerator.GetBytes(myAlg.KeySize / 8);
            byte[] iv = myKeyGenerator.GetBytes(myAlg.BlockSize / 8);

            MemoryStream msIn = new MemoryStream(Convert.FromBase64String(cipher)); //converting str (containing encrypted data) >>> bytes
            msIn.Position = 0;


            CryptoStream cs = new CryptoStream(
                msIn,
                myAlg.CreateDecryptor(key, iv),
                 CryptoStreamMode.Read
                );

            MemoryStream msOut = new MemoryStream(); //this will be for my cipher
            cs.CopyTo(msOut);
            msOut.Position = 0; //pointer within the stream has been reset to position 0

            string output = System.Text.Encoding.UTF8.GetString(msOut.ToArray()); //converting bytes >> str
            return output;


        }

    }
}
