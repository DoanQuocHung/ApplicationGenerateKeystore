using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===== Sinh khóa RSA và sinh CSR =====");
            ManageKey generateKeypair = new ManageKey();
            string subjectDN = generateKeypair.createInformation();
            generateKeypair.generateKey(2048);
            //generateKeypair.getPrivateKey();
            string CSR = generateKeypair.generateCSR(subjectDN, null);
            Console.WriteLine("Generated CSR Successfully !");

            //Test en - decrypt private key
            ManageAlgorithm a = new ManageAlgorithm();
            string result = a.EncryptPrivateKey(generateKeypair.getKey());
            Console.WriteLine("Encrypt:"+result);
            string privatekey = a.DecryptPrivateKey(generateKeypair.getPublicKey(), result);
            Console.WriteLine("\nPrivatekey:"+privatekey);

            //Console.WriteLine("\n===== Export Excel File =====");
            //WriteExcelFile writeExcelFile = new WriteExcelFile();
            ////Cipher privateKey
            ////ManageAlgorithm manageAlgorithm = new ManageAlgorithm();
            ////string cipher = manageAlgorithm.EncryptPrivateKey(generateKeypair.getPublicKey().ToString())
            //writeExcelFile.ExportExcel(CSR,null);
            //Console.WriteLine("Export Excel Successfully !");

            //String path = @"C:\Users\gia\Desktop\cert.cer";
            //X509Certificate certificate = Utils.readCertificate(path);            
            //Console.WriteLine("SubjectDN"+certificate.SubjectDN);
            //String base64 = Utils.convertCertToBase64(certificate);
            //Console.WriteLine("Base64:"+base64);
            //ManageAlgorithm algorithm = new ManageAlgorithm();
            //String input = "Tat Khanh Giaajlwdh kagoqje lkqhjkdcndHQOIW Dadm nUQIYHD HJKASNK ioqw d";
            //String key2 = "alo mot hai ba basddasdn nam sau bay tam chin muoiw";

            //String encrypt = algorithm.EncryptString(key2, input);
            //String decrypt = algorithm.DecryptString(key2, encrypt);
            //Console.WriteLine("Input:" + input);
            //Console.WriteLine("Encrypt:" + encrypt);
            //Console.WriteLine("Decrypt:" + decrypt);


            //Console.WriteLine("\n===== Generate PKCS12 Keystore Successfully ! =====");
            //GenerateP12 generateP12 = new GenerateP12();

            //X509Certificate endEntityCert = Utils.readCertificateFromFile("file/certificate.cer");
            //generateP12.pkcs12Keystore(endEntityCert, generateKeypair.getKey(), "file/testP12.p12", "testP12", "12345678");

        }
    }
}
