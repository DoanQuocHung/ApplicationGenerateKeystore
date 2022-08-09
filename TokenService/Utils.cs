using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace TokenService
{
    public class Utils
    {
        //Using regex (regular expression) to check input 
        public static Boolean CheckInputNumber(string input)
        {
            string pattern = @"^\d+$";
            Regex regex = new Regex(pattern);
            return Regex.IsMatch(input, pattern);           
        }
        public static Boolean CheckInputPath(string input)
        {
            //Check input path
            string pattern = @"^[0-9a-zA-Z_\-.\\:\s]+$";
            Regex regex = new Regex(pattern);
            if(!Regex.IsMatch(input,pattern))
                return false;
            //Check excel extension
            string end = input.Substring(input.Length-4);
            if (!end.Equals("xlsx"))
                return false;            
            return true;
        }

        public static string UsingRegexDeleteNewLine(string input)
        {            
            string replacewith = "";
            return input.Replace("\n",replacewith);            
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
            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            }            
            zip.Dispose();
        }
        public static Boolean CheckFormatPEMOfString(String input)
        {
            //Check if PEM extension or not 
            if (input.Contains("BEGIN") || input.Contains("END") || input.Contains("begin") || input.Contains("end"))
            {
                return true;
            }
            return false;
        }

        public static int CheckLevelOfCertificate(String input)
        {
            int level = 0;
            string[] delimiter = {"BEGIN"};
            string[] tokens = input.Split(delimiter, StringSplitOptions.None);            
            return tokens.Length - 1;
        }

        public static string[] convertPEMtoArrayBase64(String peminput)
        {
            string pattern = @"[-]*(BEGIN|END|begin|end)[ ]+(CERTIFICATE|certificate)[-]*";
            string[] delimiter = { "*" };
            string temp = Regex.Replace(peminput, pattern, "*");
            string[] tokens = temp.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);            
            return tokens;
        }

        public static Boolean isBlank(string input)
        {
            string pattern = @"^(\s|\n|\t)*$";
            return Regex.IsMatch(input, pattern);
        }
                                
        //Generate random password===================================================
        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

    }
}
