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
            var res = _scanner.Scan();

            List<string> links = new List<string>();
            List<string> linksHtml = new List<string>();

            foreach(var dic in res)
                foreach(var linkAndHtml in dic)
                {
                    links.Add(linkAndHtml.Key);
                    linksHtml.Add(linkAndHtml.Value);
                }


            SaveModel model = new SaveModel();
            model.linksHtml = links;
            model.scannedLinks = linksHtml;

            return View(model);

        }

        public IActionResult Save(SaveModel model)
        {

            int a = 0;

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
