using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MTGMythicScraper
{
    public class Card
    {       

        public string name { get; set; }
        public string set { get; set; }
        public string ImageUrl { get; set; }
        public string color { get; set; }
        public string manacost { get; set; }
        public int cmc { get; set; }
        public string type { get; set; }
        public string pt { get; set; }
        //public int tablerow { get; set; } = 2;
        public string text { get; set; }

        public override string ToString()
        {
            return $"{manacost} {name}";
        }


        public void Serialize(XmlWriter writer)
        {
            if (string.IsNullOrEmpty(name))
                return;

            writer.WriteStartElement("card");

            writer.WriteElementString("name", name);

            writer.WriteStartElement("set");
            writer.WriteAttributeString("picURL", ImageUrl);          
            writer.WriteString(set.ToUpper());
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(color))
                foreach (var colorId in color)
                {
                    writer.WriteElementString("color", colorId.ToString());
                }

            if (!string.IsNullOrEmpty(manacost))
                writer.WriteElementString("manacost", manacost);

            if (cmc > 1)
                writer.WriteElementString("cmc", cmc.ToString());

            if (!string.IsNullOrEmpty(type))
                writer.WriteElementString("type", type);

            if( !string.IsNullOrEmpty(pt))
                writer.WriteElementString("pt", pt);

            //writer.WriteElementString("tablerow", tablerow.ToString());
            writer.WriteElementString("text", text);

            writer.WriteEndElement();
        }
    }
}
