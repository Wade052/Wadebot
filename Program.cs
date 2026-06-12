using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;
using Wadebot.commands;

namespace Wadebot
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        static async Task Main(string[] args)
        {
            Database.Initialize();

            string discordToken =
                Environment.GetEnvironmentVariable("Discord_Token");

            string prefix = "!";

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

            // Message Handler
            Client.MessageCreated += MessageHandler.OnMessageCreated;

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

            Commands = Client.UseCommandsNext(commandsConfig);

            #region Command Registration

            Commands.RegisterCommands<LevelCommands>();
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<MathCommands>();
            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<UtilityCommands>();
            Commands.RegisterCommands<BirthdayCommands>();
            Commands.RegisterCommands<Shop>();

            #endregion

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private static Task OnClientReady(
            DiscordClient sender,
            DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            Console.WriteLine("Bot is online!");
            Console.WriteLine($"Prefix: !");
            Console.WriteLine($"Started: {DateTime.Now}");

            return Task.CompletedTask;
        }
    }
}