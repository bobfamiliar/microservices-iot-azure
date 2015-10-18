using System;
using System.Xml;

namespace ConfigUpdate
{
    class Program
    {
        public static string ConfigFile { get; private set; }
        public static string Setting { get; private set; }
        public static string Value { get; private set; }

        static void Main(string[] args)
        {
            ConfigFile = string.Empty;
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
                        ConfigFile = args[i];
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

            if ((ConfigFile == string.Empty) ||
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
            xmldoc.Load(ConfigFile);

            var mgr = new XmlNamespaceManager(xmldoc.NameTable);

            var root = xmldoc.SelectNodes("//appSettings", mgr);
            if (root == null)
                return;

            XmlNode appSettings = null;
            if (root.Count > 0)
                appSettings = root[0];

            if (appSettings == null)
                return;

            foreach (XmlNode item in appSettings.ChildNodes)
            {
                if (item.Attributes == null) continue;
                if (item.Attributes["key"].InnerText == Setting)
                    item.Attributes["value"].InnerText = Value;
            }

            xmldoc.Save(ConfigFile);
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: ConfigUpdate -file [config file path] -setting [setting to update] -value [new value for the setting]");
            Console.WriteLine("");
            Console.WriteLine("   -file      path to the config file to udpate");
            Console.WriteLine("   -setting   the setting to update");
            Console.WriteLine("   -value     the new value for the setting");
            Console.WriteLine("");
        }
    }
}
