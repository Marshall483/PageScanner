using System;
using System.Collections.Generic;
using TestNET.DATA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TestNET.DATA.Interfaces
{
    public interface IScanner
    {
        public async Task<IEnumerable<string>> InitiateScan() {
            return new List<string>();
        }
        public void ConfigureScanner(Config config);
        public async Task<string> GetHtmlStringAsync(string url){
            return "";
        }

    }
}
