using System;
using System.Collections.Generic;
using TestNET.DATA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TestNET.DATA.Interfaces
{
    public interface IScanner
    {
        public IEnumerable<Dictionary<string, string>> Scan();
        public void ConfigureScanner(Config config);
    }
}
