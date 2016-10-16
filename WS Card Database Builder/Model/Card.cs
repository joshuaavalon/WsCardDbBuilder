using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WsCardDatabaseBuilder.Model
{
    internal class Card
    {
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Rarity { get; set; }
        public string Expansion { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Side Side { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Color Color { get; set; }

        public int Level { get; set; }
        public int Cost { get; set; }
        public int Power { get; set; }
        public int Soul { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Type Type { get; set; }

        public string FirstChara { get; set; }
        public string SecondChara { get; set; }
        public string Text { get; set; }
        public string Flavor { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Trigger Trigger { get; set; }

        public string Image { get; set; }
    }
}