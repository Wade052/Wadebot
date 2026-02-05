using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wadebot
{
    public class UtilityCommands : BaseCommandModule
    {
        /*
 * Command : Info
 * Function : Wadebot will provide information about itself
 * Cooldown : 1 use every 5 seconds per user
*/
        [Command("Info")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task Info(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("I am WadeBot. \nMy creator is Wade052 \nI was created on 10/15/2025 and booted up for the first time on 10/16/2025 \nIn order to use any of my commands you have to use the prefix [>] \nIf any problems should arise please tell Wade052");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;

            LogCommand(ctx, "Info", $"{user} used the info command");
            await HandleXpAsync(ctx);

        }


        /*
         * Command : Random
         * Function : Wadebot will generate a random number between two integers provided by the user
         * Cooldown : 1 use every 5 seconds per user
        */
        [Command("Random")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task Random(CommandContext ctx, int Num1, int Num2)
        {

            int RNum;
            if (Num1 > Num2)
            {
                RNum = rand.Next(Num2, Num1 + 1);
                await ctx.Channel.SendMessageAsync($"Generated a random number between {Num2} and {Num1}. is {RNum}");

            }
            else
            {
                RNum = rand.Next(Num1, Num2 + 1);
                await ctx.Channel.SendMessageAsync($"Generated a random number between {Num1} and {Num2}. is {RNum}");

            }
            var user = ctx.User;
            LogCommand(ctx, "Random", $"{user} asked for a random number between{Num1} and {Num2} and got {RNum}");
            await HandleXpAsync(ctx);

        }

        /*
         * Command : Date
         * Function : Wadebot will provide the current date
         * Cooldown : 1 use every 5 seconds per user
        */
        [Command("Date")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task Date(CommandContext ctx)
        {
            var user = ctx.User;
            var date = DateTime.Now.ToString("MMMM/d/yyyy");
            await ctx.Channel.SendMessageAsync("The date is " + date);
            LogCommand(ctx, "Random", $"{user} asked for the date and was told 'the Date is {date}' ");
        }

        /*
         * Command : Ask
         * Function : Wadebot will use OpenAI's GPT-5 to answer a user's question
         * Cooldown : 1 use every 10 seconds per user
        */
        [Command("Ask")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task Ask(CommandContext ctx, [RemainingText] string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                await ctx.Channel.SendMessageAsync("❌ You need to type a question!");
                return;
            }

            // Call the OpenAI service
            string response = await OpenAIService.AskGPT(prompt);

            // Send the answer to Discord
            await ctx.Channel.SendMessageAsync(response);

            var user = ctx.User;
            LogCommand(ctx, "Ask", $"{user} asked the bot {prompt} and it responded with {response}");
            await HandleXpAsync(ctx);
        }





        // Global Random Instance
        private static readonly Random rand = new Random();

        //XP Handling Method
        private async Task HandleXpAsync(CommandContext ctx, int xp = 10)
        {
            bool leveledUp = Database.AddXp(
                ctx.User.Id,
                ctx.Guild.Id,
                xp,
                out int newLevel
            );

            if (leveledUp)
            {
                await ctx.Channel.SendMessageAsync(
                    $"🎉 {ctx.User.Mention} reached **Level {newLevel}**!"
                );
            }
        }

        //Logging Method
        private void LogCommand(
    CommandContext ctx,
    string command,
    string output
)
        {
            using var connection = Database.GetConnection();
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
            @"
        INSERT INTO Logs
        (UserId, UserName, GuildId, GuildName, Command, Date, Output)
        VALUES
        ($uid, $user, $gid, $gname, $cmd, $date, $out);
    ";

            cmd.Parameters.AddWithValue("$uid", ctx.User.Id.ToString());
            cmd.Parameters.AddWithValue("$user", ctx.User.Username);
            cmd.Parameters.AddWithValue("$gid", ctx.Guild.Id.ToString());
            cmd.Parameters.AddWithValue("$gname", ctx.Guild.Name);
            cmd.Parameters.AddWithValue("$cmd", command);
            cmd.Parameters.AddWithValue("$date", DateTime.Now.ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("$out", output);

            cmd.ExecuteNonQuery();
        }
    }
}
