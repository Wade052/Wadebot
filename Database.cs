using Microsoft.Data.Sqlite;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

//Table was created by me to store birthdays and logs for users in different guilds
//How to set up the database to c# was by OpenAI ChatGPT5
namespace Wadebot
{

    /* Breakdown of the Birthdays table structure:
     *          CREATE TABLE IF NOT EXISTS Birthdays (
                    UserId  TEXT NOT NULL, - User's Discord ID
                    GuildId TEXT NOT NULL, - Discord Guild (Server) ID
                    Month   INTEGER NOT NULL, - Birth month
                    Day     INTEGER NOT NULL, - Birth day
                    Year    INTEGER, - Birth year (optional)
                    PRIMARY KEY (UserId, GuildId) - Composite primary key to ensure it's unique per user per guild to avoid crossing information
     */
    public static class Database
    {
        private static readonly string DbPath =
            Path.Combine(AppContext.BaseDirectory, "wadebot.db");

        private static readonly string ConnectionString =
            $"Data Source={DbPath}";

        public static void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var birthdayCmd = connection.CreateCommand();
            birthdayCmd.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Birthdays (
                UserId  TEXT NOT NULL,
                GuildId TEXT NOT NULL,
                Month   INTEGER NOT NULL,
                Day     INTEGER NOT NULL,
                Year    INTEGER,
                PRIMARY KEY (UserId, GuildId)
            );";
            birthdayCmd.ExecuteNonQuery();

            var logsCmd = connection.CreateCommand();
            logsCmd.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Logs (
                UserId  TEXT NOT NULL,
                UserName TEXT,
                GuildId TEXT NOT NULL,
                GuildName TEXT,
                Command   TEXT NOT NULL,
                Date     TEXT NOT NULL,
                Output    TEXT NOT NULL
            );";
            logsCmd.ExecuteNonQuery();

            var LevelsCmd = connection.CreateCommand();
            LevelsCmd.CommandText =
                @"
            CREATE TABLE IF NOT EXISTS Levels (
                UserId  TEXT NOT NULL,
                GuildId TEXT NOT NULL,
                Experience INTEGER NOT NULL,
                Level INTEGER NOT NULL,
                PRIMARY KEY (UserId, GuildId)   
);";
            LevelsCmd.ExecuteNonQuery();
        }
        public static (int xp, int level) GetOrCreateLevel(ulong userId, ulong guildId)
        {
            using var connection = GetConnection();
            connection.Open();

            var select = connection.CreateCommand();
            select.CommandText =
                @"SELECT Experience, Level
          FROM Levels
          WHERE UserId = @user AND GuildId = @guild";

            select.Parameters.AddWithValue("@user", userId.ToString());
            select.Parameters.AddWithValue("@guild", guildId.ToString());

            using var reader = select.ExecuteReader();

            if (reader.Read())
                return (reader.GetInt32(0), reader.GetInt32(1));

            var insert = connection.CreateCommand();
            insert.CommandText =
                @"INSERT INTO Levels (UserId, GuildId, Experience, Level)
          VALUES (@user, @guild, 0, 1)";

            insert.Parameters.AddWithValue("@user", userId.ToString());
            insert.Parameters.AddWithValue("@guild", guildId.ToString());
            insert.ExecuteNonQuery();

            return (0, 1);
        }

        public static bool AddXp(
            ulong userId,
            ulong guildId,
            int amount,
            out int newLevel)
        {
            using var connection = GetConnection();
            connection.Open();

            var (xp, level) = GetOrCreateLevel(userId, guildId);

            xp += amount;
            int neededXp = level * 100;
            bool leveledUp = false;

            if (xp >= neededXp)
            {
                xp -= neededXp;
                level++;
                leveledUp = true;
            }

            var update = connection.CreateCommand();
            update.CommandText =
                @"UPDATE Levels
          SET Experience = @xp, Level = @level
          WHERE UserId = @user AND GuildId = @guild";

            update.Parameters.AddWithValue("@xp", xp);
            update.Parameters.AddWithValue("@level", level);
            update.Parameters.AddWithValue("@user", userId.ToString());
            update.Parameters.AddWithValue("@guild", guildId.ToString());
            update.ExecuteNonQuery();

            newLevel = level;
            return leveledUp;
        }


        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
