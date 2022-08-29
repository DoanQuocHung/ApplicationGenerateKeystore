
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    public class ManageAlgorithm
    {
        private int CountCharacter = 32;

        //Encrypt Private Key and store into a Hidden file
        public string EncryptPrivateKey(AsymmetricCipherKeyPair key)
        {
            if (key == null)
                return null;
            var a = (RsaKeyParameters)key.Public;
            BigInteger modulus = a.Modulus;
            BigInteger exponent = a.Exponent;
            string result = modulus.ToString() + ";" + exponent.ToString();

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)key.Private);
            rsaProvider.ImportParameters(rsaParams);
            return this.EncryptPrivateKey(result, rsaProvider);
        }

        private string EncryptPrivateKey(string publickey, RSACryptoServiceProvider rsaProvider)
        {
            string key = this.HashMD5(publickey);
            string keymain = this.concatKey_Key(key);

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(keymain);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {

                            var kp = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(rsaProvider);
                            using (var sw = new System.IO.StringWriter())
                            {
                                var pw = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                                pw.WriteObject(kp.Private);
                                var pem = sw.ToString();
                                streamWriter.Write(pem);
                            }
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            string result = Convert.ToBase64String(array);
            this.writeToFile(key, result);
            return result;
        }

        public AsymmetricCipherKeyPair DecryptPrivateKey(AsymmetricKeyParameter keyPublic)
        {
            //Get modulus + exponent and convert to string
            var a = (RsaKeyParameters)keyPublic;
            BigInteger modulus = a.Modulus;
            BigInteger exponent = a.Exponent;
            string result = modulus.ToString() + ";" + exponent.ToString();

            //Hash MD5 key
            string key = this.HashMD5(result);

            //Read from file and get cipherText
            string cipher_privatekey = this.readFromFile(key);

            return this.DecryptPrivateKey(key, cipher_privatekey);
        }

        private AsymmetricCipherKeyPair DecryptPrivateKey(string publickey_afterhash, string cipherText)
        {
            //Hash MD5 to get key            
            string key = this.concatKey_Key(publickey_afterhash);

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            String result = "";

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            //Get Private Key from data
            String path = "file/privateKey.pem";
            File.WriteAllText(path, result);
            //AsymmetricCipherKeyPair keyPair;
            //using (var reader = File.OpenText(path))
            //    keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
            //RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
            //File.Delete(path);
            return ImportPrivateKey2(path);
        }

        //Using for create hash of PublicKey
        private string HashMD5(String publickey)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(publickey);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string prior to .NET 5
                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        //Using for create PrivateKey of Backend
        private string SuperPrivate()
        {
            NPOI.Util.CRC32 crc32 = new NPOI.Util.CRC32();
            return crc32.StringCRC("Mobile-id").ToString("x2");
        }

        //Handle some business about create Key to en-decrypt file
        private string concatKey_Key(string key)
        {
            string privatekey = this.SuperPrivate();
            string temp = key.Remove(key.Length - 8, 8);
            temp += privatekey;
            return temp;
        }

        //Write to file Temp in System
        private void writeToFile(String name, String cipher)
        {
            string temporaryPath = Path.GetTempPath();
            string fileTempName = "71c4b1a70e48760f8ecb9686df55215c"; //Mobile-id MD5

            string path = "file/Private";

            if (!File.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            File.SetAttributes(path, FileAttributes.Hidden);
            File.WriteAllText(path + @"/" + name + ".tmp", cipher);
        }

        private string readFromFile(String key)
        {
            string temporaryPath = Path.GetTempPath();
            string fileTempName = "71c4b1a70e48760f8ecb9686df55215c"; //Mobile-id MD5

            string path = "file/Private/" + key + ".tmp";

            string line = File.ReadAllText(path);
            return line;
        }

        public AsymmetricCipherKeyPair ImportPrivateKey2(string path)
        {
            AsymmetricCipherKeyPair keyPair;

            using (var reader = File.OpenText(path)) // file containing RSA PKCS1 private key
                keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
            //return rsaParams;            
            File.Delete("file/privateKey.pem");
            return keyPair;
        }

        public RsaKeyParameters ImportPublicKey2(string path)
        {
            RsaKeyParameters keyPair;

            using (var reader = File.OpenText(path)) // file containing RSA PKCS1 private key
                keyPair = (RsaKeyParameters)new PemReader(reader).ReadObject();

            return keyPair;
        }
    }
}
