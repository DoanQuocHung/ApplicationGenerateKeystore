using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    class Utils
    {
    
        public static X509Certificate readCertificateFromFile(String pathFile)
        {
            byte[] bytes = File.ReadAllBytes(pathFile);
            X509Certificate certificate = new X509Certificate(bytes);
            return certificate;
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
    }
        
   
}
