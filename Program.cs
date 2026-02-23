using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wadebot.commands;
using System.Collections.Concurrent;


// Project : Wadebot
// Programmer : DeAndre Wade
// Description : A functional Discord bot built with DSharpPlus and OpenAI integration.
// Date : 10/16/2025 - present
// https://www.youtube.com/watch?v=qxlmioSDWmk - Used this video to help set up the bot structure with DSharpPlus
// current version : 1.5.5
// Latest update : 02/19/2026 - Created Passive XP system with cooldown to prevent spam. Each user can only gain XP once every 30 seconds. Also added console logs for when users level up and when the bot comes online.


namespace Wadebot
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        //Pasive XP system with a cooldown to prevent spam. Each user can only gain XP once every 30 seconds.
        static ConcurrentDictionary<ulong, DateTime> XpCooldown =
        new ConcurrentDictionary<ulong, DateTime>();

        private static async Task OnMessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (e.Author.IsBot || e.Guild == null || e.Message.Content.StartsWith(">"))
                return;

            if (e.Message.MessageType != MessageType.Default)
                return;

            var now = DateTime.UtcNow;
            if (XpCooldown.TryGetValue(e.Author.Id, out DateTime lastXp))
            {
                if ((now - lastXp).TotalSeconds < 30)
                    return;
            }

            XpCooldown[e.Author.Id] = now;

            // 1. Declare the variable outside the Task
            int newLevel = 0;

            // 2. Run the XP logic
            bool leveledUp = await Task.Run(() => Database.AddXp(e.Author.Id, e.Guild.Id, 5, out newLevel));

            if (leveledUp)
            {
                // 3. Now 'newLevel' is accessible here!
                await e.Channel.SendMessageAsync($"Congrats {e.Author.Mention}, you leveled up to level {newLevel}!");
            }
        }
        private static async Task AddXpAsync(ulong userId, ulong guildId, int xpAmount)
        {
            // Ensure you open the connection asynchronously
            using var connection = Database.GetConnection();
            await connection.OpenAsync(); // <--- CRITICAL: Connections must be opened

            // Check if user exists
            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = @"
        SELECT Experience, Level 
        FROM Levels 
        WHERE UserId = @uid AND GuildId = @gid";
            checkCmd.Parameters.AddWithValue("@uid", userId.ToString());
            checkCmd.Parameters.AddWithValue("@gid", guildId.ToString());

            int currentXp = 0;
            int currentLevel = 1;

            // Use ExecuteReaderAsync and await it
            using (var reader = await checkCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    currentXp = reader.GetInt32(0);
                    currentLevel = reader.GetInt32(1);
                }
            } // Reader closes automatically here

            currentXp += xpAmount;
            int requiredXp = currentLevel * 100;

            if (currentXp >= requiredXp)
            {
                currentXp = 0;
                currentLevel++;
                Console.WriteLine($"User {userId} leveled up to {currentLevel}");
            }

            var upsertCmd = connection.CreateCommand();
            upsertCmd.CommandText = @"
        INSERT INTO Levels (UserId, GuildId, Experience, Level)
        VALUES (@uid, @gid, @xp, @lvl)
        ON CONFLICT(UserId, GuildId)
        DO UPDATE SET Experience = @xp, Level = @lvl";

            upsertCmd.Parameters.AddWithValue("@uid", userId.ToString());
            upsertCmd.Parameters.AddWithValue("@gid", guildId.ToString());
            upsertCmd.Parameters.AddWithValue("@xp", currentXp);
            upsertCmd.Parameters.AddWithValue("@lvl", currentLevel);

            // Use ExecuteNonQueryAsync
            await upsertCmd.ExecuteNonQueryAsync();
        }



        static async Task Main(string[] args)
        {
            Database.Initialize();
            string discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            string prefix = Environment.GetEnvironmentVariable("BOT_PREFIX") ?? ">";

            if (string.IsNullOrWhiteSpace(discordToken))
            {
                Console.WriteLine("❌ DISCORD_TOKEN is not set!");
                return;
            }

            var discordConfig = new DiscordConfiguration
            {
                Intents = DiscordIntents.All,
                Token = discordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);
            Client.MessageCreated += OnMessageCreated;


            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(60)
            });

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { prefix },
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = false
            };

            #region Level Commands
            // Level
            #endregion
            #region fun Commands
            // Speak
            // 8ball
            // RPS
            // Whisper
            // PokemonGuesser
            // Roulette
            #endregion
            #region Math Commands
            // Add
            // Subtract
            // Multiply
            // Divide
            #endregion
            #region Moderation Commands
            // Purge
            // Mute
            // UnMute
            // Ban
            #endregion
            #region Utility Commands
            // Info
            // Random
            // Date
            // Ask
            #endregion
            #region Birthday Commands
            // SetBirthday
            // Birthday
            // BirthdayList
            #endregion

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<LevelCommands>();
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<MathCommands>();
            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<UtilityCommands>();
            Commands.RegisterCommands<BirthdayCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
           //Start of bot online confirmation 
            Console.WriteLine("Bot is online!");
            Console.WriteLine(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
           //end
            return Task.CompletedTask;

        }
    }
}


