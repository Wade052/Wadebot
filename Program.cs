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
// current version : 1.4.5
// Latest update : 02/05/2026 - Cleaned up code to make it more readable and maintainable. 


namespace Wadebot
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        static ConcurrentDictionary<ulong, DateTime> XpCooldown =
        new ConcurrentDictionary<ulong, DateTime>();

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


