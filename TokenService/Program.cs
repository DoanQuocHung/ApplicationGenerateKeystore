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
            ManageKey generateKeypair = new ManageKey();
            Console.WriteLine("===== Sinh khóa RSA và sinh CSR =====");
            string subjectDN = generateKeypair.createInformation();
            generateKeypair.generateKey(2048);
            //generateKeypair.getPrivateKey();
            string CSR = generateKeypair.generateCSR(subjectDN, null);
            Console.WriteLine("Generated CSR Successfully !");


            //Console.WriteLine("\n===== Decrypt private key =====");
            //ManageAlgorithm a = new ManageAlgorithm();
            //string result = a.EncryptPrivateKey(generateKeypair.getKey());
            //Console.WriteLine("Encrypt:" + result);
            //string privatekey = a.DecryptPrivateKey(generateKeypair.getPublicKey(), result);
            //Console.WriteLine("\nPrivatekey:" + privatekey);


            //Console.WriteLine("\n===== Export Excel File =====");
            //WriteExcelFile writeExcelFile = new WriteExcelFile();
            ////Cipher privateKey
            ////ManageAlgorithm manageAlgorithm = new ManageAlgorithm();
            ////string cipher = manageAlgorithm.EncryptPrivateKey(generateKeypair.getPublicKey().ToString())
            //writeExcelFile.ExportExcel(CSR, null);
            //Console.WriteLine("Export Excel Successfully !");


            //Console.WriteLine("\n===== Generate PKCS12 Keystore =====");
            GenerateP12 generateP12 = new GenerateP12();
            X509Certificate endEntityCert = Utils.readCertificateFromFile("file/cert.cer");
            //X509Certificate CertChain = Utils.readCertificateFromFile("file/mobileidcert.cer");
            X509Certificate CertChain = Utils.readCertificateFromFile(@"C:\Users\gia\Desktop\certChain.cer");
            generateP12.pkcs12Keystore(endEntityCert, CertChain, generateKeypair.getKey(), "file/testP12.p12", "testP12", "12345678");
            Console.WriteLine("\nGenerate PKCS12 Keystore Successfully !");

        }
    }
}
