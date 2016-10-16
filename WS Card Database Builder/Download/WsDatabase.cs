using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using WsCardDatabaseBuilder.Model;

namespace WsCardDatabaseBuilder.Download
{
    internal class WsDatabase : IEqualityComparer<Card>
    {
        private readonly string _databaseName;
        private readonly int _version;

        public WsDatabase(string databaseName, int versoin)
        {
            _databaseName = databaseName;
            _version = versoin;
        }

        public bool Equals(Card x, Card y)
        {
            return x.Serial == y.Serial;
        }

        public int GetHashCode(Card obj)
        {
            return obj.Serial.GetHashCode();
        }

        public void Save(IEnumerable<Card> cardList)
        {
            if (File.Exists(_databaseName))
                File.Delete(_databaseName);
            SQLiteConnection.CreateFile(_databaseName);
            var sqLiteConnection = new SQLiteConnection($"Data source={_databaseName}");
            sqLiteConnection.Open();
            using (var command = sqLiteConnection.CreateCommand())
            {
                command.CommandText =
                    $"CREATE TABLE {Table.Card} ( _id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                    $"{Field.Name} TEXT, {Field.Serial} TEXT UNIQUE, {Field.Rarity} TEXT, {Field.Expansion} TEXT, {Field.Side} TEXT, {Field.Color} TEXT, " +
                    $"{Field.Level} INTEGER, {Field.Power} INTEGER, {Field.Cost} INTEGER, {Field.Soul} INTEGER, {Field.Type} TEXT, {Field.FirstChara} TEXT, " +
                    $"{Field.SecondChara} TEXT, {Field.Text} TEXT, {Field.Flavor} TEXT, {Field.Trigger} TEXT, {Field.Image} TEXT)";
                command.ExecuteNonQuery();
            }
            using (var command = sqLiteConnection.CreateCommand())
            {
                command.CommandText =
                    $"CREATE TABLE {Table.Version} (_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, {Field.Version} INTEGER)";
                command.ExecuteNonQuery();
            }
            using (var transaction = sqLiteConnection.BeginTransaction())
            {
                using (var command = sqLiteConnection.CreateCommand())
                {
                    command.CommandText =
                        $"INSERT INTO {Table.Card} ({Field.Name}, {Field.Serial}, {Field.Rarity}, {Field.Expansion}, {Field.Side}, {Field.Color}," +
                        $" {Field.Level}, {Field.Power}, {Field.Cost}, {Field.Soul}, {Field.Type}, {Field.FirstChara}, {Field.SecondChara}, {Field.Text}, " +
                        $"{Field.Flavor}, {Field.Trigger}, {Field.Image})" +
                        $" VALUES(@{Field.Name}, @{Field.Serial}, @{Field.Rarity}, @{Field.Expansion}, @{Field.Side}, @{Field.Color}, @{Field.Level}," +
                        $" @{Field.Power}, @{Field.Cost}, @{Field.Soul}, @{Field.Type}, @{Field.FirstChara}, @{Field.SecondChara}, @{Field.Text}, " +
                        $"@{Field.Flavor}, @{Field.Trigger}, @{Field.Image})";
                    foreach (var card in cardList.Distinct(this))
                    {
                        command.Parameters.AddWithValue(Field.Name, card.Name);
                        command.Parameters.AddWithValue(Field.Serial, card.Serial);
                        command.Parameters.AddWithValue(Field.Rarity, card.Rarity);
                        command.Parameters.AddWithValue(Field.Expansion, card.Expansion);
                        command.Parameters.AddWithValue(Field.Side, card.Side.ToString());
                        command.Parameters.AddWithValue(Field.Color, card.Color.ToString());
                        command.Parameters.AddWithValue(Field.Level, card.Level);
                        command.Parameters.AddWithValue(Field.Power, card.Power);
                        command.Parameters.AddWithValue(Field.Cost, card.Cost);
                        command.Parameters.AddWithValue(Field.Soul, card.Soul);
                        command.Parameters.AddWithValue(Field.Type, card.Type.ToString());
                        command.Parameters.AddWithValue(Field.FirstChara, card.FirstChara);
                        command.Parameters.AddWithValue(Field.SecondChara, card.SecondChara);
                        command.Parameters.AddWithValue(Field.Text, card.Text);
                        command.Parameters.AddWithValue(Field.Flavor, card.Flavor);
                        command.Parameters.AddWithValue(Field.Trigger, card.Trigger.ToString());
                        command.Parameters.AddWithValue(Field.Image, card.Image);
                        command.ExecuteNonQuery();
                    }
                }

                using (var command = sqLiteConnection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {Table.Version} ({Field.Version}) VALUES(@{Field.Version})";
                    command.Parameters.AddWithValue(Field.Version, _version);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            sqLiteConnection.Close();
        }

        private abstract class Table
        {
            public const string Card = "card";
            public const string Version = "version";
        }

        private abstract class Field
        {
            public const string Version = "Version";
            public const string Name = "Name";
            public const string Serial = "Serial";
            public const string Rarity = "Rarity";
            public const string Expansion = "Expansion";
            public const string Side = "Side";
            public const string Color = "Color";
            public const string Level = "Level";
            public const string Power = "Power";
            public const string Cost = "Cost";
            public const string Soul = "Soul";
            public const string Type = "Type";
            public const string FirstChara = "FirstChara";
            public const string SecondChara = "SecondChara";
            public const string Text = "Text";
            public const string Flavor = "Flavor";
            public const string Trigger = "Trigger";
            public const string Image = "Image";
        }
    }
}