using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    public class ManageKey
    {
        public static String type_pkcs12 = "PKCS12";
        public static String type_jks = "JKS";

        private AsymmetricCipherKeyPair key;
        private Pkcs10CertificationRequest CSR;

        public Pkcs10CertificationRequest getCSR()
        {
            return this.CSR;
        }

        public AsymmetricCipherKeyPair getKey()
        {
            return this.key;
        }

        public String createInformation()
        {
            String CN = "Nguyen Van A";     //Common Name - Domain name
            String OU = "Cong Ty ABC";      //Organizational Unit Name
            String O = "Cong Ty ABC";       //Organization Name
            String L = "Quan 1";            //Location
            String S = "TPHCM";             //State
            String C = "VN";                //Country
            //String P = "PrivateKey";
            String result = "CN=" + CN + ",OU=" + OU + ",O=" + O + ",C=" + C + ",L=" + L + ",ST=" + S ;
            return result;
        }

        public void generateKey(int keySize)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            key = default(AsymmetricCipherKeyPair);
            var keyGenerationParameters = new KeyGenerationParameters(random, keySize);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            key = keyPairGenerator.GenerateKeyPair();
        }

        public string generateCSR(String subjectDN, String algorithm)
        {
            var subject = new X509Name(subjectDN);
            if (subjectDN == null)
                return null;
            if (algorithm == null)
            {
                algorithm = "SHA1WITHRSA";
            }
            Pkcs10CertificationRequest csr = new Pkcs10CertificationRequest(algorithm, subject, key.Public, null, key.Private);

            StringBuilder CSRPem = new StringBuilder();
            PemWriter CSRPemWriter = new PemWriter(new StringWriter(CSRPem));
            CSRPemWriter.WriteObject(csr);
            CSRPemWriter.Writer.Flush();

            string CSRtext = CSRPem.ToString();

            using (StreamWriter f = new StreamWriter(@"file/resultCSR.txt"))
            {
                f.Write(CSRtext);
            }
            return CSRtext;
        }

        public void generateKeyStore(String certificate, String CACertificate, String alias, String password, String type)
        {
            if (certificate == null || CACertificate == null || alias == null
                || password == null || type == null)
            {
                throw new Exception("Need more information");
            }
            if (this.key == null)
            {
                throw new Exception("Please, Create Key first!!");
            }
            AsymmetricKeyParameter privateKey = key.Private;



        }

        //Get from excel file
        private void getPrivateKey(String cipher)
        {

        }
    }
}
