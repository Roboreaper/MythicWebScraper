using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTGMythicScraper
{
    public class CardScraper
    {
        const string Tagname      = "<!--CARD NAME-->";
        const string Tagmanacost  = "<!--MANA COST-->";
        const string Tagtype      = "<!--TYPE-->";
        const string Tagtext      = "<!--CARD TEXT-->";
        const string Tagflavor    = "<!--FLAVOR TEXT-->";
        const string Tagpt        = "<!--P/T-->";
        const string Tagend       = "<!--END CARD TEXT-->";

        public Card Scrape(string page, string set, string img)
        {
            string htmlCode = page;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlCode);

            // content
            HtmlNode table = doc.DocumentNode.Descendants("table").ElementAt(5);

            var text = table.InnerText;
            Card c = new Card();
            c.set = set;
            c.ImageUrl = img;

            var currenttag = "";
            using (var sr = new StringReader(text))
            {
                string txtBlob = "";
             
                try
                {
                    // quick and dirty loop to scan table contents
                    while( sr.Peek() >= 0)
                    {
                        var l = sr.ReadLine();
                        if( l.StartsWith("<!--")) // tag found;
                        {
                            var tag = CheckTag(l);
                            if (tag == "Unknown")
                                continue;

                            if( tag != currenttag)
                            {
                                processTexBlob(ref c, currenttag, txtBlob);

                                currenttag = tag;
                                txtBlob = "";
                            }
                        }
                        else
                        {
                            txtBlob += l;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }


            return c;
        }

        // straightforward method :)
        private void processTexBlob(ref Card card ,string currentTag , string txtBlob)
        {
            var temp = txtBlob.Trim();
            temp = temp.Trim(new Char[] { '"' });

            switch (currentTag)
            {
                case Tagname:
                    //card.name = temp;
                    card.Name = Regex.Match(temp, @"(?i)^[a-z ,'-_]+").Value;
                    break;
                case Tagmanacost:
                    card.manacost = temp;
                    card.cmc = DetermineCmC(temp);
                    card.color = DetermineColor(temp);
                    break;
                case Tagtype:
                    card.type = temp;
                    break;
                case Tagtext:
                    card.text = temp;
                    break;
                case Tagflavor:                    
                    break;
                case Tagpt:
                    card.pt = temp;
                    break;
                case Tagend:
                default:
                    return;
            }


        }

        // straightforward method :)
        private string DetermineColor(string manacost)
        {
            string color = "";
            foreach (char c in manacost)
            {
                
                switch(c)
                {
                    case 'W':
                        color += "W";
                        break;
                    case 'U':
                        color += "U";
                        break;
                    case 'B':
                        color += "B";
                        break;
                    case 'G':
                        color += "G";
                        break;
                    case 'R':
                        color += "R";
                        break;
                    default:
                        break;
                }

            }
            return string.Join("", color.Distinct());
        }

        private int DetermineCmC(string manacost)
        {
            int cmc = 0;
            foreach (char c in manacost)
            {
                int temp = 0;
                if(int.TryParse(c.ToString(), out temp))
                {
                    // is it a number
                    cmc += temp;
                }
                else
                {
                    // nope jsut a symbol
                    cmc++;
                }

            }
            return cmc;
        }

        // ugly code :(
        private string CheckTag(string l)
        {
            if (l.Contains(Tagname))
                return Tagname;
            else if (l.Contains(Tagmanacost))
                return Tagmanacost;
            else if (l.Contains(Tagflavor))
                return Tagflavor;
            else if (l.Contains(Tagpt))
                return Tagpt;
            else if (l.Contains(Tagtext))
                return Tagtext;
            else if (l.Contains(Tagtype))
                return Tagtype;
            else if (l.Contains(Tagend))
                return Tagend;
            else
                return "Unknown";
        }
    }
}
