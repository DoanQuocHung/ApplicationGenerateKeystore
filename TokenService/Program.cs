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

            Console.WriteLine("\n===== Export Excel File =====");
            WriteExcelFile writeExcelFile = new WriteExcelFile();
            writeExcelFile.ExportExcel(CSR);
            Console.WriteLine("Export Excel Successfully !");


            Console.WriteLine("\n===== Generate PKCS12 Keystore Successfully ! =====");
            GenerateP12 generateP12 = new GenerateP12();

            X509Certificate endEntityCert = Utils.readCertificateFromFile("file/certificate.cer");
            generateP12.pkcs12Keystore(endEntityCert, generateKeypair.getKey(), "file/testP12.p12", "testP12", "12345678");

        }
    }
}
