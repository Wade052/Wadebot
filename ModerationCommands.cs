using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wadebot
{
    public class ModerationCommands : BaseCommandModule 
    {
        /*
         * Command : Purge
         * Function : Wadebot will delete a specified number of messages from the channel
         * Cooldown : 1 use every 5 seconds per user
         * Permission : Administrator required
        */
        [Command("Purge")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task Purge(CommandContext ctx, int amt)
        {
            if (amt < 1)
            {
                await ctx.Channel.SendMessageAsync("Please specify a number greater than 0.");
                return;
            }

            // Discord bulk delete limit is 100 messages at once
            if (amt > 100)
                amt = 100;

            // Get messages
            var messages = await ctx.Channel.GetMessagesAsync(amt + 1); // +1 to include the purge command itself

            // Delete messages
            await ctx.Channel.DeleteMessagesAsync(messages);

            // Confirmation message
            var confirmation = await ctx.Channel.SendMessageAsync($"🧹 Deleted {amt} messages!");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;

            // Optional: auto-delete the confirmation message after a short delay
            await Task.Delay(3000);
            await confirmation.DeleteAsync();

            LogCommand(ctx, "Purge", $"{user} purged {amt} in {ctx.Guild} , {ctx.Channel}");
            await HandleXpAsync(ctx);
        }


        /*
         * Command : Mute
         * Function : Wadebot will mute a specified user for a given duration with an optional reason
         * Permission : Administrator required
        */
        [Command("Mute")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
                                                                                                                /*V Standard Reason V*/
        public async Task Mute(CommandContext ctx, DiscordMember user, int mins, [RemainingText] string reason = "I have no idea gang")
        {
            var Muter = ctx.User;
            var dur = DateTimeOffset.UtcNow.AddMinutes(mins); // dur = duration
            await user.TimeoutAsync(dur, reason);
            await ctx.Channel.SendMessageAsync(user + " was muted for " + mins + " minutes \nbecasue :" + reason);

            LogCommand(ctx, "Mute", $"{user} was muted for {mins} because {reason} in {ctx.Guild}");
            await HandleXpAsync(ctx);

        }
        /*
         * Command : UnMute
         * Function : Wadebot will unmute a specified user with an optional reason
         * Permission : Administrator required
        */
        [Command("UnMute")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]

        public async Task UnMute(CommandContext ctx, DiscordMember user, [RemainingText] string reason = "Be nice now")
        {
            var unMuter = ctx.User;

            await user.TimeoutAsync(null, reason);
            await ctx.Channel.SendMessageAsync(user + " was unmuted\n" + reason);

            LogCommand(ctx, "UnMute", $"{user} Was unmuted by{unMuter} reason: {reason} in {ctx.Guild}");
            await HandleXpAsync(ctx);

        }

        /*
         * Command : Ban
         * Function : Wadebot will ban a specified user for a given number of days with an optional reason
         * Permission : Administrator required
        */
        [Command("Ban")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task ban(CommandContext ctx, DiscordMember user, int days, [RemainingText] string reason = "See ya later stinky")
        {
            var Banhammer = ctx.User;

            await user.BanAsync(days, reason);
            await ctx.Channel.SendMessageAsync(user + " was Banned for " + days + " days \nbecause :" + reason);

            LogCommand(ctx, "Ban", $"{user} was banned for {days} in {ctx.Guild} because :{reason}");
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
