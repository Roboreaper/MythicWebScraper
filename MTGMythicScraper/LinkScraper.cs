using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MTGMythicScraper
{
    public class CardLink
    {
        public string Url { get; set; }
        public string ImgUrl { get; set; }


    }

    class LinkScraper
    {
        public List<CardLink> Find(string txt)
        {
            List<CardLink> foundUrls = new List<CardLink>();
           // 1.
           // Find all matches in file.
           MatchCollection m1 = Regex.Matches(txt, @"(<a.*?>.*?</a>)",  RegexOptions.Singleline);

            // 2.
            // Loop over each match.
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;

                string href = "";
                // 3.
                // Get href attribute.
                Match m2 = Regex.Match(value, @"href=\""(.*?)\""",   RegexOptions.Singleline);
                if (m2.Success)
                {
                    href = m2.Groups[1].Value;
                }

                if (!href.StartsWith("cards/"))
                    continue;

                string img = "";
                // 3.
                // Get src attribute.
                Match m3 = Regex.Match(value, @"src=\""(.*?)\""",
                RegexOptions.Singleline);
                if (m2.Success)
                {
                    img = m3.Groups[1].Value;
                }

                // 4.
                // Remove inner tags from text.
                string t = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);

                foundUrls.Add(new CardLink() { Url = href , ImgUrl= img } );
            }

            return foundUrls;
        }
    }
}
