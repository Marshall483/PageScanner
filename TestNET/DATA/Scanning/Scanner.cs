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
using System.Collections.Concurrent;

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

        private static List<string> _alreadyScanned = new List<string>();

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


        public async Task<IEnumerable<string>> InitiateScan()
        {
            ConcurrentQueue<string> heapToScan = new ConcurrentQueue<string>();
            heapToScan.Enqueue(_parentUrl);
            List<string> nextSpin = new List<string>();
            _alreadyScanned.Add(_parentUrl);
            string nextUrl = "";

            for(int deep = 0; deep < _deep; deep++)
            {
                while (!heapToScan.Count.Equals(0))
                {
                    if(heapToScan.TryDequeue(out nextUrl));
                        List<string> newLinks = await ScanPage(nextUrl);

                    newLinks = newLinks.Except(_alreadyScanned).ToList();
                    _alreadyScanned = _alreadyScanned.Union(newLinks).ToList();
                    nextSpin = nextSpin.Union(newLinks).ToList();
                }
                foreach (var nextLink in nextSpin)
                    heapToScan.Enqueue(nextLink);

                nextSpin.Clear();
            }

            return _alreadyScanned;
        }

        /// <summary>
        ///  Scanning one page and return a List wich _constraint elemnts
        /// </summary>
        /// <param name="url"> URL page </param>
        /// <returns></returns>
        public async Task<List<string>> ScanPage(string url)
        {
            var html = new HtmlDocument();
            var htmlString = await GetHtmlStringAsync(url);
            html.LoadHtml(htmlString);

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

            return hrefs.Except(_alreadyScanned).Where(href => href.Contains(_domen)).
                Take(_constraint).ToList();
        }

        public async Task<string> GetHtmlStringAsync(string url)
        {
            _request = WebRequest.Create(url);
            _request.Proxy = null;
            
            _response = await _request.GetResponseAsync();

            using (StreamReader sReader = new StreamReader(_response.GetResponseStream(), _encode))
            {
                return await sReader.ReadToEndAsync();
            }
        }

    }
}
