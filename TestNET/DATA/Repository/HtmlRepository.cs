using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestNET.DATA.Models;
using TestNET.DATA.Interfaces;
using TestNET.DATA;

namespace TestNET.DATA.Repository
{
    public class HtmlRepository : IHtmlData
    {

        private volatile AppDBContent _dBContent;

        public HtmlRepository(AppDBContent context) =>
            _dBContent = context;

        public IEnumerable<HTMLSource> GetAllHtml => _dBContent.Sources;

        public bool AddLink(string link, string html)
        {
            _dBContent.Sources.Add(new HTMLSource { Link = link, Content = html });

            try { 
                _dBContent.SaveChanges();
            }
            catch(Exception) { 
                return false; 
            }
            return true;
        }
    }
}
