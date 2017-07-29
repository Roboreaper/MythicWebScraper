using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MTGMythicScraper
{
    class Program
    {

        public static string tableStart = "<!--THE individual card--><!--Do Not EDIT-->";
        public static string tableEnd = "<!--DELETE THIS LINE and one below to enable sourcing";

        static void Main(string[] args)
        {

            (new ScraperMainForm()).ShowDialog();
            return;

            //string sourceCode = "";
            //string siteUrl = "http://mythicspoiler.com/";
            //var options = new CommandLineOptions();
            //if(CommandLine.Parser.Default.ParseArguments(args, options) )
            //{             
            //    using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            //    {
            //        sourceCode = client.DownloadString("http://mythicspoiler.com/"+ options.Set+"/");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine(options.GetUsage());
            //    var l = Console.ReadLine();

            //    return;
            //}

            //LinkScraper scraper = new LinkScraper();
            //var result =scraper.Find(sourceCode);

            //Console.WriteLine($"found {result.Count} cards");
            //List<Card> Cards = new List<Card>();

            //CardScraper cs = new CardScraper();

            //var cardCount = result.Count;
            //var progress = 0; var temp = 0; var tenth = cardCount / 10;
            //foreach (var link in result)
            //{
            //    string cardPage = "";
            //    string tempUrl = siteUrl + options.Set + "/" + link.Url;
            //    try
            //    {
            //        using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            //        {
            //            cardPage = client.DownloadString(tempUrl);

            //            var card = cs.Scrape(cardPage, options.Set, siteUrl + options.Set + "/" + link.ImgUrl);
            //            if (string.IsNullOrEmpty(card.Name))
            //            {
            //                Console.WriteLine("Error for: " + link.Url);
            //                continue;
            //            }

            //            Cards.Add(card);
            //        }
            //    }
            //    catch(Exception e)
            //    {
            //        Console.WriteLine("Timout for Error for: " + link.Url);
            //    }

            //    ++temp;
            //    if (temp >= tenth)
            //    {
            //        temp = 0;
            //        progress += 10;
            //        Console.WriteLine($"{progress}%...");
            //    }


            //}

            //Console.WriteLine(Cards.Count);

            //XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            //{
            //    Indent = true,
            //    IndentChars = "\t",
                
            //};

            //using (var writer = XmlWriter.Create("cocatrice_" + options.Set + ".xml", xmlWriterSettings))
            //{
            //    Console.WriteLine("Creating XML");

            //    writer.WriteStartDocument();
              
            //    //writer.WriteStartElement("set");
            //    //writer.WriteElementString("longname", "Commander 2016");
            //    //writer.WriteElementString("settype", "Commander");
            //    //writer.WriteElementString("releasedate", DateTime.Now.ToString("YYYY-MM-dd"));
            //    //writer.WriteEndElement();

            //    writer.WriteStartElement("cards");

            //    foreach (var card in Cards)
            //    {
            //        card.Serialize(writer);
            //    }

            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}

            //Console.WriteLine("done");
            //Console.ReadLine();

        }

        private static Card GetCard(CardLink link,int id, string siteUrl, CommandLineOptions options)
        {
            CardScraper cs = new CardScraper();
            string cardPage = "";
            string tempUrl = siteUrl + options.Set + "/" + link.Url;
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                cardPage = client.DownloadString(tempUrl);

                return cs.Scrape(id,cardPage, options.Set, siteUrl + "/" + link.ImgUrl);               
            }
        }
      
    }
}
