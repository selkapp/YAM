using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Office.Interop.Word;
using WordApplication = Microsoft.Office.Interop.Word.Application;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using NUglify;

namespace onbiraralik
{
    public partial class Asama4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button2_Click2(object sender, EventArgs e)
        {
            string urller;
            string kelimeGrubu;
            urller = textBox1.Text;
            kelimeGrubu = textBox2.Text;
            sayac(urller, kelimeGrubu);
        }



        private static string urlPattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        private static string tagPattern = @"<a\b[^>]*(.*?)</a>";
        private static string emailPattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
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
        private string ConvertUrlsToLinks(string msg)
        {
            string regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            return r.Replace(msg, "<a href=\"$1\" title=\"Click to open in a new window or tab\" target=\"&#95;blank\">$1</a>").Replace("href=\"www", "href=\"http://www");
        }








        private Label[] lbl;
        public void sayac(string url, string aranan)
        {
            string arananKelime;
            int[] sayacim = new int[200];
            int[] sayacim2 = new int[10];

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
//--------------------------------------------------------------------------------------------------------------------------------------------

                Microsoft.Office.Interop.Word.Application appWord;      // word application var
                object objNull = null;      // word object method calls require
                                            // references to objects... create
                                            // object for null and
                object objFalse = false;      // false entries and language

                object objLanguage = Microsoft.Office.Interop.Word.WdLanguageID.wdTurkish; // or appropritate lang!

                // Try opening Word app
                appWord = new Microsoft.Office.Interop.Word.Application();
                List<string> synonym = new List<string>();
                synonym.Clear();


               
                // now call get_SynonymInfo to get SynonymInfo structure for
                // word entered in TextBox tbWord
                Microsoft.Office.Interop.Word.SynonymInfo si = appWord.get_SynonymInfo(kelimeGrubu[0], ref (objLanguage));


                // first find out how many meanings were found for word
                int iMeanings = (int)si.MeaningCount;

                if (iMeanings > 0)
                {
                    var strMeanings = si.MeaningList as Array;
                    if (strMeanings != null)
                        foreach (var strMeaning in strMeanings)
                        {
                            // get Synonym List for each meaning... note that
                            // get_SynonymList takes an object ref, thus we
                            // must create objMeaning object
                            var objMeaning = strMeaning;

                            var aSynonyms = si.SynonymList[objMeaning];

                            var strSynonyms = si.SynonymList[objMeaning] as Array;
                            if (strSynonyms != null)
                                foreach (string strSynonym in strSynonyms)
                                {
                                    // loop over each synonym in ArrayList
                                    // and add to lbSynonym ListBox
                                    synonym.Add(strSynonym);
                                }
                        }
                }
                else
                {
                    // no meanings/synonyms found... set ListBox value to "NONE"
                    synonym.Add("NONE");
                }
                // Clean up COM object

                // quit WINWORD app
                appWord.Quit(ref objFalse, ref objNull, ref objNull);
                Label1.Text = " "; 
                Label1.Text += synonym[i] + " ";
                //---------------------------------------------------------------------------------------------------------------------------------------

                for (int j = 0; j < kelimeGrubu.Length; j++)
                {
                    arananKelime = kelimeGrubu[j];
                    sayacim[j] = Regex.Matches(icerik2.ToLower(), arananKelime.ToLower()).Count;
                    //skorDeger = ((sayacim.Min() * kelimeGrubu.Length) * 10000) + (sayacim.Max() - sayacim.Min());

                }

                    arananKelime = synonym[0];
                    sayacim2[0] = Regex.Matches(icerik2.ToLower(), arananKelime.ToLower()).Count;
                    //skorDeger = ((sayacim.Min() * kelimeGrubu.Length) * 10000) + (sayacim.Max() - sayacim.Min());

                

                sayacim[0] += sayacim2[0];
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

            int en_kucuk, aklimda;
            string aklimdaUrl;
            for (int i = 0; i < urller.Length - 1; i++)
            {
                en_kucuk = i;
                for (int j = i + 1; j < urller.Length; j++)
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

            for (int i = urller.Length - 1; i >= 0; i--)
            {
                lbl[i] = new Label();
                lbl[i].Style["Position"] = "Absolute";
                lbl[i].Style["Top"] = uste_uzaklik.ToString() + "px";
                lbl[i].Style["Left"] = "100px";
                lbl[i].Font.Bold = true;
                lbl[i].Font.Name = "Verdana";
                lbl[i].ID = i.ToString();
                lbl[i].Text = urller[i].ToString() + " skor:  " + skor[i].ToString();
                Panel1.Controls.Add(lbl[i]);
                uste_uzaklik += 30;

            }


        }





    }
}