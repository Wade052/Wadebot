using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wadebot
{
    public static class MessageHandler
    {
        private static readonly string PhraseHold =
            Path.Combine(AppContext.BaseDirectory, "Phrases.txt");

        private static readonly string BannedWords =
            Path.Combine(AppContext.BaseDirectory, "banned.txt");

        private static readonly Random rand = new Random();

        private static readonly ConcurrentDictionary<ulong, DateTime> XpCooldown =
            new ConcurrentDictionary<ulong, DateTime>();

        public static async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot || e.Guild == null || e.Message.Content.StartsWith("!"))
                return;

            if (e.Message.MessageType != MessageType.Default)
                return;

            string message = e.Message.Content.Trim();

            if (string.IsNullOrWhiteSpace(message))
                return;

            if (e.Author.Username.Equals("v0rtexking98", StringComparison.OrdinalIgnoreCase))
                await e.Channel.SendMessageAsync("I know who you are");

            if (message.Contains("bark", StringComparison.OrdinalIgnoreCase))
                await e.Channel.SendMessageAsync("Arf Arf");

            SavePhraseIfAllowed(e, message);

            await HandlePassiveXpAsync(e);

            await MaybeRepeatPhraseAsync(e);
        }

        private static void SavePhraseIfAllowed(MessageCreateEventArgs e, string message)
        {
            int x = message.GetNormalizedLength();

            string[] banned = File.Exists(BannedWords)
                ? File.ReadAllLines(BannedWords)
                : Array.Empty<string>();

            bool isAllowed =
                !e.Author.Username.Equals("s1lversg", StringComparison.OrdinalIgnoreCase) &&
                !message.StartsWith("!") &&
                !message.Contains("@everyone") &&
                !message.Contains("@here") &&
                x <= 100 &&
                !banned.Any(word => message.Contains(word, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
                return;

            File.AppendAllText(PhraseHold, message + Environment.NewLine);
        }

        private static async Task HandlePassiveXpAsync(MessageCreateEventArgs e)
        {
            var now = DateTime.UtcNow;

            if (XpCooldown.TryGetValue(e.Author.Id, out DateTime lastXp))
            {
                if ((now - lastXp).TotalSeconds < 30)
                    return;
            }

            XpCooldown[e.Author.Id] = now;

            int newLevel = 0;

            bool leveledUp = await Task.Run(() =>
                Database.AddXp(e.Author.Id, e.Guild.Id, 5, out newLevel)
            );

            if (leveledUp)
            {
                await e.Channel.SendMessageAsync(
                    $"Congrats {e.Author.Mention}, you leveled up to level {newLevel}!"
                );
            }
        }

        private static async Task MaybeRepeatPhraseAsync(MessageCreateEventArgs e)
        {
            if (rand.Next(1, 3) == 1)
                return;

            if (!File.Exists(PhraseHold))
                return;

            string[] phrases = File.ReadAllLines(PhraseHold)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToArray();

            if (phrases.Length == 0)
                return;

            string phrase = phrases[rand.Next(phrases.Length)];

            await e.Channel.SendMessageAsync(phrase);
        }
    }
}