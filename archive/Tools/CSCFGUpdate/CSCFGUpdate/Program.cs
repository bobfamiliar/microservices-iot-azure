using System;
using System.Xml;

namespace CSCFGUpdate
{
    class Program
    {
        public static string CSCFGFile { get; private set; }
        public static string Setting { get; private set; }
        public static string Value { get; private set; }

        static void Main(string[] args)
        {
            CSCFGFile = string.Empty;
            Setting = string.Empty;
            Value = string.Empty;

            if (args.Length < 6)
            {
                Usage();
                return;
            }

            // parse command line arguments
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-file": 
                        i++;
                        CSCFGFile = args[i];
                        break;
                    case "-setting": 
                        i++;
                        Setting = args[i];
                        break;
                    case "-value":
                        i++;
                        Value = args[i];
                        break;
                    case "?": // help
                        Usage();
                        break;
                    default: // default
                        Usage();
                        break;
                }
            }

            if ((CSCFGFile == string.Empty) ||
                (Setting == string.Empty) ||
                (Value == string.Empty))
            {
                Console.WriteLine("ERROR: missing command line arguments.");
                Usage();
                return;
            }

            UpdateSetting();
        }

        private static void UpdateSetting()
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(CSCFGFile);

            var mgr = new XmlNamespaceManager(xmldoc.NameTable);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration");

            foreach (XmlNode item in xmldoc.SelectNodes("//x:Setting", mgr))
            {
                if (item.Attributes != null && item.Attributes["name"].InnerText == Setting)
                    item.Attributes["value"].InnerText = Value;
            }

            xmldoc.Save(CSCFGFile);
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: CSCFGUpdate -file [cscfg file path] -setting [setting to update] -value [new value for the setting]");
            Console.WriteLine("");
            Console.WriteLine("   -file      path to the cscfg file to udpate");
            Console.WriteLine("   -setting   the setting to update");
            Console.WriteLine("   -value     the new value for the setting");
            Console.WriteLine("");
        }
    }
}
