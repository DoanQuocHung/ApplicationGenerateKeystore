using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

            Console.WriteLine("\n===== Get subject CN from cert =====");
            string fileCerTEST = "file/testCer2408.cer";
            string text = File.ReadAllText(fileCerTEST);
            Console.WriteLine(text);

            X509Certificate2 x509 = new X509Certificate2();
            byte[] rawData = ReadFile(fileCerTEST);
            x509.Import(rawData);

            string subjectCN;
            string subjectMST;
            string mark1 = "OID.";
            string mark2 = "CN=";
            string mark3 = "=CMND";
            string subjectCert = x509.Subject;
            Console.WriteLine("Cer: " + subjectCert);
            string[] words = subjectCert.Split(',');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains(mark1))
                {
                    string[] words_2 = words[i].Split('=');
                    subjectMST = words_2[1].Replace(':', '-');
                }
                else if (words[i].Contains(mark2))
                {
                    //subjectCN = words[i].Remove(0, 4);
                    string[] words_2 = words[i].Split('=');
                    Console.WriteLine("Field cn: " + words_2[1]);
                }
            }

            //Console.WriteLine("Subject: " + x509.SubjectName.Name);
            //Console.Write("{0}Subject: {1}{0}", Environment.NewLine, x509.Subject);
            //Console.Write("{0}Issuer: {1}{0}", Environment.NewLine, x509.Issuer);
            ////Console.Write("{0}Version: {1}{0}", Environment.NewLine, x509.Version);
            //Console.Write("{0}Valid Date: {1}{0}", Environment.NewLine, x509.NotBefore);
            //Console.Write("{0}Expiry Date: {1}{0}", Environment.NewLine, x509.NotAfter);
            //Console.Write("{0}Thumbprint: {1}{0}", Environment.NewLine, x509.Thumbprint);
            //Console.Write("{0}Serial Number: {1}{0}", Environment.NewLine, x509.SerialNumber);
        }
        public static byte[] ReadFile(string fileName)
        {
            FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int size = (int)f.Length;
            byte[] data = new byte[size];
            size = f.Read(data, 0, size);
            f.Close();
            return data;
        }
    }
}
