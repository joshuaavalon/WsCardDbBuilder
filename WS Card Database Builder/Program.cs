using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using WsCardDatabaseBuilder.Download;
using WsCardDatabaseBuilder.Model;

namespace WsCardDatabaseBuilder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var option = new Option();
            if (!Parser.Default.ParseArguments(args, option)) return;
            var serialDownloader = new SerialDownloader();
            var serials = serialDownloader.Download().ToList();
            var cardDownloader = new CardDownloader(option, option.CachePath);
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