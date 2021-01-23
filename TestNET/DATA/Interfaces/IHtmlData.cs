using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestNET.DATA.Models;

namespace TestNET.DATA.Interfaces
{
    public interface IHtmlData
    {
        IEnumerable<HTMLSource> GetAllHtml { get; }
    }
}
