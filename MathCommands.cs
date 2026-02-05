using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wadebot
{
    public class MathCommands : BaseCommandModule
    {
        /*
         * Command : Add
         * Function : Wadebot will add two integers provided by the user and return the sum
         * Cooldown : 1 use every 5 seconds per user
        */
        [Command("Add")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task AddCommand(CommandContext ctx, int a, int b)
        {
            int sum = a + b;
            await ctx.Channel.SendMessageAsync($"The sum of {a} and {b} is {sum}.");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;

            LogCommand(ctx, "Add", $"{user} asked what {a} + {b} is and the sum is: {sum}");
            await HandleXpAsync(ctx);

        }


        /*
         * Command : Subtract
         * Function : Wadebot will subtract two integers provided by the user and return the difference
         * Cooldown : 1 use every 5 seconds per user
         */
        [Command("Subtract")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task SubtractCommand(CommandContext ctx, int a, int b)
        {
            int difference = a - b;
            await ctx.Channel.SendMessageAsync($"The difference between {a} and {b} is {difference}.");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;
            LogCommand(ctx, "Subtract", $"{user} asked what {a} - {b} is and the difference is: {difference}");
            await HandleXpAsync(ctx);

        }

        /*
         * Command : Multiply
         * Function : Wadebot will multiply two integers provided by the user and return the product
         * Cooldown : 1 use every 5 seconds per user
         */
        [Command("Multiply")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task MultiplyCommand(CommandContext ctx, int a, int b)
        {
            int product = a * b;
            await ctx.Channel.SendMessageAsync($"The product of {a} and {b} is {product}.");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;

            LogCommand(ctx, "Multiply", $"{user} asked what {a} * {b} is and the product is: {product}");
            await HandleXpAsync(ctx);

        }

        /*
         * Command : Divide
         * Function : Wadebot will divide two integers provided by the user and return the quotient
         * Cooldown : 1 use every 5 seconds per user
         */
        [Command("Divide")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task DivideCommand(CommandContext ctx, int a, int b)
        {
            if (b == 0 || a == 0)
            {
                await ctx.Channel.SendMessageAsync("Error: Division by zero is not allowed.");
                return;
            }
            double quotient = (double)a / b;
            await ctx.Channel.SendMessageAsync($"The quotient of {a} divided by {b} is {quotient}.");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;
            LogCommand(ctx, "Divide", $"{user} asked what {a} / {b} is and the difference is: {quotient}");
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
