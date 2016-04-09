using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Xml;

namespace LooksFamliar.Tools.NuGetUpdate
{
    class Program
    {
        public static string AssemblyName { get; private set; }
        public static string PackageConfigFile { get; private set; }
        public static string PackageFolderPath { get; private set; }
        public static string ProjectFilePath { get; private set; }
        public static string NugetFolderPath { get; private set; }
        public static string NewVersionNumber { get; private set; }
        public static string NetFwVersion { get; private set; }
        public static bool Verbose { get; private set; }

        static void Main(string[] args)
        {
            AssemblyName = string.Empty;
            PackageConfigFile = string.Empty;
            PackageFolderPath = string.Empty;
            ProjectFilePath = string.Empty;
            NugetFolderPath = string.Empty;
            NewVersionNumber = string.Empty;
            NetFwVersion = string.Empty;

            if (args.Length < 10)
            {
                Usage();
                return;
            }

            // parse command line arguments
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-name": // assembly name that is referenced by the project
                        i++;
                        AssemblyName = args[i];
                        break;
                    case "-proj": // project file
                        i++;
                        ProjectFilePath = args[i];
                        break;
                    case "-config": //package.config
                        i++;
                        PackageConfigFile = args[i];
                        break;
                    case "-folder": // package folder path
                        i++;
                        PackageFolderPath = args[i];
                        break;
                    case "-nugets": // nuget folder path
                        i++;
                        NugetFolderPath = args[i];
                        break;
                    case "-netfw": // framework version
                        i++;
                        NetFwVersion = args[i];
                        break;
                    case "-verbose": // output debugging info
                        i++;
                        Verbose = true;
                        break;
                    case "?": // help
                        Usage();
                        break;
                    default: // default
                        Usage();
                        break;
                }
            }

            if ((AssemblyName == string.Empty) ||
                (PackageConfigFile == string.Empty) ||
                (PackageFolderPath == string. Empty) ||
                (ProjectFilePath == string.Empty) ||
                (NugetFolderPath == string.Empty) ||
                (NetFwVersion == string.Empty))
            {
                Console.WriteLine("ERROR: missing command line arguments.");
                Usage();
                return;
            }

            // To perform the NuGet update there are four steps:
            // 1. get the new version number of the assembly from the local nuget folder
            // 2. update the packages.config xml file with the new version number
            // 3. delete the associated assembly folder from packages folder
            // 4. update the project file Reference with new version number info

            var defaultColor = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                if (Verbose) Console.WriteLine("NuGetUpdate: Updating " + AssemblyName + " in " + ProjectFilePath);
                GetNuGetPackageVersion();
                if (Verbose) Console.WriteLine("NuGetUpdate: Using " + AssemblyName + "." + NewVersionNumber);
                UpdateVersionInPackagesConfig();
                //if (Verbose) Console.WriteLine("NuGetUpdate: Updated " + PackageConfigFile);
                //DeleteOldPacakgeFolder();
                //if (Verbose) Console.WriteLine("NuGetUpdate: Removed old package folder for " + AssemblyName);
                UpdateProjectFile();
                if (Verbose) Console.WriteLine("NuGetUpdate: Updated " + ProjectFilePath);
            }
            catch (Exception err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(err.Message);   
            }

            Console.ForegroundColor = defaultColor;

        }

        private static void Usage()
        {
            Console.WriteLine("Usage: NuGetUpdate -name [assembly name] -proj [project file] -config [packages.config file] -folder [packages folder] -nugets [local NuGet folder] -netfw [framework version] -verbose");
            Console.WriteLine("");
            Console.WriteLine("   -name            short name of the assembly");
            Console.WriteLine("   -proj            path to the project file");
            Console.WriteLine("   -config          path to the packages.config file");
            Console.WriteLine("   -folder          path to the project packages folder");
            Console.WriteLine("   -nugets          path to the local NuGet package folder");
            Console.WriteLine("   -netfw           .net fw servion, i.e. net40, net452"   );
            Console.WriteLine("   -verbose         [optional] outputs detail at each step");
            Console.WriteLine("");
        }

        private static void GetNuGetPackageVersion()
        {
            var nugetFolderInfo = new DirectoryInfo(NugetFolderPath);

            if (Verbose)
                Console.WriteLine(NugetFolderPath);

            foreach (var versionElements in from file in nugetFolderInfo.GetFiles() where file.Name.StartsWith(AssemblyName) select file.Name.Split('.'))
            {
                if (Verbose)
                    Console.WriteLine($"Parsing {AssemblyName}");
                NewVersionNumber = versionElements[1] + "." + versionElements[2] + "." + versionElements[3] + "." + versionElements[4];
            }

            if (NewVersionNumber == string.Empty)
                throw new Exception("NuGetUpdate Error: " + AssemblyName + " NuGet package file not found.");
        }

        private static void UpdateVersionInPackagesConfig()
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(PackageConfigFile);

            var packages = xmldoc.ChildNodes[1].ChildNodes;

            foreach (XmlNode item in packages)
            {
                if (item.Attributes["id"].InnerText.StartsWith(AssemblyName))
                    item.Attributes["version"].InnerText = NewVersionNumber;
            }

            xmldoc.Save(PackageConfigFile);
        }

        private static void DeleteOldPacakgeFolder()
        {
            var packageFolderInfo = new DirectoryInfo(PackageFolderPath);

            foreach (var dir in packageFolderInfo.GetDirectories())
            {
                if (dir.Name.StartsWith(AssemblyName))
                    dir.Delete(true);
            }
        }

        private static void UpdateProjectFile()
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(ProjectFilePath);

            var mgr = new XmlNamespaceManager(xmldoc.NameTable);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

            foreach (XmlNode item in xmldoc.SelectNodes("//x:Reference", mgr))
            {
                var include = item.Attributes["Include"].InnerText;
                if (include.StartsWith(AssemblyName))
                {
                    item.Attributes["Include"].InnerText = AssemblyName + @", Version = " + NewVersionNumber + ", Culture = neutral, processorArchitecture = MSIL"; ;
                    if (item.FirstChild.InnerText.StartsWith(".."))
                        item.FirstChild.InnerText = @"..\packages\" + AssemblyName + @"." + NewVersionNumber + @"\lib\" + NetFwVersion + @"\" + AssemblyName + @".dll";
                    else
                        item.FirstChild.InnerText = @"packages\" + AssemblyName + @"." + NewVersionNumber + @"\lib\" + NetFwVersion + @"\" + AssemblyName + @".dll";
                }
            }

            xmldoc.Save(ProjectFilePath);
        }
    }
}
