using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WsDeckDatabase.Download
{
    internal class SerialDownloader
    {
        private const string Url = @"http://ws-tcg.com/cardlist/";
        private const string ExpansionRegex = @"showExpansionDetail\('(\d+)',''\)";
        private const string PageRegex = @"page\('(\d+)','\d+'\)";

        public IEnumerable<string> Download()
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(Url);
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Any()) return null;
            var html = htmlDoc.DocumentNode.InnerHtml;
            IEnumerable<string> expansionSet = GetMatchRegex(html, ExpansionRegex).ToList();
            var serialSet = new HashSet<string>();
            var count = 0;
            foreach (var no in expansionSet)
            {
                serialSet.UnionWith(DowloadExpansion(no));
                Console.Out.WriteLine($"{no}:{++count}/{expansionSet.Count()}");
            }
            serialSet.RemoveWhere(string.IsNullOrWhiteSpace);
            return serialSet;
        }

        private static IEnumerable<string> DowloadExpansion(string expansionId)
        {
            var result = PostData($"expansion_id={expansionId}");
            ISet<string> serialSet = new HashSet<string>();
            serialSet.UnionWith(PrasePage(result));
            IEnumerable<string> pageSet = GetMatchRegex(result, PageRegex).ToList();
            foreach (var pageNo in pageSet)
            {
                result = PostData($"expansion_id={expansionId}&page={pageNo}");
                serialSet.UnionWith(PrasePage(result));
            }
            return serialSet;
        }

        private static IEnumerable<string> PrasePage(string html)
        {
            ISet<string> serialSet = new HashSet<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            foreach (var node in htmlDoc.DocumentNode.SelectNodes("//tr/td[1]"))
            {
                serialSet.Add(node.InnerText);
            }
            return serialSet;
        }

        private static IEnumerable<string> GetMatchRegex(string source, string regexPattern)
        {
            var matches = Regex.Matches(source, regexPattern);
            ISet<string> set = new HashSet<string>();
            foreach (Match match in matches)
            {
                set.Add(match.Groups[1].Value);
            }
            return set;
        }

        private static string PostData(string data)
        {
            var request = WebRequest.Create("http://ws-tcg.com/jsp/cardlist/expansionDetail") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var reqStream = new StreamWriter(request.GetRequestStream()))
            {
                reqStream.Write(data);
            }
            string result;
            using (var response = request.GetResponse())
            {
                var sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
            }
            return result;
        }
    }
}