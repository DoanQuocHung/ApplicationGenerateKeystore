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


        }
    }
}
