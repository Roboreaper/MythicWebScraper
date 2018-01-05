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
        [Browsable(false)]
        public int ID { get; private set; }

        public Card(int id)
        {
            ID = id;
        }

        [Browsable(false)]
        public string set { get; set; }
        [Browsable(false)]
        public string ImageUrl { get; set; }
        [Browsable(false)]
        public string color { get; set; }
        [Browsable(false)]
        public string cost { get; set; }
        [Browsable(false)]
        public int cmc { get; set; }

        [Category("Not Editable")]
        public string Set { get { return set; }  }

        [Category("Not Editable")]
        public string Color { get { return color; } }

        [Category("Not Editable")]
        public string Cost { get { return cost; } }

        [Category("Not Editable")]
        public int CMC { get { return cmc; } }


        private string name;
        [Category("Editable")]
        public string Name { get { return name; } set { name = value; NameChanged(); } }

        [Category("Editable")]
        public string type { get; set; }
        [Category("Editable")]
        public string pt { get; set; }
        [Category("Editable")]
        public string text { get; set; }
        [Category("Editable")]
        public string manacost
        {
            get { return cost; }
            set
            {
                cost = value.ToUpper();
                color = CardScraper.DetermineColor(cost);
                cmc = CardScraper.DetermineCmC(cost);
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
            return $"{cost} {name}";
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

            if (!string.IsNullOrEmpty(cost))
                writer.WriteElementString("manacost", cost);

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
