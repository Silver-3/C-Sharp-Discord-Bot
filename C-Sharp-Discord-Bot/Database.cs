using Microsoft.Data.Sqlite;
using System;

namespace DiscordBot
{
    public class Database
    {
        private SqliteConnection? Connection;

        public void Connect(string ConnectionString)
        {
            Connection = new SqliteConnection(ConnectionString);
            Connection.Open();

            string tableCommand = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER,
                Money INTEGER DEFAULT 0
            );";

            using (var createTable = new SqliteCommand(tableCommand, Connection))
            {
                createTable.ExecuteNonQuery();
            }

            string indexCommand = @"CREATE UNIQUE INDEX IF NOT EXISTS idx_users_userid ON Users(UserID);";
            using (var createIndex = new SqliteCommand(indexCommand, Connection))
            {
                createIndex.ExecuteNonQuery();
            }

            Console.WriteLine("Database connected and table ensured.");
        }

        public int GetMoney(long userId)
        {
            string selectCommand = "SELECT Money FROM Users WHERE UserID = @userId";

            using (var select = new SqliteCommand(selectCommand, Connection))
            {
                select.Parameters.AddWithValue("@userId", userId);
                var result = select.ExecuteScalar();

                if (result == null)
                    return 0;

                return Convert.ToInt32(result);
            }
        }

        public void SetMoney(long userId, int amount)
        {
            using (var transaction = Connection?.BeginTransaction())
            {
                string updateCommand = "UPDATE Users SET Money = @money WHERE UserID = @userId";
                using (var update = new SqliteCommand(updateCommand, Connection, transaction))
                {
                    update.Parameters.AddWithValue("@money", amount);
                    update.Parameters.AddWithValue("@userId", userId);
                    int rowsAffected = update.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        string insertCommand = "INSERT INTO Users (UserID, Money) VALUES (@userId, @money)";
                        using (var insert = new SqliteCommand(insertCommand, Connection, transaction))
                        {
                            insert.Parameters.AddWithValue("@userId", userId);
                            insert.Parameters.AddWithValue("@money", amount);
                            insert.ExecuteNonQuery();
                        }
                    }
                }

                transaction?.Commit();
            }
        }

        public void AddMoney(long userId, int amount)
        {
            int currentMoney = GetMoney(userId);
            int newBalance = currentMoney + amount;
            SetMoney(userId, newBalance);
        }

        public void SubtractMoney(long userId, int amount)
        {
            int currentMoney = GetMoney(userId);
            int newBalance = Math.Max(0, currentMoney - amount);
            SetMoney(userId, newBalance);
        }
    }
}
