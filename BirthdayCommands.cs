using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wadebot
{
    public class BirthdayCommands: BaseCommandModule
    {

        /*
         * Command : SetBirthday
         * Function : Wadebot will store the user's birthday in the database
         * Cooldown : 1 use every 10 seconds per user
        */
        [Command("setbirthday")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task SetBirthday(CommandContext ctx, int month, int day, int year = 0)
        {
            if (ctx.Guild == null)
            {
                await ctx.RespondAsync("❌ This command must be used in a server.");
                return;
            }

            if (!DateTime.TryParse($"{year}-{month}-{day}", out _))
            {
                await ctx.RespondAsync("❌ Invalid date.");
                return;
            }
            var logdate = DateTime.Now;

            using var connection = Database.GetConnection();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
    INSERT OR REPLACE INTO Birthdays (UserId, GuildId, Month, Day, Year)
    VALUES ($user, $guild, $month, $day, $year);
    ";

            command.Parameters.AddWithValue("$user", ctx.User.Id.ToString());
            command.Parameters.AddWithValue("$guild", ctx.Guild.Id.ToString());
            command.Parameters.AddWithValue("$month", month);
            command.Parameters.AddWithValue("$day", day);
            command.Parameters.AddWithValue("$year", year == 0 ? DBNull.Value : year);

            command.ExecuteNonQuery();

            await ctx.RespondAsync($"🎉 Birthday saved: **{month}/{day}/{(year == 0 ? "?" : year)}**");

            //Log System
            connection.Open();

            var logs = connection.CreateCommand();
            logs.CommandText =
            @"
    INSERT INTO Logs (UserId, UserName, GuildId, GuildName, Command, Date, Output)
    VALUES ($userID, $user, $guild, $server, $command, $date, $output);
    ";

            logs.Parameters.AddWithValue("$userID", ctx.User.Id.ToString());
            logs.Parameters.AddWithValue("$user", ctx.User.ToString());
            logs.Parameters.AddWithValue("$guild", ctx.Guild.Id.ToString());
            logs.Parameters.AddWithValue("server", ctx.Guild.ToString());
            logs.Parameters.AddWithValue("command", "SetBirthday");
            logs.Parameters.AddWithValue("date", logdate.ToString());
            logs.Parameters.AddWithValue("output", "Birthday Saved: " + month + "/" + day + "/" + year);

            command.ExecuteNonQuery();
        }



        /*
         * Command : Birthday
         * Function : Wadebot will retrieve and display the user's stored birthday from the database
         * Cooldown : 1 use every 10 seconds per user
        */
        [Command("birthday")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task GetBirthday(CommandContext ctx)
        {
            if (ctx.Guild == null)
            {
                await ctx.RespondAsync("❌ Use this in a server.");
                return;
            }

            using var connection = Database.GetConnection();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
    SELECT Month, Day, Year FROM Birthdays
    WHERE UserId = $user AND GuildId = $guild;
    ";

            command.Parameters.AddWithValue("$user", ctx.User.Id.ToString());
            command.Parameters.AddWithValue("$guild", ctx.Guild.Id.ToString());

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                await ctx.RespondAsync("❌ No birthday set.");
                return;
            }

            int month = reader.GetInt32(0);
            int day = reader.GetInt32(1);
            string year = reader.IsDBNull(2) ? "?" : reader.GetInt32(2).ToString();

            await ctx.RespondAsync($"🎂 Your birthday is **{month}/{day}/{year}**");
        }

        /*
         * Command : BirthdayList
         * Function : Wadebot will list all stored birthdays in the server from the database
         * Cooldown : 1 use every 10 seconds per user
        */
        [Command("BirthdayList")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task BirthdayList(CommandContext ctx)
        {
            if (ctx.Guild == null)
            {
                await ctx.RespondAsync("❌ Use this in a server.");
                return;
            }

            using var connection = Database.GetConnection();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
    SELECT UserId, Month, Day, Year
    FROM Birthdays
    WHERE GuildId = $guild
    ORDER BY Month, Day;
    ";

            command.Parameters.AddWithValue("$guild", ctx.Guild.Id.ToString());

            using var reader = command.ExecuteReader();

            var list = new List<string>();

            while (reader.Read())
            {
                string userId = reader.GetString(0);
                int month = reader.GetInt32(1);
                int day = reader.GetInt32(2);
                string year = reader.IsDBNull(3) ? "?" : reader.GetInt32(3).ToString();

                list.Add($"<@{userId}> — {month}/{day}/{year}");
            }

            if (list.Count == 0)
            {
                await ctx.RespondAsync("📄 No birthdays registered.");
                return;
            }

            await ctx.RespondAsync("🎈 **Registered Birthdays:**\n" + string.Join("\n", list));

        }
    }
}
