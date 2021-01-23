using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestNET.ViewModels
{
    public class SaveModel
    {
        public string Source { get; set; }
        public string ContentString { get; set; }
        public IEnumerable<string> ScannedLinks { get; set; }

    }
}
