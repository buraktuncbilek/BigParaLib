using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigParaDovizLib
{
    public enum DövizKodu
    {
        USD,
        EUR,
        GBP
    }

    public static class BigParaDöviz
    {
        public static Uri BaseUrl = new Uri("https://bigpara.hurriyet.com.tr/doviz/");
        private static readonly CultureInfo tr_culture = CultureInfo.GetCultureInfo("tr-TR");

        /// <summary>
        /// Dolar, Euro ve sterlin alış, satış ve değişim oranını verir.
        /// </summary>
        /// <returns>Kur kodu bazlı liste dönecektir.</returns>
        public static List<DovizTip> TümKurlarıVer()
        {
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(BaseUrl);

                var aa = doc.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("dovizBar"));

                if (aa != null) doc.LoadHtml(aa.InnerHtml);

                var dovizler = doc.DocumentNode.Descendants().Where(x => x.HasClass("dbItem"));


                return (from doviz in dovizler
                    let names = doviz.Descendants().Where(x => x.HasClass("name")).ToList()
                    let values = doviz.Descendants().Where(x => x.HasClass("value")).ToList()
                    select new DovizTip()
                    {
                        Döviz = names[0].InnerText.Trim(),
                        DovizKoduString = names[0].InnerText.Contains("olar") ? "USD" : (names[0].InnerText.Contains("uro") ? "EUR" : "GBP"),
                        DovizKodu = names[0].InnerText.Contains("olar") ? DövizKodu.USD : (names[0].InnerText.Contains("uro") ? DövizKodu.EUR : DövizKodu.GBP),
                        Alış = Convert.ToDecimal(values[1].InnerText.Trim(), tr_culture),
                        Satış = Convert.ToDecimal(values[2].InnerText.Trim(), tr_culture),
                        DeğişimOranı = Convert.ToDecimal(values[0].InnerText.Replace("%", "").Trim(), tr_culture)
                    }).ToList();
            }
            catch (Exception)
            {
                throw;
                return new List<DovizTip>();
            }
        }

        /// <summary>
        /// Seçilen döviz koduna ait kur bilgisi döner.
        /// </summary>
        /// <param name="DovizKodu">Döviz kodu tipi</param>
        /// <returns>Kur bilgisi ve değişim oranı. Detaylar için ToString çağır.</returns>
        public static DovizTip DövizKoduİleKurVer(DövizKodu DovizKodu)
        {
            return TümKurlarıVer().FirstOrDefault(x => x.DovizKodu == DovizKodu);
        }

        public class DovizTip
        {
            public string Döviz { get; set; }
            public string DovizKoduString { get; set; }
            public DövizKodu DovizKodu { get; set; }
            public decimal DeğişimOranı { get; set; }
            public decimal Alış { get; set; }
            public decimal Satış { get; set; }

            /// <summary>
            /// Özet Bilgi verir.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"Döviz Kodu: {this.DovizKodu}{Environment.NewLine}Değişim Oranı: {this.DeğişimOranı} %{Environment.NewLine}Alış TL: {this.Alış}{Environment.NewLine}Satış TL: {this.Satış}";
            }
        }
    }
}
