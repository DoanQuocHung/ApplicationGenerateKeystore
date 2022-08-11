using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    public class GenerateP12
    {
        public void pkcs12Keystore(X509Certificate newCert, X509Certificate CertChain, AsymmetricCipherKeyPair kp, string FilePath, string CertAlias, string Password)
        {
            var newStore = new Pkcs12Store();
            var certEntry = new X509CertificateEntry(newCert);
            var certChain = new X509CertificateEntry(CertChain);

            newStore.SetCertificateEntry(
                CertAlias,
                certEntry
                );

            newStore.SetKeyEntry(
                CertAlias,
                new AsymmetricKeyEntry(kp.Private),
                new[] { certEntry, certChain }
                );

            using (var certFile = File.Create(FilePath))
            {
                newStore.Save(
                    certFile,
                    Password.ToCharArray(),
                    new SecureRandom(new CryptoApiRandomGenerator())
                    );
                certFile.Close();
            }


        }

        public void pkcs12Keystore(X509Certificate newCert, X509Certificate CertChain, AsymmetricKeyParameter kp, string FilePath, string CertAlias, string Password)
        {
            var newStore = new Pkcs12Store();
            var certEntry = new X509CertificateEntry(newCert);
            var certChain = new X509CertificateEntry(CertChain);
            
            newStore.SetCertificateEntry(
                CertAlias,
                certEntry
                );

            newStore.SetKeyEntry(
                CertAlias,
                new AsymmetricKeyEntry(kp),
                new[] { certEntry, certChain }
                );

            using (var certFile = File.Create(FilePath))
            {
                newStore.Save(
                    certFile,
                    Password.ToCharArray(),
                    new SecureRandom(new CryptoApiRandomGenerator())
                    );
                certFile.Close();
            }
        }
        public void pkcs12Keystore(X509Certificate newCert, List<X509Certificate> listCertChain, AsymmetricKeyParameter kp, string FilePath, string CertAlias, string Password)
        {
            var newStore = new Pkcs12Store();
            var certEntry = new X509CertificateEntry(newCert);            
            X509CertificateEntry[] entry = new X509CertificateEntry[listCertChain.Count + 1];
            int position = 0;
            entry[0] = certEntry; position++;
            foreach(X509Certificate cert in listCertChain)
            {
                entry[position] = new X509CertificateEntry(cert);
                position++;
            }

            newStore.SetCertificateEntry(
                CertAlias,
                certEntry
                );

            newStore.SetKeyEntry(
                CertAlias,
                new AsymmetricKeyEntry(kp),
                entry
                );

            using (var certFile = File.Create(FilePath))
            {
                newStore.Save(
                    certFile,
                    Password.ToCharArray(),
                    new SecureRandom(new CryptoApiRandomGenerator())
                    );
                certFile.Close();
            }
        }
    }
}
