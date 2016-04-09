using System;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Common.Wire;

namespace LooksFamiliar.Tools.DbInit.Config
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateManifest(
                "ConfigM",
                "The ConfigM Microservice provides storage and retrieval of configuration manifests for Microservices",
                "Manifest",
                "ConfigM",
                "ManifestCollection",
                "<ConfigM Public End Point>",
                "<ConfigM Admin End Point>");

            CreateManifest(
                "LogM",
                "The LogM Microservice provides logging of various types of messages via a message Q and a ReST API for reporting on those messages",
                "Message",
                "LogM",
                "MessageCollection",
                "",
                "<LogM Admin End Point>");

            CreateManifest(
                "RefM",
                "The RefM Microservice provides common reference data such as list of states, country codes, etc.",
                "Entity",
                "RefM",
                "ReferenceCollection",
                "<RefM Public End Point>",
                "<RefM Admin End Point>");

            CreateManifest(
                "ProfileM",
                "The ProfileM Microservice provides user profiles containing address, social and preference settings",
                "UserProfile",
                "ProfileM",
                "ProfileCollection",
                "<ProfileM Public End Point>",
                "<ProfileM Admin End Point>");

            CreateManifest(
                "DeviceM",
                "The DeviceM Microservice provides device registration capabilities",
                "Registration",
                "DeviceM",
                "Registry",
                "<DeviceM Public End Point>",
                "<DeviceM Admin End Point>");

            CreateManifest(
                "BiometricsAPI",
                "The BiometricsAPI provides contextual acces to real-time telemetry coming from BioMax devices.",
                "BiometricReading",
                "BiometricsDb",
                "Biometrics",
                "<BiometricsAPI Public End Point>",
                "");

            CreateManifest(
                "AlarmWorker",
                "The Biometrics Alarm Worker reads incoming alarm messages, logs them in the BiometricsDb database and fires off alerts to applications that subscribe to the biometrics alarm notification channel.",
                "BiometricReading",
                "BiometricsDb",
                "Biometrics",
                "",
                "");

            Console.ReadLine();
        }

        private static void CreateManifest(string name, string description, string model, string database, string collection, string publicapi, string adminapi)
        {
            var manifest = new Manifest
            {
                name = name,
                description = description,
                cachettl = 10
            };

            manifest.lineitems[LineitemsKey.ModelName] = model;
            manifest.lineitems[LineitemsKey.Database] = database;
            manifest.lineitems[LineitemsKey.Collection] = collection;
            manifest.lineitems[LineitemsKey.PublicAPI] = publicapi;
            manifest.lineitems[LineitemsKey.AdminAPI] = adminapi;

            var json = ModelManager.ModelToJson<Manifest>(manifest);
            var filename = AppDomain.CurrentDomain.BaseDirectory + @"\data\" + name + "manifest.json";
            System.IO.File.WriteAllText(filename, json);

            Console.WriteLine("The ConfigM Manifest for " + manifest.name + " has been created");
        }
    }
}
