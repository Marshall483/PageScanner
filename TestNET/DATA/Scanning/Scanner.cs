using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestNET.DATA.Scanning;
using TestNET.DATA.Interfaces;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.Text;
using TestNET.DATA.Models;

namespace TestNET.DATA.Scanning
{
    public class Scanner : IScanner
    {
        private static WebRequest _request;
        private static WebResponse _response;
        private static Encoding _encode = Encoding.GetEncoding("utf-8");

        private string _parentUrl;
        private string _domen;
        private int _constraint;
        private int _deep;

        private List<Dictionary<string, string>> _urlBodyPairs; 

        public void ConfigureScanner(Config config)
        {
            _urlBodyPairs = new List<Dictionary<string, string>>();
            _domen = ParseDomen(config.Url);
            _constraint = config.Constraint;
            _parentUrl = config.Url;
            _deep = config.Deep;
        }

        private string ParseDomen(string url)
        {
            int index = url.IndexOf("//");
            index++;
            StringBuilder sb = new StringBuilder();
            char cur = url[++index];

            while (!cur.Equals('/'))
            {
                sb.Append(cur);
                cur = url[++index];
            }

            return sb.ToString();
        }

        public IEnumerable<Dictionary<string, string>> Scan()
        {
            List<string> nextScan = ScanPage(_parentUrl);
            List<string> temp = new List<string>();

            for (int deep = 0; deep < _deep; deep++)
            {
                foreach (string link in nextScan)
                    temp.Union(ScanPage(link));
                nextScan = temp;
            }

            return _urlBodyPairs;
        }


        public List<string> ScanPage(string url)
        {
            var html = new HtmlDocument();
            html.LoadHtml(GetHtmlString(url));

            var nodes = FindNodesWichLinks(html);
            var links = ExtractDomenLinks(nodes);
            _urlBodyPairs.Add(SetHtml(links));

            return links;
        }

        private HtmlNodeCollection FindNodesWichLinks(HtmlDocument html) =>
            html.DocumentNode.SelectNodes("//a/@href");

        private Dictionary<string, string> SetHtml(IEnumerable<string> links)
        {
            Dictionary<string, string> pages = new Dictionary<string, string>();

            foreach (string link in links)
                pages[link] = GetHtmlString(link);

            return pages;
        }

        private List<string> ExtractDomenLinks(HtmlNodeCollection nodes)
        {
            List<string> hrefs = new List<string>();

            for (int i = 0; i < nodes.Count; i++)
                hrefs.Add(nodes[i].Attributes["href"].Value);

            return hrefs.Where(href => href.Contains(_domen)).
                Take(_constraint).ToList();
        }

        private string GetHtmlString(string url)
        {
            _request = WebRequest.Create(url);
            _request.Proxy = null;
            _response = _request.GetResponse();
            using (StreamReader sReader = new StreamReader(_response.GetResponseStream(), _encode))
            {
                return sReader.ReadToEnd();
            }
        }
    }
}
