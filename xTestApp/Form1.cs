using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BigParaDovizLib;

namespace xTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Uri BaseURL = new Uri("https://bigpara.hurriyet.com.tr/doviz/");
        private void button1_Click(object sender, EventArgs e)
        {
            var web = new HtmlWeb();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc = web.Load(BaseURL);

            var aa = doc.DocumentNode.Descendants().Where(x => x.HasClass("dovizBar")).FirstOrDefault();


            doc.LoadHtml(aa.InnerHtml);

            var dovizler = doc.DocumentNode.Descendants().Where(x => x.HasClass("dbItem"));

            List<DovizTip> list = new List<DovizTip>();

            var tr_culture = CultureInfo.GetCultureInfo("tr-TR");

            foreach (var doviz in dovizler)
            {
                /*
                     * 0: "ABD Doları"
                     * 1: ALIŞ(TL)
                     * 2: SATIŞ(TL)
                     */
                var names = doviz.Descendants().Where(x => x.HasClass("name")).ToList();

                /*
                 * 0: 1,85 %
                 * 1: 27,4215
                 * 2: 27,4278
                 */
                var values = doviz.Descendants().Where(x => x.HasClass("value")).ToList();

                list.Add(new DovizTip()
                {
                    Doviz = names[0].InnerText.Trim(),
                    DovizKodu = names[0].InnerText.Contains("olar") ? "USD" : (names[0].InnerText.Contains("uro") ? "EUR" : "GBP"),
                    Alış = Convert.ToDecimal(values[1].InnerText.Trim(), tr_culture),
                    Satış = Convert.ToDecimal(values[2].InnerText.Trim(), tr_culture),
                    DeğişimOranı = Convert.ToDecimal(values[0].InnerText.Replace("%", "").Trim(), tr_culture)
                });
            }

            list = list;

        }

        public class DovizTip
        {
            public string Doviz { get; set; }
            public string DovizKodu { get; set; }
            public decimal DeğişimOranı { get; set; }
            public decimal Alış { get; set; }
            public decimal Satış { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Enum.GetNames(typeof(BigParaDovizLib.DövizKodu)).ToList().ForEach(x => comboBox1.Items.Add(x));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dt=(DövizKodu)Enum.Parse(typeof(DövizKodu),comboBox1.Text);
            var kur = BigParaDöviz.DövizKoduİleKurVer(dt);
            MessageBox.Show(kur.ToString(),"Kur bilgisi");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = BigParaDöviz.TümKurlarıVer();
        }
    }
}
