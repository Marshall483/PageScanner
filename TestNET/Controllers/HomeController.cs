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
        public IActionResult SetConfig(Config config)
        {
            _scanner.ConfigureScanner(config);
            var res = _scanner.InitiateScan();

            SaveModel model = new SaveModel();
            model.Source = config.Url;
            model.ContentString = String.Join('|', res);
            model.ScannedLinks = res;

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(SaveModel model)
        {
            foreach (var link in model.ContentString.Split('|'))
            {
                var html = _scanner.GetHtmlString(link);
                _dbHtmlData.AddLink(link, html);
            }
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
