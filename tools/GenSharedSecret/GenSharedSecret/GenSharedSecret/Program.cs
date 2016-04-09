using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace GenSharedSecret
{
    class Program
    {
        static void Main(string[] args)
        {
            var apikey = Guid.NewGuid().ToString();
            
            File.AppendAllText(System.IO.Directory.GetCurrentDirectory()+"\\app-key.txt", apikey + "\r\n\r\n");
        }
    }
}
