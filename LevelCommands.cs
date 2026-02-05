using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Wadebot.commands
{
    public class LevelCommands : BaseCommandModule
    {
        /*
         * Command : Level
         * Function : Wadebot will display the user's current level and XP
         * cooldown : 1 use every 5 seconds per user
        */
        [Command("level")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task Level(CommandContext ctx)
        {
            var (xp, level) =
                Database.GetOrCreateLevel(ctx.User.Id, ctx.Guild.Id);

            int needed = level * 100;

            await ctx.Channel.SendMessageAsync(
                $"📊 **{ctx.User.Username}**\n" +
                $"Level: **{level}**\n" +
                $"XP: **{xp}/{needed}**"
            );
        }
    }
}
