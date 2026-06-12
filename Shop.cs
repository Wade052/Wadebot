using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wadebot
{
    public class Shop : BaseCommandModule
    {
        (string Name, int Price)[] shop =
        {
            ("Pet Rock", 15),
            ("Stick", 5),
            ("Purple", 50),
            ("A Picture of a tree", 30),
            ("Soda", 5)
        };

        [Command("Balance")]
        public async Task Balance(CommandContext ctx)
        {
            int balance = Database.GetOrCreateBalance(ctx.User.Id, ctx.Guild.Id);

            await ctx.Channel.SendMessageAsync(
                $"💰 {ctx.User.Username}, you have **{balance} coins**."
            );
        }

        [Command("Shop")]
        public async Task Store(CommandContext ctx)
        { 
           await ctx.Channel.SendMessageAsync("Shop isnt open yet srry :3");
        }
    }
}
