using System;
using System.Collections.Generic;
using System.IO;
using FirmwareParser.CoreClass;
using Newtonsoft.Json;


namespace FirmwareParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            string Line;
            DateTime DateToday = DateTime.UtcNow.Date;
            int TimeToday = DateTime.UtcNow.Hour;

            DlinkSwitch dlinkSwitch = new DlinkSwitch();
            var Dlink = dlinkSwitch.ReadSwitchFile();


            Product product = new Product();

            while ((line = Dlink.ReadLine()) != null)
            {
                string htmldownloadedfile = dlinkSwitch.DownloadHTTP(dlinkSwitch.Lineurl(line));

                if (!File.Exists((@"C:\Users\Telekom1\source\repos\SwitchUpdater\SwitchUpdater\data\" + dlinkSwitch.Linemodel(line) + ".html")))
                {
                    dlinkSwitch.CreateHTMLFILE(dlinkSwitch.Linemodel(line), htmldownloadedfile);

                }
                string filesyzelength = dlinkSwitch.LengthTake(dlinkSwitch.Linemodel(line));

                string CheckSum = dlinkSwitch.HashTake(htmldownloadedfile);

                var HTMLfile = dlinkSwitch.ReadHTMLFILE(dlinkSwitch.Linemodel(line));
                while ((Line = HTMLfile.ReadLine()) != null)
                {
                    var pair = dlinkSwitch.Regparser(htmldownloadedfile);
                    string HREF = pair.Key;
                    string TEXT = pair.Value;


                    product.Models.Add(new Dictionary<string, string>()
                        {
                            { "CheckSum",  CheckSum},
                            { "Size", filesyzelength},
                            { "Model", dlinkSwitch.Linemodel(line) },
                            { "ModelUrl", dlinkSwitch.Lineurl(line) },
                            { "FileUpdateTime", Convert.ToString(DateTime.Now) },
                            { "Firmware", TEXT },
                            { "FirmwareUrl", HREF }                
                        });

                    break;

                }
                HTMLfile.Close();
                product.UpdTime = DateTime.Now;
                product.Manufacturer = "Dlink";
            }
            Dlink.Close();
            string json = JsonConvert.SerializeObject(product, Formatting.Indented);
            var JsonCreate = dlinkSwitch.JsonToFile(product.Manufacturer);
            JsonCreate.WriteLine(json);
            JsonCreate.Close();
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
