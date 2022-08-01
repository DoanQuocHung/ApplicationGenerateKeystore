﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    class GenerateP12
    {
        public void generateP12()
        {
            Pkcs12Store pkcs12Store = new Pkcs12Store();
            string keyAlias = null;

            foreach (string name in pkcs12Store.Aliases)
            {
                if (pkcs12Store.IsKeyEntry(name))
                {
                    keyAlias = name;
                    break;
                }
            }

            //AsymmetricKeyParameter key = pkcs12Store.GetKey(keyAlias).Key;
            //X509CertificateEntry[] ce = pkcs12Store.GetCertificateChain(keyAlias);
            //List<X509Certificate> chain = new List<X509Certificate>(ce.Length);
            //foreach (var c in ce)
            //{
            //    chain.Add(c.Certificate);
            //}

            //stamper = new Stamper()
            //{
            //    CertChain = chain,
            //    PrivateKey = key,
            //    Stamp = iTextSharp.text.Image.GetInstance(stampImage)
            //};


        }
    }
}