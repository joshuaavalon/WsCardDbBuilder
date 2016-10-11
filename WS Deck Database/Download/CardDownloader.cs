using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WsDeckDatabase.Model;

namespace WsDeckDatabase.Download
{
    internal class CardDownloader
    {
        private readonly string _query;
        private readonly HtmlWeb _web;

        /// <summary>
        ///     Unwanted string to be removed.
        /// </summary>
        public IEnumerable<string> UnwantedString = new[] {"-", "－", "（バニラ）"};

        public CardDownloader(string query = @"http://ws-tcg.com/cardlist/?cardno=")
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            _query = query;
            _web = new HtmlWeb();
        }

        public Card Download(string serial)
        {
            var card = new Card();
            if (serial == null)
                throw new ArgumentNullException(nameof(serial));
            var url = _query + serial;
            var htmlDoc = _web.Load(url);
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Any()) return null;
            var documentNode = htmlDoc.DocumentNode;
            var tableNode = documentNode?.SelectSingleNode("//table[@class='status']");
            if (tableNode == null) return null;
            card.Name = tableNode.SelectSingleNode("//th[@class='cell_1']/following-sibling::td/text()[1]")?
                .InnerText.Trim();
            card.Serial = tableNode.SelectSingleNode("//td[@class='cell_2']")?.InnerText?.Trim();
            card.Rarity = tableNode.SelectSingleNode("//td[@class='cell_4']")?.InnerText?.Trim();
            card.Expansion = tableNode.SelectSingleNode("//th[text()='エクスパンション']/following-sibling::td")?
                .InnerText.Trim();
            if(card.Expansion == "シリーズ セカンドシーズン")
            {
                card.Expansion = "<物語>" + card.Expansion;
            }
            var path = tableNode.SelectSingleNode("//th[text()='サイド']/following-sibling::td/img")?
                .GetAttributeValue("src", "").Trim();
            card.Side = Path.GetFileNameWithoutExtension(path).EnumParse<Side>();
            var type = tableNode.SelectSingleNode("//th[text()='種類']/following-sibling::td")?.InnerText.Trim();
            card.Type = type.ParseType();
            path = tableNode.SelectSingleNode("//th[text()='色']/following-sibling::td/img")?
                .GetAttributeValue("src", "").Trim();
            card.Color = Path.GetFileNameWithoutExtension(path).EnumParse<Color>();
            card.Level = (tableNode.SelectSingleNode("//th[text()='レベル']/following-sibling::td")?
                .InnerText.Trim()).IntParseOrDefault();
            card.Cost = (tableNode.SelectSingleNode("//th[text()='コスト']/following-sibling::td")?
                .InnerText.Trim()).IntParseOrDefault();
            card.Power =
                (tableNode.SelectSingleNode("//th[text()='パワー']/following-sibling::td")?
                    .InnerText.Trim()).IntParseOrDefault();
            var soul = tableNode.SelectNodes("//th[text()='ソウル']/following-sibling::td/img");
            card.Soul = soul?.Count(x => Path.GetFileNameWithoutExtension(x.Attributes["src"].Value ?? "")
                .Contains("soul")) ?? 0;
            var nodes = tableNode.SelectNodes("//th[text()='トリガー']/following-sibling::td/img");
            card.Trigger = NodesToTrigger(nodes);
            var charaList =
                SplitCharaString(tableNode.SelectSingleNode("//th[text()='特徴']/following-sibling::td").InnerText);
            if (charaList == null || charaList.Count != 2)
            {
                card.FirstChara = "";
                card.SecondChara = "";
            }
            else
            {
                card.FirstChara = charaList[0];
                card.SecondChara = charaList[1];
            }

            var rgx = new Regex(@"<font[^>]+>");
            card.Text =
                rgx.Replace(
                    tableNode.SelectSingleNode("//th[text()='テキスト']/following-sibling::td")?
                        .InnerHtml.ReplaceBr()
                        .Trim() ?? "", "");
            card.Text = card.Text.Replace("</font>", "");
            if(card.Text.Contains("<img"))
            {
            card.Text = ReplaceImg(card.Text);
            }
            
            rgx = new Regex(@"<img[^>]+>");
            card.Flavor =
                rgx.Replace(
                    tableNode.SelectSingleNode("//th[text()='フレーバー']/following-sibling::td")?
                        .InnerHtml.ReplaceBr()
                        .Trim() ?? "", "");
            var image =
                tableNode.SelectSingleNode("//td[@class='graphic']/img")
                    .GetAttributeValue("src", "")
                    .Trim();
            if (!image.Contains(@"http://ws-tcg.com"))
                image = @"http://ws-tcg.com" + image;
            card.Image = image;
            return card;
        }

        private static Trigger NodesToTrigger(HtmlNodeCollection nodes)
        {
            if (nodes == null) return Trigger.None;
            switch (nodes.Count)
            {
                case 1:
                    var src1 = nodes[0].Attributes["src"].Value;
                    if (src1 == null) return Trigger.None;
                    if (Path.GetFileNameWithoutExtension(src1).Contains("soul"))
                        return Trigger.OneSoul;
                    if (Path.GetFileNameWithoutExtension(src1).Contains("stock"))
                        return Trigger.Bag;
                    if (Path.GetFileNameWithoutExtension(src1).Contains("salvage"))
                        return Trigger.Door;
                    if (Path.GetFileNameWithoutExtension(src1).Contains("draw"))
                        return Trigger.Book;
                    return Path.GetFileNameWithoutExtension(src1).Contains("treasure") ? Trigger.Gold : Trigger.None;
                case 2:
                    var src2 = nodes[1].Attributes["src"].Value;
                    if (src2 == null) return Trigger.None;
                    if (Path.GetFileNameWithoutExtension(src2).Contains("soul"))
                        return Trigger.TwoSoul;
                    if (Path.GetFileNameWithoutExtension(src2).Contains("bounce"))
                        return Trigger.Wind;
                    if (Path.GetFileNameWithoutExtension(src2).Contains("shot"))
                        return Trigger.Fire;
                    return Path.GetFileNameWithoutExtension(src2).Contains("gate") ? Trigger.Gate : Trigger.None;
                default:
                    return Trigger.None;
            }
        }

        private IList<string> SplitCharaString(string charas)
        {
            if (charas == null) return null;
            var split = charas.Split('・');
            for (var i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
                if (UnwantedString.Any(str => split[i].Equals(str)))
                    split[i] = "";
            }
            return split;
        }

        private string ReplaceImg(string text)
        {
            text = text.Replace(@"<img src='../cardlist/partimages/bounce.gif'>", "リターン");
            text = text.Replace(@"<img src='../cardlist/partimages/stock.gif'>", "プール");
            text = text.Replace(@"<img src='../cardlist/partimages/salvage.gif'>", "カムバック");
            text = text.Replace(@"<img src='../cardlist/partimages/draw.gif'>", "ドロー");
            text = text.Replace(@"<img src='../cardlist/partimages/treasure.gif'>", "トレジャー");
            text = text.Replace(@"<img src='../cardlist/partimages/shot.gif'>", "ショット");
            text = text.Replace(@"<img src='../cardlist/partimages/gate.gif'>", "ゲート");
        
            return text;
        }
    }
}