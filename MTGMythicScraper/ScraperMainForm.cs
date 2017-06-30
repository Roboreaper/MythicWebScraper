using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MTGMythicScraper
{
    public partial class ScraperMainForm : Form
    {
        List<Card> Cards = new List<Card>();

        LinkScraper Linkscraper = new LinkScraper();
        CardScraper Cardscaper = new CardScraper();

        string siteUrl = "http://mythicspoiler.com/";
        string Set = "";

        CancellationTokenSource tokenSource = new CancellationTokenSource();        

        public ScraperMainForm()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void buttonScanPage_Click(object sender, EventArgs e)
        {
            if( string.IsNullOrEmpty( textBoxSet.Text))
            {
                MessageBox.Show(this, "Enter Set Code");
                return;
            }

            buttonExport.Enabled =  buttonScanPage.Enabled = false;
            Set = textBoxSet.Text;

            var sourceCode = "";
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                sourceCode = client.DownloadString("http://mythicspoiler.com/" + Set + "/");
            }

            
            var result = Linkscraper.Find(sourceCode);
            Console.WriteLine($"found {result.Count} cards");

            propertyGrid.SelectedObject = null;
            listView1.Items.Clear();
            foreach (var c in Cards)
            {
                c.PropertyChanged -= CardChanged;
            }

            GetCards(result);

        }

        private void CardChanged(object sender, PropertyChangedEventArgs e)
        {
            FillList();
        }

        private async void  GetCards(List<CardLink> links)
        {

            progressBar.Minimum = 0;
            progressBar.Maximum = links.Count;
            progressBar.Step = 1;
            progressBar.Value = 0;

            tokenSource = new  CancellationTokenSource();
            buttonCancel.Enabled = true;

           await Task.Run(() =>
           {
               var cardCount = links.Count;
               var progress = 0; var temp = 0; var tenth = cardCount / 10;
               foreach (var link in links)
               {
                   if (tokenSource.Token.IsCancellationRequested)
                       break;

                   string cardPage = "";
                   var img = siteUrl + Set + "/" + link.ImgUrl;

                   string tempUrl = siteUrl + textBoxSet.Text + "/" + link.Url;
                   try
                   {
                       using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                       {
                           cardPage = client.DownloadString(tempUrl);

                           var card = Cardscaper.Scrape(cardPage, Set,img);
                           
                           if (string.IsNullOrEmpty(card.Name))
                           {
                               Console.WriteLine("Error for: " + link.Url);

                               var c = new Card() { Name = "Error: " + link.Url.Split('/', '.')[1], ImageUrl = img, set= Set };
                               Cards.Add(c);
                               TempAddCard(c);

                               continue;
                           }

                           Cards.Add(card);
                           TempAddCard(card);
                       }
                   }
                   catch (Exception e)
                   {
                       Console.WriteLine("Timout for Error for: " + link.Url);

                       var c = new Card() { Name = "Error: " + link.Url.Split('/', '.')[1], ImageUrl = img, set = Set };
                       Cards.Add(c);
                       TempAddCard(c);

                   }

                   ++temp;
                   if (temp >= tenth)
                   {
                       temp = 0;
                       progress += 10;
                       Console.WriteLine($"{progress}%...");
                   }

                   UpdateProgressBar();
               }
           }, tokenSource.Token);

            Console.WriteLine("100%");
            progressBar.Value = progressBar.Maximum;
            labelPercent.Text = "Done.";

            buttonCancel.Enabled = false;

            foreach (var c in Cards)
            {
                c.PropertyChanged += CardChanged;
            }

            FillList();
            buttonExport.Enabled = buttonScanPage.Enabled = true;

        }

        private void TempAddCard(Card card)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { TempAddCard(card); }));
            }
            else
            {
                listView1.Items.Add(new ListViewItem(card.Name) { Tag = card });
            }           
        }

        private void FillList()
        {
            propertyGrid.SelectedObject = null;
            listView1.Items.Clear();
            var ordered = Cards.OrderBy(c => c.Name).Select(c => new ListViewItem(c.Name) { Tag = c }).ToArray();
            listView1.Items.AddRange(ordered);
        }

        void UpdateProgressBar()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { UpdateProgressBar(); }));
            }
            else
            {
                this.progressBar.PerformStep();
                labelPercent.Text = $"{progressBar.Value}/{progressBar.Maximum}";
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",

            };

            var path = System.IO.Path.Combine(Application.StartupPath, "cocatrice_" + Set + ".xml");
            using (var writer = XmlWriter.Create(path, xmlWriterSettings))
            {
                Console.WriteLine("Creating XML");

                writer.WriteStartDocument();

                //writer.WriteStartElement("set");
                //writer.WriteElementString("longname", "Commander 2016");
                //writer.WriteElementString("settype", "Commander");
                //writer.WriteElementString("releasedate", DateTime.Now.ToString("YYYY-MM-dd"));
                //writer.WriteEndElement();

                writer.WriteStartElement("cards");

                foreach (var card in Cards)
                {
                    if (card.Name.StartsWith("Error:"))
                        continue;

                    card.Serialize(writer);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Console.WriteLine("done");

            Process.Start(Application.StartupPath);

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;

            var tag = listView1.SelectedItems[0]?.Tag;

            if (tag == null)
                return;

            if (tag is Card)
            {
                propertyGrid.SelectedObject = tag;
                pictureBox1.ImageLocation = ((Card)tag).ImageUrl;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            tokenSource.Cancel();
        }
    }
}
