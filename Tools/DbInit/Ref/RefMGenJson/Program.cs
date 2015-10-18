using System;
using System.Data;
using System.Data.OleDb;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Common.Wire;

namespace REfMBootstrap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Generate Country Reference Data");
            RefMGenJson.CountryData();

            Console.WriteLine(@"Generate Language Reference Data");
            RefMGenJson.LanguageData();

            Console.WriteLine(@"Generate State Reference Data");
            RefMGenJson.StateData();

            Console.WriteLine(@"Generate MA ZipCode Reference Data");
            RefMGenJson.MAZipCodeData();

            Console.WriteLine(@"Generate Product Data");
            RefMGenJson.ProductData();
        }

        public static class RefMGenJson
        {
            public static void ProductData()
            {
                var count = 0;

                var datafile = AppDomain.CurrentDomain.BaseDirectory + @"data\ProductData.xlsx";

                var connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + datafile + @"; Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                var adapter = new OleDbDataAdapter("select * from [products$]", connStr);
                var ds = new DataSet();
                adapter.Fill(ds, "products");

                foreach (DataRow r in ds.Tables["products"].Rows)
                {
                    var domain = r.ItemArray[0].ToString();
                    var code = r.ItemArray[1].ToString();
                    var value = r.ItemArray[2].ToString();
                    var key1 = r.ItemArray[3].ToString();
                    var key2 = r.ItemArray[5].ToString();
                    var key3 = r.ItemArray[7].ToString();
                    var value1 = r.ItemArray[4].ToString();
                    var value2 = r.ItemArray[6].ToString();
                    var value3 = r.ItemArray[8].ToString();

                    var entity = new Entity(domain, code, value, SequenceType.ALPHA_ASC);
                    entity.attributes[key1] = value1;
                    entity.attributes[key2] = value2;
                    entity.attributes[key3] = value3;

                    var json = ModelManager.ModelToJson<Entity>(entity);
                    var filename = AppDomain.CurrentDomain.BaseDirectory + @"data\product" + count.ToString() + ".json";
                    System.IO.File.WriteAllText(filename, json);

                    Console.WriteLine(count + " Entity: " + entity.code);

                    count++;
                }

            }

            public static void StateData()
            {
                var count = 0;

                var datafile = AppDomain.CurrentDomain.BaseDirectory + @"data\StateData.xlsx";

                var connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + datafile + @"; Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                var adapter = new OleDbDataAdapter("select * from [states$]", connStr);
                var ds = new DataSet();
                adapter.Fill(ds, "states");

                foreach (DataRow r in ds.Tables["states"].Rows)
                {
                    var code = r.ItemArray[0].ToString();
                    var value = r.ItemArray[1].ToString();
                    var capitol = r.ItemArray[2].ToString();
                    var population = r.ItemArray[3].ToString();
                    var squaremiles = r.ItemArray[4].ToString();

                    var entity = new Entity("States", code, value, SequenceType.ALPHA_ASC);
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("Capitol", capitol));
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("Population", population));
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("Square Miles", squaremiles));
                    entity.link = "US";

                    var json = ModelManager.ModelToJson<Entity>(entity);
                    var filename = AppDomain.CurrentDomain.BaseDirectory + @"data\state" + count.ToString() + ".json"; 
                    System.IO.File.WriteAllText(filename, json);

                    Console.WriteLine(count + " Entity: " + entity.code + " | " + entity.codevalue + " | " + capitol + " | " + population + " | " + squaremiles);

                    count++;
                }
            }

            public static void MAZipCodeData()
            {
                var count = 0;

                var datafile = AppDomain.CurrentDomain.BaseDirectory + @"data\MAZipCodes.xlsx";

                var connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + datafile + @"; Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                var adapter = new OleDbDataAdapter("select * from [mazipcodes$]", connStr);
                var ds = new DataSet();
                adapter.Fill(ds, "mazipcodes");

                foreach (DataRow r in ds.Tables["mazipcodes"].Rows)
                {
                    var code = r.ItemArray[0].ToString();
                    var value = r.ItemArray[1].ToString();
                    var state = r.ItemArray[3].ToString();
                    var latitude = r.ItemArray[5].ToString();
                    var longitude = r.ItemArray[6].ToString();

                    var entity = new Entity("ZipCodes", code, value, SequenceType.ALPHA_ASC);
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("State", state));
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("Latitude", latitude));
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("Longitude", longitude));
                    entity.link = "MA";

                    var json = ModelManager.ModelToJson<Entity>(entity);
                    var filename = AppDomain.CurrentDomain.BaseDirectory + @"data\zipcode" + count.ToString() + ".json";
                    System.IO.File.WriteAllText(filename, json);

                    Console.WriteLine(count + " Entity: " + code + " | " + value + " | " + state + " | " + latitude + " | " + longitude);

                    count++;
                }
            }

            public static void CountryData()
            {
                var count = 0;

                var datafile = AppDomain.CurrentDomain.BaseDirectory + @"data\CountryData.xlsx";

                var connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + datafile + @"; Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                var adapter = new OleDbDataAdapter("select * from [countrycodes$]", connStr);
                var ds = new DataSet();
                adapter.Fill(ds, "countrycodes");

                foreach (DataRow r in ds.Tables["countrycodes"].Rows)
                {
                    var code = r.ItemArray[0].ToString();
                    if (code == string.Empty) continue;
                    var value = r.ItemArray[1].ToString();
                    var ico = r.ItemArray[2].ToString();

                    var entity = new Entity("CountryCodes", code, value, SequenceType.ALPHA_ASC);
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("ICO", ico));

                    var json = ModelManager.ModelToJson<Entity>(entity);
                    var filename = AppDomain.CurrentDomain.BaseDirectory + @"data\country" + count.ToString() + ".json";
                    System.IO.File.WriteAllText(filename, json);

                    Console.WriteLine(count + " Entity: " + code + " | " + value + " | " + ico);

                    count++;
                }
            }

            public static void LanguageData()
            {
                var count = 0;

                var datafile = AppDomain.CurrentDomain.BaseDirectory + @"data\LanguageData.xlsx";

                var connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + datafile + @"; Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                var adapter = new OleDbDataAdapter("select * from [languagecodes$]", connStr);
                var ds = new DataSet();
                adapter.Fill(ds, "languagecodes");

                foreach (DataRow r in ds.Tables["languagecodes"].Rows)
                {
                    var code = r.ItemArray[3].ToString();
                    if (code == string.Empty) continue;
                    var value = r.ItemArray[1].ToString();
                    var languageFamily = r.ItemArray[0].ToString();
                    var iso6392T = r.ItemArray[4].ToString();
                    var iso6392B = r.ItemArray[5].ToString();

                    var entity = new Entity("LanguageCodes", code, value, SequenceType.ALPHA_ASC);

                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("ISO639-2T", iso6392T));
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("ISO639-2B", iso6392B));
                    entity.attributes.Add(new LooksFamiliar.Microservices.Ref.Models.Attribute("Language Family", languageFamily));

                    var json = ModelManager.ModelToJson<Entity>(entity);
                    var filename = AppDomain.CurrentDomain.BaseDirectory + @"data\language" + count.ToString() + ".json";
                    System.IO.File.WriteAllText(filename, json);

                    Console.WriteLine(count + " Entity: " + code + " | " + value + " | " + iso6392T + " | " + iso6392B + " | " + languageFamily);

                    count++;
                }
            }
        }
    }
}
