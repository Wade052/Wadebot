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
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
