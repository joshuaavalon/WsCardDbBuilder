using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using WsDeckDatabase.Model;

namespace WsDeckDatabase.Download
{
    internal class WsDatabase : IEqualityComparer<Card>
    {
        private const string DatabaseName = "wsdb.db";

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
            if (File.Exists(DatabaseName))
                File.Delete(DatabaseName);
            SQLiteConnection.CreateFile(DatabaseName);
            var sqLiteConnection = new SQLiteConnection("Data source=WsDb.db");
            sqLiteConnection.Open();
            using (var command = sqLiteConnection.CreateCommand())
            {
                command.CommandText =
                    "CREATE TABLE card ( _id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Name TEXT, Serial TEXT UNIQUE, Rarity TEXT, Expansion TEXT, Side TEXT, Color TEXT, Level INTEGER, Power INTEGER, Cost INTEGER, Soul INTEGER, Type TEXT, FirstChara TEXT, SecondChara TEXT, Text TEXT, Flavor TEXT, Trigger TEXT, Image TEXT)";
                command.ExecuteNonQuery();
            }
            using (var command = sqLiteConnection.CreateCommand())
            {
                command.CommandText =
                    "CREATE TABLE version (_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Version INTEGER)";
                command.ExecuteNonQuery();
            }
            using (var transaction = sqLiteConnection.BeginTransaction())
            {
                using (var command = sqLiteConnection.CreateCommand())
                {
                    command.CommandText =
                        "INSERT INTO card (Name, Serial, Rarity, Expansion, Side, Color, Level, Power, Cost, Soul, Type, FirstChara, SecondChara, Text, Flavor, Trigger, Image)" +
                        " VALUES(@Name, @Serial, @Rarity, @Expansion, @Side, @Color, @Level, @Power, @Cost, @Soul, @Type, @FirstChara, @SecondChara, @Text, @Flavor, @Trigger, @Image)";
                    foreach (var card in cardList.Distinct(this))
                    {
                        command.Parameters.AddWithValue("Name", card.Name);
                        command.Parameters.AddWithValue("Serial", card.Serial);
                        command.Parameters.AddWithValue("Rarity", card.Rarity);
                        command.Parameters.AddWithValue("Expansion", card.Expansion);
                        command.Parameters.AddWithValue("Side", card.Side.ToString());
                        command.Parameters.AddWithValue("Color", card.Color.ToString());
                        command.Parameters.AddWithValue("Level", card.Level);
                        command.Parameters.AddWithValue("Power", card.Power);
                        command.Parameters.AddWithValue("Cost", card.Cost);
                        command.Parameters.AddWithValue("Soul", card.Soul);
                        command.Parameters.AddWithValue("Type", card.Type.ToString());
                        command.Parameters.AddWithValue("FirstChara", card.FirstChara);
                        command.Parameters.AddWithValue("SecondChara", card.SecondChara);
                        command.Parameters.AddWithValue("Text", card.Text);
                        command.Parameters.AddWithValue("Flavor", card.Flavor);
                        command.Parameters.AddWithValue("Trigger", card.Trigger.ToString());
                        command.Parameters.AddWithValue("Image", card.Image);
                        command.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            sqLiteConnection.Close();
        }
    }
}