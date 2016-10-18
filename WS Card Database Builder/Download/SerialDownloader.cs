using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WsCardDatabaseBuilder.Download
{
    internal class SerialDownloader
    {
        private readonly string _url;
        private readonly Option _option;
        private readonly string _cachePath;
        private const string ExpansionRegex = @"showExpansionDetail\('(\d+)',''\)";
        private const string PageRegex = @"page\('(\d+)','\d+'\)";


        public SerialDownloader(Option option, string cachePath, string url = @"http://ws-tcg.com/cardlist/")
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            _url = url;
            _cachePath = cachePath;
            _option = option;
            if (!option.DisableCache && !Directory.Exists(_cachePath))
                Directory.CreateDirectory(_cachePath);
        }

        public IEnumerable<string> Download()
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(_url);
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Any()) return null;
            var html = htmlDoc.DocumentNode.InnerHtml;
            var expansionSet = GetMatchRegex(html, ExpansionRegex).ToList();
            var serialSet = new HashSet<string>();
            var count = 0;
            Console.Out.Write("Download serials...");
            using (var progressBar = new ProgressBar())
            {
                foreach (var no in expansionSet)
                {
                    var path = $@"{_cachePath}\{no}.txt";
                    IList<string> serials;
                    if (_option.SerialCache && File.Exists(path))
                    {
                        serials = ReadCache(path).ToList();
                    }
                    else
                    {
                        serials = DowloadExpansion(no).ToList();
                        if(!_option.DisableCache)
                            File.WriteAllLines(path, serials);
                    }
                    serialSet.UnionWith(serials);
                    progressBar.Report((double)++count / expansionSet.Count);
                }
            }
            Console.WriteLine("Done.");
            serialSet.RemoveWhere(string.IsNullOrWhiteSpace);
            return serialSet;
        }

        private static IEnumerable<string> ReadCache(string path)
        {
            return File.ReadAllText(path).Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static IEnumerable<string> DowloadExpansion(string expansionId)
        {
            var result = PostData($"expansion_id={expansionId}");
            var serialSet = new HashSet<string>();
            serialSet.UnionWith(PrasePage(result));
            var pageSet = GetMatchRegex(result, PageRegex).ToList();
            foreach (var pageNo in pageSet)
            {
                result = PostData($"expansion_id={expansionId}&page={pageNo}");
                if (!string.IsNullOrWhiteSpace(result))
                    serialSet.UnionWith(PrasePage(result));
            }
            return serialSet;
        }

        private static IEnumerable<string> PrasePage(string html)
        {
            var serialSet = new HashSet<string>();
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
            var set = new HashSet<string>();
            foreach (Match match in matches)
            {
                set.Add(match.Groups[1].Value);
            }
            return set;
        }

        private static string PostData(string data)
        {
            var request = WebRequest.Create("http://ws-tcg.com/jsp/cardlist/expansionDetail") as HttpWebRequest;
            if (request == null)
                throw new ApplicationException();
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var reqStream = new StreamWriter(request.GetRequestStream()))
            {
                reqStream.Write(data);
            }
            using (var response = request.GetResponse())
            {
                var stream = response.GetResponseStream();
                if (stream == null) return null;
                var sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }
        }
    }
}