using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;

namespace SimpleScript
{
    [TestClass]
    public class WikiLinkTest
    {
        [TestMethod]
        public void WikiTest()
        {
            WikiLinks val = new WikiLinks();
            val.Wikipedia("https://en.wikipedia.org/wiki/Help:Introduction",1); 
        }
    }
}
