using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaiScuola
{
    class Program
    {
        private static string MakeValidFileName(string name)
        {
            return Regex.Replace(name.ToLower(), "[^a-z0-9]", "_");
        }


        static void MedicinaInterna()
        {

            for (int page = 1; page <= 3; page++)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var cli = new WebClient())
                {
                    cli.Encoding = Encoding.UTF8;
                    cli.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.56");
                    cli.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                    string htmlCode = cli.DownloadString($"http://www.raiscuola.rai.it/categorie/medicina-interna/196/{page}/default.aspx");
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlCode);
                    
                    //<div class="entry-content">
                    foreach (var node in doc.DocumentNode.SelectNodes("//div[contains(@class, 'entry-content')]/a[@href]"))
                    {
                        // Get the value of the HREF attribute
                        string hrefValue = node.GetAttributeValue("href", string.Empty);
                        string titleValue = node.GetAttributeValue("title", string.Empty);
                        if (hrefValue.StartsWith("/articoli"))
                        {                           
                            try
                            {
                                //sample page name http://www.raiscuola.rai.it/articoli/gli-enzimi-digestivi/9642/default.aspx
                                using (var cli2 = new WebClient())
                                {
                                    cli2.Encoding = Encoding.UTF8;
                                    cli2.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng;q=0.8,application/signed-exchange;v=b3;q=0.9");
                                    cli2.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.56");
                                    cli2.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                                    string htmlCode2 = cli2.DownloadString($"http://www.raiscuola.rai.it{hrefValue}");
                                    
                                    // sample video name http://flashedu.rai.it/ieduportale/medita/7894.mp4

                                    var url = Regex.Match(htmlCode2, @"http://flashedu.rai.it/ieduportale/medita/(\d+).mp4");
                                    Debug.WriteLine($"[{titleValue}]({url.Value})");
                                    cli2.DownloadFile(url.Value, MakeValidFileName($"{url.Groups[1].Value}_{titleValue}.mp4"));
                                    Console.WriteLine($"Downloaded {titleValue}");                                   
                                }
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            MedicinaInterna();
        }
    }
}
