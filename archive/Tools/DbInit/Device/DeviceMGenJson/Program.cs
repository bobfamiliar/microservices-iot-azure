using System;
using System.Collections.Generic;
using System.Configuration;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Config.Public.SDK;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Profile.Public.SDK;
using LooksFamiliar.Microservices.Ref.Public.SDK;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Device.Models;

namespace DeviceMGenJson
{
    class Program
    {
        static void Main(string[] args)
        {
            var count = 0;

            var configM = new ConfigM { ApiUrl = ConfigurationManager.AppSettings["ConfigM"] };

            var profileManifest = configM.GetByName("ProfileM");
            var refManifest = configM.GetByName("RefM");

            var profileM = new ProfileM {ApiUrl = profileManifest.lineitems[LineitemsKey.PublicAPI]};
            var participants = profileM.GetAllByType(UserType.Participant);

            var refM = new RefM {ApiUrl = refManifest.lineitems[LineitemsKey.PublicAPI]};
            var products = refM.GetAllByDomain("Products");
            var models = refM.GetAllByDomain("Models");

            // create  a device registration for each particpant
            foreach (var p in participants)
            {
                var r = new Registration();

                foreach (var m in models)
                {
                    if (m.code == "BIOMAX-HOME")
                    {
                        r.key = string.Empty;
                        r.productline = products[0].code;
                        r.model = m.code;
                        r.version = m.attributes["version"];
                        r.firmwareversion = m.attributes["firmware"];
                    }
                }

                r.participantid = p.id;

                var json = ModelManager.ModelToJson<Registration>(r);
                var filename = AppDomain.CurrentDomain.BaseDirectory + @"data\deviceregistration" + count + ".json";
                System.IO.File.WriteAllText(filename, json);

                Console.WriteLine("Device Registration " + count + " generated");
                count++;
            }
        }
    }
}
