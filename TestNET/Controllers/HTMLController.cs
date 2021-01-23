using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestNET.DATA.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TestNET.ViewModels;

namespace TestNET.Controllers
{
    public class HTMLController : Controller
    {

        private readonly IHtmlData _htmlData;

        public HTMLController(IHtmlData htmlData)
        {
            _htmlData = htmlData;
        }

        public ViewResult List()
        {
            var viewModel = new HtmlViewModel();
            viewModel.Parent_Link = "google.com";
            viewModel.Deep = 3;
            viewModel.Child_Links = new List<string>
            {
                "Amazon.com",
                "Aliexppes.com",
                "Apple.com"
            };
            
            return View(viewModel);
        }
    }
}
