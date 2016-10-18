using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using WsCardDatabaseBuilder.Download;

namespace WsCardDatabaseBuilder
{
    internal class Program
    {
        private const string CachePath = @"Cache\";
        private const string ExpansionPath = @"Expansion\";
        private const string CardPath = @"Card\";

        private static void Main(string[] args)
        {
            var option = new Option();
            if (!Parser.Default.ParseArguments(args, option)) return;
            if (!option.DisableCache && !Directory.Exists(CachePath))
                Directory.CreateDirectory(CachePath);
            var serialDownloader = new SerialDownloader(option,Path.Combine(CachePath, ExpansionPath));
            var serials = serialDownloader.Download().ToList();
            var cardDownloader = new CardDownloader(option, Path.Combine(CachePath, CardPath));
            var wsdb = new WsDatabase(option.OutputPath, option.Version);
            var nullList = new List<string>();
            wsdb.Save(cardDownloader.Download(serials, nullList));
            if (nullList.Count > 0)
            {
                Console.Out.WriteLine("Some serial(s) have prase error:");
                foreach (var serial in nullList)
                {
                    Console.Out.WriteLine(serial);
                }
            }
            Console.Out.WriteLine("Finish building Database.");
        }
    }
}