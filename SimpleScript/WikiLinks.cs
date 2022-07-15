using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;using System.IO;
using Newtonsoft.Json;

namespace SimpleScript
{
    public class FinalResult
    {
        // AllFoundLinks
        public List<string> AllFoundLinks= new List<string>();
        //TotalCount
        public int TotalCount { get; set; }
        // UniqueCount
        public int UniqueCount { get; set; }
    }


    class WikiLinks
    {
        public int terminator;
        public List<string> ALLLinks = new List<string>();
        public List<string> VisitedUrls = new List<string>();
        public List<string> QueueUrls = new List<string>();
        private  async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            VisitedUrls.Add(fullUrl);
            QueueUrls.Remove(fullUrl);
            return response;
        }

        public void Wikipedia(String Link, int Terminator)
        {
           if (!Link.StartsWith("https://en.wikipedia"))
                throw new Exception("Given Link is not wikipedia, Sorry I can proceed.");

                terminator = Terminator;
                var response = CallUrl(Link).Result;
                //From respons we html souce code 
                var linkList = ParseHtml(response);
                //linkList has all html links
                loopThrough(linkList);
                Console.WriteLine("All found links:" + ALLLinks.Count + "\n total count : " + VisitedUrls.Count + "\nUnique count : " + ALLLinks.Distinct().Count());
                //Creating the object for finalresult for JSON File.
                FinalResult FR = new FinalResult()
                {
                    //AllFoundLinks
                    AllFoundLinks = ALLLinks
                    // TotalCount
                    ,
                    TotalCount = ALLLinks.Count,
                    //UniqueCount
                    UniqueCount = ALLLinks.Distinct().Count()
                };
                Console.WriteLine("JSON converted string: ");
                // convert to Json string by seralization of the instance of class.
                string stringjson = JsonConvert.SerializeObject(FR);
                Console.WriteLine(stringjson);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "Links.JSON",
                      stringjson.ToString());
                
            

        }

        private void loopThrough(List<string> linkList)
        {

            foreach (var single in linkList)
            {
                if (!VisitedUrls.Contains(single)) // if link already visited we dont need to revisit again.
                {
                    var response = CallUrl(single).Result;
                    var SublinkList = ParseHtml(response);

                    if (terminator <= 0)
                        break;

                    //LOOPING SUBLinkLISTS
                    if (SublinkList.Count > 0)
                    {
                        loopThrough(SublinkList);
                        if (terminator == 0)
                            return;
                    }
                }
            }
        }

        private List<string> ParseHtml(string html)
        {
            List<string> PlainRunList = new List<string>();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection")).ToList();
            try
            {
                foreach (var link in programmerLinks)
                {
                    if (link.FirstChild.Attributes.Count > 0 && link.FirstChild.Attributes[0].Value.StartsWith("/wiki/") && link.FirstChild != null)
                    {
                        ALLLinks.Add(link.FirstChild.Attributes[0].Value.StartsWith("http") ? link.FirstChild.Attributes[0].Value : "https://en.wikipedia.org" + link.FirstChild.Attributes[0].Value);
                        PlainRunList.Add(link.FirstChild.Attributes[0].Value.StartsWith("http") ? link.FirstChild.Attributes[0].Value : "https://en.wikipedia.org" + link.FirstChild.Attributes[0].Value);
                    }
                }
            }
            catch (Exception exp) { }
            QueueUrls.AddRange(PlainRunList.Distinct().ToList());
            terminator--;
            return QueueUrls;
        }
    }
}
