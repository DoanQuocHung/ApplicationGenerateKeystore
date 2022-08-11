using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    public static class Constant
    {
        public static string path_file_private_test = @"file/test.tmp"; //test
        public static string path_file_private = Path.GetTempPath() + "/71c4b1a70e48760f8ecb9686df55215c.tmp";
        public static string path_file_csr = @"file/resultCSR.txt";
        public static string CN = "Nguyen van B";//Common Name - Domain name
        public static string OU = "Cong TY ABC";//Organizational Unit Name
        public static string O  = "Cong ty ABC";//Organization Name
        public static string L  = "Location";//Location
        public static string S  = "State";//State
        public static string C  = "Country";//Country


    }
}
