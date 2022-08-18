
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
    public class ManageAlgorithm
    {
        private int CountCharacter = 32;

        //Encrypt Private Key and store into a Hidden file
        public string EncryptPrivateKey(AsymmetricCipherKeyPair key)
        {
            if (key == null)
            {
                return null;
            }
            var a = (RsaKeyParameters)key.Public;
            BigInteger modulus = a.Modulus;
            BigInteger exponent = a.Exponent;
            string result = modulus.ToString() + ";" + exponent.ToString();

            var b = (RsaKeyParameters)key.Private;
            BigInteger modulus2 = b.Modulus;
            BigInteger exponent2 = b.Exponent;
            string result2 = modulus2.ToString() + ";" + exponent2.ToString();
            return this.EncryptPrivateKey(result, result2);
        }

        private string EncryptPrivateKey(string publickey, string plainText)
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
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            string result = Convert.ToBase64String(array);
            this.writeToFile(key, result);
            return result;
        }
        
        public RsaKeyParameters DecryptPrivateKey(AsymmetricKeyParameter keyPublic)
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

        private RsaKeyParameters DecryptPrivateKey(string publickey_afterhash, string cipherText)
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
                            result =  streamReader.ReadToEnd();
                        }
                    }
                }
            }
            //Create Private Key from data
            String[] q = result.Split(';');

            BigInteger modulus = new BigInteger(q[0]);
            BigInteger exponent = new BigInteger(q[1]);
            RsaKeyParameters privateKey = new RsaKeyParameters(true,modulus,exponent);
            return privateKey;
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
        private void writeToFile(String key, String cipher)
        {
            string temporaryPath = Path.GetTempPath();
            string fileTempName = "71c4b1a70e48760f8ecb9686df55215c"; //Mobile-id MD5
            string path = "file/test.tmp";
            string result = key + ":" + cipher;
            File.AppendAllText(path, result + "\n");
            File.SetAttributes(path, FileAttributes.Hidden);
        }

        private string readFromFile(String key)
        {
            string temporaryPath = Path.GetTempPath();
            string fileTempName = "71c4b1a70e48760f8ecb9686df55215c"; //Mobile-id MD5
            string path = "file/test.tmp";
            string[] line = File.ReadAllLines(path);
            string temp = null;
            string result = null;
            for (int i = 0; i < line.Length; i++)
            {
                string[] row = line[i].Split(':');
                string keyOfRow = row[0];
                if (keyOfRow.Equals(key))
                {
                    result = row[1];
                    continue;
                }
                temp += line[i] += "\n";
            }
            if (result == null)
                return null;
            
            //Get object and delete row            
            File.Delete(path);
            File.AppendAllText(path, temp);
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            return result;
        }

       
    }
}
