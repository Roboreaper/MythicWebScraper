using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MTGMythicScraper
{
    public class Card : INotifyPropertyChanged
    {
        private string name;

        public string Name { get { return name; } set { name = value; NameChanged(); } }

        [Browsable(false)]
        public string set { get; set; }
        [Browsable(false)]
        public string ImageUrl { get; set; }
        [Browsable(false)]
        public string color { get; set; }
        [Browsable(false)]
        public string manacost { get; set; }       
        [Browsable(false)]
        public int cmc { get; set; }

        public string type { get; set; }
        public string pt { get; set; }
        //public int tablerow { get; set; } = 2;
        public string text { get; set; }
        public string manacostAuto
        {
            get { return manacost; }
            set
            {
                manacost = value;
                color = CardScraper.DetermineColor(value);
                cmc = CardScraper.DetermineCmC(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("manacost"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("color"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("cmc"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        void NameChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("name"));
        }

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
