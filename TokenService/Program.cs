using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ManageKey generateKeypair = new ManageKey();

            //Console.WriteLine("===== Sinh khóa RSA và sinh CSR =====");
            //string subjectDN = generateKeypair.createInformation();
            //generateKeypair.generateKey(2048);
            //string CSR = generateKeypair.generateCSR(subjectDN, null);
            //Console.WriteLine("Generated CSR Successfully !");


            //Console.WriteLine("\n===== Decrypt private key =====");
            //ManageAlgorithm a = new ManageAlgorithm();
            //string result = a.EncryptPrivateKey(generateKeypair.getKey());
            //Console.WriteLine("Encrypt:" + result);
            //string privatekey = a.DecryptPrivateKey(generateKeypair.getPublicKey());
            //Console.WriteLine("\nPrivatekey:" + privatekey);


            //Console.WriteLine("\n===== Export Excel File =====");
            //WriteExcelFile writeExcelFile = new WriteExcelFile();
            //ManageAlgorithm manageAlgorithm = new ManageAlgorithm();
            //string cipher = manageAlgorithm.EncryptPrivateKey(generateKeypair.getPublicKey().ToString())
            //writeExcelFile.ExportExcel(CSR);
            //Console.WriteLine("Export Excel Successfully !");


            //Console.WriteLine("\n===== Generate PKCS12 Keystore =====");
            //GenerateP12 generateP12 = new GenerateP12();
            //X509Certificate endEntityCert = Utils.readCertificateFromFile("file/cert.cer");
            //X509Certificate CertChain = Utils.readCertificateFromFile("file/mobileidcert.cer");

            //string EndCertDecoded = Utils.convertCertToBase64(endEntityCert);
            //string CertChainDecoded = Utils.convertCertToBase64(CertChain);

            //X509Certificate x509cert_1 = new X509Certificate(Convert.FromBase64String(EndCertDecoded));
            //X509Certificate x509cert_2 = new X509Certificate(Convert.FromBase64String(CertChainDecoded));

            //generateP12.pkcs12Keystore(x509cert_1, x509cert_2, generateKeypair.getKey(), "file/testP12.p12", "testP12", "12345678");  //endEntityCert, CertChain
            //Console.WriteLine("\nGenerate PKCS12 Keystore Successfully !");


            //Console.WriteLine("\n===== Compress File RAR =====");
            //String pathzip = "file/Zip.zip";
            //String path1 = @"C:\Users\gia\Desktop\cert.cer";
            //String path2 = @"C:\Users\gia\Desktop\certChain.cer";
            //List<String> temp = new List<String>();
            //temp.Add(path1);
            //temp.Add(path2);
            //IEnumerable<string> file = temp;
            //Utils.CreateZipFile(pathzip,file);

            //Console.WriteLine("\n===== Generate random password =====");
            //string passWord = Utils.CreatePassword(8);
            //Console.WriteLine("TEST: " + passWord);

            string path = @"C:\Users\gia\Desktop\New Text Document (2).txt";
            string q = File.ReadAllText(path);
            Console.WriteLine("Level:"+Utils.CheckLevelOfCertificate(q));
            string[] result = Utils.convertPEMtoArrayBase64(q);
            foreach(string s in result)
            {
                Console.WriteLine("Ketqua:" + s);
            }
            //Console.WriteLine("String:" + Utils.convertPEMtoArrayBase64(q));
        }
    }
}
