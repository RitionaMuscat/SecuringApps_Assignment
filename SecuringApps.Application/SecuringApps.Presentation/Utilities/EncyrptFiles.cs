using System;
using System.IO;
using System.Security.Cryptography;

namespace SecuringApps.Presentation.Utilities
{
    public class EncyrptFiles
    {
        public class AsymmetricKeys
        {
            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }
        }
        public static byte[] Hash(byte[] originalData)
        {
            var myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(originalData);
            return digest;
        }
        public static AsymmetricKeys GenerateAsymmetricKey()
        {
            RSACryptoServiceProvider myAlg = new RSACryptoServiceProvider();

            AsymmetricKeys keys = new AsymmetricKeys()
            {
                PublicKey = myAlg.ToXmlString(false),
                PrivateKey = myAlg.ToXmlString(true)
            };
            return keys;
        }

        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    rng.GetBytes(data);
                }
            }

            return data;
        }
        public static string FileEncrypt(string inputFile, byte[] password)
        {

            string fileName = inputFile + ".aes";
            //create output file name
            FileStream fsCrypt = new FileStream(fileName, FileMode.Create);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            var key = new Rfc2898DeriveBytes(password, password, 50000);

            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            fsCrypt.Write(password, 0, password.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputFile, FileMode.Open);

            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cs.Write(buffer, 0, read);
                }
                //Close Streams
                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return fileName;
        }
        public static byte[] DigitalSign(string privateKey, MemoryStream dataToBeSigned)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            byte[] digest = Hash(dataToBeSigned.ToArray());

            byte[] signature = rsa.SignHash(digest, "SHA512");
            return signature;
        }

        public static bool VerifySignature(string publicKey, MemoryStream dataToBeVerified, byte[] signature)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            byte[] digest = Hash(dataToBeVerified.ToArray());

            bool result = rsa.VerifyHash(digest, "SHA512", signature);

            return result;
        }
        public static void FileDecrypt(string inputFile, string outputFile, byte[] password, string signature)
        {
            FileStream fsCrypt = new FileStream(inputFile.Substring(134), FileMode.Open);
            fsCrypt.Position = 0;

            fsCrypt.Read(password, 0, password.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(password, password, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + Guid.NewGuid() + ".pdf", FileMode.Create);
            fsOut.Position = 0;

            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            fsOut.Close();
            fsCrypt.Close();
        }
        public static string DigitallySign(byte[] hashValue)
        {
            byte[] signedHashValue;

            RSA rsa = RSA.Create();

            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            rsaFormatter.SetHashAlgorithm("SHA512");

            signedHashValue = rsaFormatter.CreateSignature(hashValue);
            return Convert.ToBase64String(signedHashValue);
        }

        public static bool DigitallyVerify(byte[] hashValue, byte[] signedHashValue)
        {
            RSA rsa = RSA.Create();
            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA512");
            if (rsaDeformatter.VerifySignature(hashValue, signedHashValue))
            {
                Console.WriteLine("The signature is valid.");
            }
            else
            {
                Console.WriteLine("The signature is not valid.");
            }
            return true;
        }
    }
}
