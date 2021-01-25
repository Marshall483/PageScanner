using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestNET.Models;
using TestNET.DATA.Models;
using TestNET.DATA.Scanning;
using TestNET.DATA.Interfaces;
using TestNET.ViewModels;
using TestNET.DATA;

namespace TestNET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IScanner _scanner;
        private readonly IHtmlData _dbHtmlData;

        public HomeController(ILogger<HomeController> logger, IScanner scanner, IHtmlData htmlData, AppDBContent content)
        {
            _logger = logger;
            _scanner = scanner;
            _dbHtmlData = htmlData;
        }

        [HttpPost]
        public async Task<IActionResult> SetConfig(Config config)
        {
            _scanner.ConfigureScanner(config);
            var res = await _scanner.InitiateScan();

            SaveModel model = new SaveModel();
            model.Source = config.Url;
            model.ContentString = String.Join('|', res);
            model.ScannedLinks = res;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(SaveModel model)
        {
            int added = 0;
            List<Task<string>> tsks = new List<Task<string>>();

            var links = model.ContentString.Split('|');

            foreach (var link in links)
                tsks.Add(_scanner.GetHtmlStringAsync(link));

            var htmls = await Task.WhenAll(tsks);

            for (int i = 0; i < links.Length; i++)
                if (_dbHtmlData.AddLink(links[i], htmls[i]))
                    added++;

            ViewBag.Added = added;

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
