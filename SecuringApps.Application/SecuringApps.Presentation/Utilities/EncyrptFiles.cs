using System;
using System.IO;
using System.Security.Cryptography;

namespace SecuringApps.Presentation.Utilities
{

    public class EncyrptFiles
    {
        #region Key Generation
        public class KeyPair
        {
            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }
        }

        public static KeyPair GenerateNewKeyPair(int keySize = 4096)
        {
            // KeySize is measured in bits. 1024 is the default, 2048 is better, 4096 is more robust but takes a fair bit longer to generate.
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                return new KeyPair { PublicKey = rsa.ToXmlString(false), PrivateKey = rsa.ToXmlString(true) };
            }
        }

        #endregion
  
        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    // Fille the buffer with the generated data
                    rng.GetBytes(data);
                }
            }

            return data;
        }
             public static string FileEncrypt(string inputFile, string password)
              {

                  //generate random salt
                  byte[] salt = GenerateRandomSalt();

                  string fileName = inputFile + ".aes";
                  //create output file name
                  FileStream fsCrypt = new FileStream(fileName, FileMode.Create);

                  //convert password string to byte arrray
                  byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

                  //Set Rijndael symmetric encryption algorithm
                  RijndaelManaged AES = new RijndaelManaged();
                  AES.KeySize = 256;
                  AES.BlockSize = 128;
                  AES.Padding = PaddingMode.PKCS7;

                 var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
                  AES.Key = key.GetBytes(AES.KeySize / 8);
                  AES.IV = key.GetBytes(AES.BlockSize / 8);

                  // write salt to the begining of the output file, so in this case can be random every time
                  fsCrypt.Write(salt, 0, salt.Length);

                  CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

                  FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                  //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
                  byte[] buffer = new byte[1048576];
                  int read;

                  try
                  {
                      while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                      {
                          cs.Write(buffer, 0, read);
                      }

                      // Close up
                      fsIn.Close();
                  }
                  catch (Exception ex)
                  {
                      Console.WriteLine("Error: " + ex.Message);
                  }
                  finally
                  {
                      cs.Close();
                      fsCrypt.Close();
                  }
                  return fileName;
              }
        

   
        public static void FileDecrypt(string inputFile, string outputFile, string password)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];
            var fileName = inputFile.Substring(250);

            FileStream fsCrypt = new FileStream(inputFile.Substring(134), FileMode.Open);
            fsCrypt.Position = 0;

            fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);
          

            FileStream fsOut = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+@"\"+Guid.NewGuid()+".pdf", FileMode.Create);
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
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }

        }

        public static string DigitallySign(Stream input, string privateKey)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKey);

            byte[] digitalSignature = RSA.SignData(input, new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(digitalSignature);
        }

        public static bool DigitallyVerify(Stream input, string signature, string publicKeys)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKeys);

            byte[] signatureAsBytes = Convert.FromBase64String(signature);

            bool result = rsa.VerifyData(input, signatureAsBytes, new HashAlgorithmName("SHA512"), RSASignaturePadding.Pkcs1);
            return result;
        }

    }

}
