using System;
using System.Data;
using System.Data.OleDb;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Common.Wire;

namespace ProfileMBootstrap
{
    class Program
    {
        static void Main(string[] args)
        {
            var profileMicroservice = new ProfileInitialize();
            Console.WriteLine(@"Load User Profiles - Press Enter to Begin");
            profileMicroservice.GenProfiles();
        }

        public class ProfileInitialize
        {
            public void GenProfiles()
            {
                var count = 0;

                var datafile = AppDomain.CurrentDomain.BaseDirectory + @"data\ProfileData.xlsx";

                var connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + datafile + @"; Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'";
                var adapter = new OleDbDataAdapter("select * from [participants$]", connStr);
                var ds = new DataSet();
                adapter.Fill(ds, "participants");

                foreach (DataRow r in ds.Tables["participants"].Rows)
                {
                    var participant = new UserProfile();

                    participant.firstname = r.ItemArray[0].ToString();
                    participant.lastname = r.ItemArray[1].ToString();
                    participant.address.address1 = r.ItemArray[2].ToString();
                    participant.address.address3 = r.ItemArray[3].ToString();
                    participant.address.city = r.ItemArray[4].ToString();
                    participant.address.state = r.ItemArray[5].ToString();
                    participant.address.zip = r.ItemArray[6].ToString();
                    participant.address.country = r.ItemArray[7].ToString();
                    participant.social.phone = r.ItemArray[8].ToString();
                    participant.location.longitude = System.Convert.ToDouble(r.ItemArray[9]);
                    participant.location.latitude = System.Convert.ToDouble(r.ItemArray[10]);

                    participant.username = participant.firstname[0] + @"." + participant.lastname;
                    participant.type = UserType.Participant;
                    participant.social.email = participant.username + @"@looksfamiliar.com";

                    var json = ModelManager.ModelToJson<UserProfile>(participant);
                    var filename = AppDomain.CurrentDomain.BaseDirectory + @"data\participant" + count.ToString() + ".json";
                    System.IO.File.WriteAllText(filename, json);

                    Console.WriteLine(count + " Participant: " + count.ToString());

                    count++;
                }
            }
        }
    }
}
