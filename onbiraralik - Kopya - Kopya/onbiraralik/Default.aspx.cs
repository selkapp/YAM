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
   

    public partial class _Default : Page
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

        



        protected void Button2_Click2(object sender, EventArgs e)
        {
            string arananKelime;
            int sayac = 0;

            WebResponse myWebRes;
            string url_deger = textBox1.Text;
            WebRequest myWebRequest = WebRequest.Create(url_deger);
            myWebRes = myWebRequest.GetResponse();
            Stream streamResponse = myWebRes.GetResponseStream();


            StreamReader sreader = new StreamReader(myWebRes.GetResponseStream(), System.Text.Encoding.UTF8);//reads the data stream

            string icerik = sreader.ReadToEnd();
            string Links = GetNewLinks(icerik).ToString();

            var result = Uglify.HtmlToText(icerik);
            arananKelime = TextBox4.Text;
            string icerik2 = result.Code;

            Label6.Text = Regex.Matches(icerik2.ToLower(), arananKelime.ToLower()).Count.ToString();
            // int konum = arananKelime.IndexOf(arananKelime);

            //while (konum != -1)
            //{
            //    konum = icerik2.IndexOf(arananKelime, konum + 1);          
            //    sayac++;
            //}


            // Label6.Text = Convert.ToString(sayac);

            Label7.Visible = true;
            Label8.Visible = true;
            Label9.Text = icerik2;

        }

        protected void TextBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}