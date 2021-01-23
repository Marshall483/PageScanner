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

        public void ConfigureScanner(Config config)
        {
            //Значит цикл в _deep оборотов, в котором на каждом этапе добавляем полученные ссылки 
            //в очередь, которой будем вереть на следующем этапе.
            // вернем перечисление отсканированнх сайтов

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


        public IEnumerable<string> InitiateScan()
        {
            Queue<string> heapToScan = new Queue<string>();
            heapToScan.Enqueue(_parentUrl);
            List<string> nextSpin = new List<string>();
            List<string> aleradyScanned = new List<string>();
            aleradyScanned.Add(_parentUrl);

            for(int deep = 0; deep < _deep; deep++)
            {
                while (!heapToScan.Count.Equals(0))
                {
                    string nextUrl = heapToScan.Dequeue();
                    List<string> newLinks = ScanPage(nextUrl);

                    newLinks = newLinks.Except(aleradyScanned).ToList();
                    aleradyScanned = aleradyScanned.Union(newLinks).ToList();
                    nextSpin = nextSpin.Union(newLinks).ToList();
                }
                foreach (var nextLink in nextSpin)
                    heapToScan.Enqueue(nextLink);

                nextSpin.Clear();
            }

            return aleradyScanned;
        }


        public List<string> ScanPage(string url)
        {
            var html = new HtmlDocument();
            html.LoadHtml(GetHtmlString(url));

            var nodes = FindNodesWichLinks(html);
            var links = ExtractDomenLinks(nodes);

            return links;
        }

        private HtmlNodeCollection FindNodesWichLinks(HtmlDocument html) =>
            html.DocumentNode.SelectNodes("//a/@href");

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
