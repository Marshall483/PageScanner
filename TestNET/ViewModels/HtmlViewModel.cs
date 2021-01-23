using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestNET.ViewModels
{
    public class HtmlViewModel
    {
        public string Parent_Link { get; set; }
        public int Deep { get; set; }
        public IEnumerable<string> Child_Links { get; set; }

    }
}
