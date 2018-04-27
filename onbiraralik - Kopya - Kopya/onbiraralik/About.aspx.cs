using HtmlAgilityPack;
using NUglify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace onbiraralik
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public ISet<string> GetNewLinks(string content)
        {
            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");

            ISet<string> newLinks = new HashSet<string>();
            foreach (var match in regexLink.Matches(content))
            {
                if (!newLinks.Contains(match.ToString()))
                    newLinks.Add(match.ToString());
            }

            return newLinks;
        }
        private Label[] lbl;
        public void  sayac(string url,string aranan)
        {
            string arananKelime;
            int[] sayacim = new int[200];
            string[] urller = url.Split(' ');
            string[] kelimeGrubu = aranan.Split(' ');
            int[] skor = new int[100];
            for (int i = 0; i < urller.Length; i++)
            {
                WebResponse myWebRes;
                string url_deger = urller[i];
                WebRequest myWebRequest = WebRequest.Create(url_deger);
                myWebRes = myWebRequest.GetResponse();
                Stream streamResponse = myWebRes.GetResponseStream();


                StreamReader sreader = new StreamReader(myWebRes.GetResponseStream(), System.Text.Encoding.UTF8);//reads the data stream

                string icerik = sreader.ReadToEnd();
                string Links = GetNewLinks(icerik).ToString();

                var result = Uglify.HtmlToText(icerik);
                string icerik2 = result.Code;
                int skorDeger = 0;
                for (int j = 0; j < kelimeGrubu.Length; j++)
                {
                    arananKelime = kelimeGrubu[j];
                    sayacim[j] = Regex.Matches(icerik2.ToLower(), arananKelime.ToLower()).Count;
                    //skorDeger = ((sayacim.Min() * kelimeGrubu.Length) * 10000) + (sayacim.Max() - sayacim.Min());

                }

                int min = sayacim[0];
                int mak = sayacim[0];
                for (int k = 1; k < kelimeGrubu.Length; k++)
                {
                    if (min > sayacim[k])
                        min = sayacim[k];
                    if (mak < sayacim[k])
                        mak = sayacim[k];
                }


                    skorDeger = ((min * kelimeGrubu.Length) * 100) + (mak - min);
                

                skor[i] = skorDeger;              
            }

            int  en_kucuk, aklimda;
            string aklimdaUrl;
            for (int i = 0; i < urller.Length - 1; i++)
            {
                en_kucuk = i;
                for (int j = i + 1; j < urller.Length ; j++)
                {
                    if (skor[j] < skor[en_kucuk])
                    {
                        en_kucuk = j;
                       
                    }
                }
                aklimda = skor[i];
                skor[i] = skor[en_kucuk];
                skor[en_kucuk] = aklimda;
                aklimdaUrl = urller[i];
                urller[i] = urller[en_kucuk];
                urller[en_kucuk] = aklimdaUrl;
   
            }

            int uste_uzaklik = 500;
            lbl = new Label[urller.Length];
            
            for (int i= urller.Length-1 ; i>=0 ; i--)
            {
                lbl[i] = new Label();
                lbl[i].Style["Position"] = "Absolute";
                lbl[i].Style["Top"] = uste_uzaklik.ToString() + "px";
                lbl[i].Style["Left"] = "100px";
                lbl[i].Font.Bold = true;
                lbl[i].Font.Name = "Verdana";
                lbl[i].ID = i.ToString();
                lbl[i].Text = urller[i].ToString()+" skor:  " + skor[i].ToString() ;
                Panel1.Controls.Add(lbl[i]);
                uste_uzaklik += 30;

            }
            //for (int i =kelimeGrubu.Length-1 ; i>=0 ; i--)
            //{
            //    lbl[i] = new Label();
            //    lbl[i].Style["Position"] = "Absolute";
            //    lbl[i].Style["Top"] = uste_uzaklik.ToString() + "px";
            //    lbl[i].Style["Left"] = "300px";
            //    lbl[i].Font.Bold = true;
            //    lbl[i].Font.Name = "Verdana";
            //    lbl[i].ID = i.ToString();
            //    lbl[i].Text = urller[i].ToString() + skor[i].ToString();
            //    Panel1.Controls.Add(lbl[i]);
            //    uste_uzaklik += 30;
            //}

        }

        protected void Button2_Click2(object sender, EventArgs e)
        {
            string urller;
            string kelimeGrubu;
            urller = textBox1.Text;
            kelimeGrubu = textBox2.Text;
            sayac(urller, kelimeGrubu);

        }



    }
}