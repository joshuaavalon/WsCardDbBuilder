using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WsDeckDatabase.Download;
using WsDeckDatabase.Model;

namespace WsDeckDatabase
{
    internal class Program
    {
        private const string CachePath = @"Cache\";

        private static void Main(string[] args)
        {
            var forced = args.Length > 0 && args.Contains("-f");
            var serialDownloader = new SerialDownloader();
            var serials = serialDownloader.Download().ToList();

            var cards = new List<Card>();
            var count = 0;
            var nullList = new List<string>();
            Directory.CreateDirectory(CachePath);
            var cardDownloader = new CardDownloader();
            serials.ForEach(serial =>
            {
                var fileName = serial.Replace("/", "").Replace("-", "");
                var path = $@"{CachePath}\{fileName}.json";
                Card card;
                if (!forced && File.Exists(path))
                {
                    card = JsonConvert.DeserializeObject<Card>(File.ReadAllText(path));
                }
                else
                {
                    card = cardDownloader.Download(serial);
                    if (card != null)
                        File.WriteAllText(path, JsonConvert.SerializeObject(card, Formatting.Indented));
                }

                if (card != null)
                    cards.Add(card);
                else
                    nullList.Add(serial);

                Console.Out.WriteLine($"{serial}:{++count}/{serials.Count()}");
            });
            Console.Out.WriteLine("Finish Download Cards!");
            var wsdb = new WsDatabase();
            wsdb.Save(cards);
            Console.Out.WriteLine("Finish Upload Cards!");
            foreach (var serial in nullList)
            {
                Console.Out.WriteLine(serial);
            }
        }
    }
}