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

namespace TestNET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IScanner _scanner;

        public HomeController(ILogger<HomeController> logger,IScanner scanner )
        {
            _logger = logger;
            _scanner = scanner;
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

        public IActionResult Save(string scanned)
        {
            string[] links = scanned.Split('|');
            


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
