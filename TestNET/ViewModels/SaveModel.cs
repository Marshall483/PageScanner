using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestNET.ViewModels
{
    public class SaveModel
    {

        public IEnumerable<string> scannedLinks { get; set; }
        public IEnumerable<string> linksHtml { get; set; }

    }
}
