using System;
using System.Collections.Generic;
using TestNET.DATA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TestNET.DATA.Interfaces
{
    public interface IScanner
    {
        public IEnumerable<string> InitiateScan();
        public void ConfigureScanner(Config config);
        public string GetHtmlString(string url);

    }
}
