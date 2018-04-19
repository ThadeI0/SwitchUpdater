using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using AngleSharp.Parser.Html;
using AngleSharp.Extensions;

namespace FirmwareParser.CoreClass
{
    //Сборщик сведений о моделях
    public class Product
    {
        public DateTime UpdTime;
        public string Manufacturer;
        public List<Dictionary<string, string>> Models = new List<Dictionary<string, string>>();

    }

    //Коммутаторы Dlink
    public class DlinkSwitch
    {
        public StreamReader ReadSwitchFile()
        {
            try
            {
                StreamReader SwitchFile = new StreamReader(@"Dlink.txt");
                return SwitchFile;
            }
            catch (FileNotFoundException e)
            {
                Console.Write(e);
                return null;
            }

        }

        public string Linemodel(string line)
        {
            string lineA = line.Remove(line.IndexOf(' '), line.Length - line.IndexOf(' ')).Replace('\\', '_').Replace('/', '_');
            return lineA;
        }

        public string Lineurl(string line)
        {
            string lineA = line.Remove(0, line.IndexOf(' ') + 1);

            return lineA;
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString());

            return sb.ToString();
        }

        public string HashTake(string HTML)
        {
            string hashtofile = GetHashString(HTML);
            return hashtofile;
        }

        public string LengthTake(string model)
        {
            FileInfo fileInfo = new FileInfo(@"httpproj\" + model + ".html");
            string filelength = fileInfo.Length.ToString();
            return filelength;
        }

        public string DownloadHTTP(string url)
        {
            var webClient = new WebClient();
            string HTML = webClient.DownloadString(url);
            return HTML;
        }

        public void CreateHTMLFILE(string model, string HTML)
        {
            StreamWriter file = new StreamWriter(@"httpproj\" + model + ".html");
            file.WriteLine(HTML);
            file.Close();
        }

        public StreamReader ReadHTMLFILE(string model)
        {
            try
            {
                StreamReader HtmlFile = new StreamReader(@"httpproj\" + model + ".html");
                return HtmlFile;
            }
            catch (FileNotFoundException e)
            {
                Console.Write(e);
                return null;
            }

        }

        public KeyValuePair<string, string> Regparser(string HTML)
        {
            string HREF = null;
            string TEXT = null;
            var parser = new HtmlParser();
            var document = parser.Parse(HTML);
            var SelAlla = document.QuerySelectorAll("a[href*='Firmware']");

            foreach (var item in SelAlla)
            {
                HREF = item.GetAttribute("href");
                TEXT = item.Text();
                break;

            }
            return new KeyValuePair<string, string>(HREF, TEXT);
           
        }

        public StreamWriter JsonToFile(string manufacturer)
        {
            StreamWriter File = new StreamWriter(@"httpproj\" + manufacturer + ".json");
            return File;
        }
    }

}
