
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;


namespace TokenService
{
    class Utils
    {
        public static Boolean CheckExtensionPEM(String pathFile)
        {
            String q = File.ReadAllText(pathFile, Encoding.UTF8);
            if (q.Contains("BEGIN") || q.Contains("END") || q.Contains("begin") || q.Contains("end"))
            {
                return true;
            }
            return false;
        }
        public static X509Certificate readCertificateFromFile(String pathFile)
        {
            if (CheckExtensionPEM(pathFile))
            {
                string base64 = readCertificateFromPemFile(pathFile);
                return convertBase64ToCert(base64);
            }
            byte[] bytes = File.ReadAllBytes(pathFile);
            X509Certificate certificate = new X509Certificate(bytes);
            return certificate;
        }

        private static string readCertificateFromPemFile(String pathFile)
        {
            String[] q = File.ReadAllLines(pathFile);
            int count = 1;
            String base64 = "";
            while (true)
            {
                if (count == q.Length - 2)
                    break;
                base64 += q[count];
                count++;

            }
            return base64;
        }

        public static String convertCertToBase64(X509Certificate certificate)
        {
            byte[] bytes = certificate.GetEncoded();
            return Convert.ToBase64String(bytes);
        }

        public static X509Certificate convertBase64ToCert(String base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return new X509Certificate(bytes);
        }


        public static void CreateZipFile(string fileName, IEnumerable<string> files)
        {
            // Create and open a new ZIP file
            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }

    }


}
