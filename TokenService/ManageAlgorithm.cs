using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    internal class ManageAlgorithm
    {     

        public  string EncryptPrivateKey(AsymmetricCipherKeyPair key)
        {
            if(key == null)
                return null;
            var a = (RsaKeyParameters)key.Public;
            BigInteger modulus = a.Modulus;            
            BigInteger exponent = a.Exponent;
            string result = modulus.ToString() +";" + exponent.ToString();
            //Console.WriteLine("resultPublic:" + result);

            var b = (RsaKeyParameters)key.Private;
            BigInteger modulus2 = b.Modulus;
            BigInteger exponent2 = b.Exponent;
            string result2 = modulus2.ToString() + ";" + exponent2.ToString();
            //Console.WriteLine("resultPrivate:" + result2);

            return this.EncryptPrivateKey(result, result2);
        }
        public  string EncryptPrivateKey(string publickey, string plainText)
        {
            string key = this.HashKey(publickey);
            Console.WriteLine("Key:" + key);
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public string DecryptPrivateKey(AsymmetricKeyParameter keyPublic, string cipherText)
        {
            var a = (RsaKeyParameters)keyPublic;
            BigInteger modulus = a.Modulus;
            BigInteger exponent = a.Exponent;
            string result = modulus.ToString() + ";" + exponent.ToString();
            //Console.WriteLine("resultPublic:" + result);

            return this.DecryptPrivateKey(result, cipherText);
        }
        public  string DecryptPrivateKey(string publickey, string cipherText)
        {
            string key = this.HashKey(publickey);
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

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
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private  string HashKey(String publickey)
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
    }
}
