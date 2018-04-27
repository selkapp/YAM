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
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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






        private static List<string> getMatches(string source)
        {
            var matchesList = new List<string>();
            //reg expression for A Tag [html] 
            //get the collection that match the pattern
            MatchCollection matches = Regex.Matches(source, tagPattern);
            //add the text under the href attribute
            //to the list
            foreach (Match match in matches)
            {
                string val = match.Value.Trim();
                if (val.Contains("href=\""))
                {
                    string link = getSubstring(val, "href=\"", "\"");
                    matchesList.Add(link);
                }
            }
           
            return matchesList;
        }
        private static string getSubstring(string source, string start, string end)
        {
            int startIndex = source.IndexOf(start) + start.Length;
            int length = source.IndexOf(end, startIndex) - startIndex;
            return source.Substring(startIndex, length);
        }
        private static string getAblosuteUrl(string domainName, string path)
        {
            string absoluteUrl = "";
            if (domainName[domainName.Length - 1] == '/')
            {
                absoluteUrl += domainName;
            }
            else
            {
                absoluteUrl += domainName + '/';
            }

            if (path.Contains("../"))
            {
                string temp = domainName.Substring(0, domainName.LastIndexOf('/', 6));
                temp = temp.Substring(0, temp.LastIndexOf('/', 5));
                absoluteUrl = temp + path.Substring(3);
                return absoluteUrl;
            }
            if (path.Contains("./"))
            {
                string temp = domainName.Substring(0, domainName.LastIndexOf('/', 1));
                absoluteUrl = temp + path.Substring(2);
                return absoluteUrl;
            }
            if (path.Contains("/"))
            {
                absoluteUrl += path.Substring(1);
                return absoluteUrl;
            }
            absoluteUrl += path;

            return absoluteUrl;
        }

        private static string getDomainName(string url)
        {
            int length = url.IndexOf('/', 8);
            string domainName = url.Substring(0, length);
            return domainName;
        }









        private Label[] lbl;
        private Label[] lbl2;

        public void sayac(string url, string aranan)
        {
            string arananKelime;
            int[] sayacim = new int[200];
            string[] urller = url.Split(' ');
            string[] kelimeGrubu = aranan.Split(' ');
            int[] skor = new int[100];
            List<string> innerUrls = new List<string>();
            int uste_uzaklik2 = 500;
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

                innerUrls.Clear();


                List<string> links = getMatches(icerik);
                foreach (string link2 in links)
                {
                    if (!Regex.IsMatch(link2, urlPattern) && !Regex.IsMatch(link2, emailPattern))
                    {
                        string absoluteUrlPath = getAblosuteUrl(getDomainName(urller[i]), link2);
                        innerUrls.Add(absoluteUrlPath);
                    }
                    else
                    {
                        innerUrls.Add(link2);
                    }
                }


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

                string linkUrl = urller[i];
                string link = ConvertUrlsToLinks(linkUrl);
                urller[i] = link;
                skorDeger = ((min * kelimeGrubu.Length) * 100) + (mak - min);

                lbl2 = new Label[100];
                
                if(innerUrls.Count>1)
                    for (int f = 1; f < 7; f++)
                    {
                   
                        lbl2[f] = new Label();
                        lbl2[f].Style["Position"] = "Absolute";
                        lbl2[f].Style["Top"] = uste_uzaklik2.ToString() + "px";
                        lbl2[f].Style["Left"] = "800px";
                        lbl2[f].Font.Bold = true;
                        lbl2[f].Font.Name = "Verdana";
                        lbl2[f].ID = f.ToString();                  
                        lbl2[f].Text = innerUrls[f];
                        Panel1.Controls.Add(lbl2[f]);
                        uste_uzaklik2 += 30;
                    }
                if (innerUrls.Count==0||innerUrls.Count==1)
                    for (int f = 1; f < innerUrls.Count ; f++)
                    {

                        lbl2[f] = new Label();
                        lbl2[f].Style["Position"] = "Absolute";
                        lbl2[f].Style["Top"] = uste_uzaklik2.ToString() + "px";
                        lbl2[f].Style["Left"] = "800px";
                        lbl2[f].Font.Bold = true;
                        lbl2[f].Font.Name = "Verdana";
                        lbl2[f].ID = f.ToString();
                        lbl2[f].Text = innerUrls[f];
                        Panel1.Controls.Add(lbl2[f]);
                        uste_uzaklik2 += 30;
                    }
   
                uste_uzaklik2 += 50;
                

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
            //lbl2 = new Label[100];

            for (int i = urller.Length - 1; i >= 0; i--)
            {
                
                lbl[i] = new Label();
                lbl[i].Style["Position"] = "Absolute";
                lbl[i].Style["Top"] = uste_uzaklik.ToString() + "px";
                lbl[i].Style["Left"] = "100px";
                lbl[i].Font.Bold = true;
                lbl[i].Font.Name = "Verdana";
                lbl[i].ID = i.ToString();
                lbl[i].Text = urller[i] +" skor:  " + skor[i].ToString();
                Panel1.Controls.Add(lbl[i]);
                uste_uzaklik += 30;

            }
         


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