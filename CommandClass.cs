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
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Wadebot.commands
{

    public class CommandTest : BaseCommandModule
    {

        /* 
         * Command : Speak
         * Function : Wadebot will choose a random phrase from the string array words
         * Cooldown : 1 use every 5 seconds per user
        */
        [Command("Speak")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task PlaceHolderCommand(CommandContext ctx)
        {
            string[] words = { "Hello", "Im not alive", "I was created in C#", "Not all roads lead to rome", "absolute lamp", "Hue Hue Hue", "New year same me", ":D", "Happy New Year", "oh word?" };
            Random rand = new Random();
            string chosenWord = words[rand.Next(words.Length)];
            await ctx.Channel.SendMessageAsync(chosenWord);
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;



            //Log System
            using var connection = Database.GetConnection();
            connection.Open();

            var logs = connection.CreateCommand();
            logs.CommandText =
            @"
    INSERT INTO Logs (UserId, UserName, GuildId, GuildName, Command, Date, Output)
    VALUES ($userID, $user, $guild, $server, $command, $date, $output);
    ";

            logs.Parameters.AddWithValue("$userID", ctx.User.Id.ToString());
            logs.Parameters.AddWithValue("$user", ctx.User.Username.ToString());
            logs.Parameters.AddWithValue("$guild", ctx.Guild.Id.ToString());
            logs.Parameters.AddWithValue("server", ctx.Guild.ToString());
            logs.Parameters.AddWithValue("command", "Speak");
            logs.Parameters.AddWithValue("date", timestamp.ToString());
            logs.Parameters.AddWithValue("output", "Wade Bot said: "+ chosenWord);

            logs.ExecuteNonQuery();
        }

        /*
         * Command : 8Ball
         * Function : Wadebot will respond to a user's question with a random answer from the string array responses
         * Cooldown : 1 use every 5 seconds per user
        */
        [Command("8Ball")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task EightBallCommand(CommandContext ctx, [RemainingText] string question)
        {
            string[] responses = { "Yes", "No", "Maybe", "Dont count on it", "Definitely", "Absolutely not", "Ask again later", "Allegedly", "I have no idea", "Perchance" };
            string[] Faces = { ":)", ":(", ":/"};
            string face = "";
            Random rand = new Random();
            string chosenResponse = responses[rand.Next(responses.Length)];
            if (chosenResponse == responses[0] || chosenResponse == responses[4]) 
            {
                face = Faces[0];
            }
            else if (chosenResponse == responses[1] || chosenResponse == responses[3]||chosenResponse == responses[5])
            {
                face = Faces[1];
            }
            else
            {
                face = Faces[2];
            }
            await ctx.Channel.SendMessageAsync($"Question: {question}\nAnswer: {chosenResponse}\n{face}");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;

            //Log System
            using var connection = Database.GetConnection();
            connection.Open();

            var logs = connection.CreateCommand();
            logs.CommandText =
            @"
    INSERT INTO Logs (UserId, UserName, GuildId, GuildName, Command, Date, Output)
    VALUES ($userID, $user, $guild, $server, $command, $date, $output);
    ";

            logs.Parameters.AddWithValue("$userID", ctx.User.Id.ToString());
            logs.Parameters.AddWithValue("$user", ctx.User.Username.ToString());
            logs.Parameters.AddWithValue("$guild", ctx.Guild.Id.ToString());
            logs.Parameters.AddWithValue("server", ctx.Guild.ToString());
            logs.Parameters.AddWithValue("command", "8ball");
            logs.Parameters.AddWithValue("date", timestamp.ToString());
            logs.Parameters.AddWithValue("output", user+" asked" + question + "Wade Bot said: " + chosenResponse);

            logs.ExecuteNonQuery();
        }
        // The Following commands deal with basic arithmetic operations

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
            var Log = $"{user.Username}#{user.Discriminator} Calculated {a} + {b} = {sum} at {timestamp}";
            Console.WriteLine(Log);
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
            var Log = $"{user.Username}#{user.Discriminator} Calculated {a} - {b} = {difference} at {timestamp}";
            Console.WriteLine(Log);
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
            var Log = $"{user.Username}#{user.Discriminator} Calculated {a} * {b} = {product} at {timestamp}";
            Console.WriteLine(Log);
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
            var Log = $"{user.Username}#{user.Discriminator} Calculated {a} / {b} = {quotient} at {timestamp}";
            Console.WriteLine(Log);
        }
        // end

        /*
         * Command : Roulette
         * Function : Wadebot will randomly select a non-bot member from the server and mention them in the channel
         * Cooldown : 1 use every 5 seconds per user
        */
        [Command("Roulette")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task RouletteCommand(CommandContext ctx)
        {
            var members = await ctx.Guild.GetAllMembersAsync();

            // Filter out bots so you only pick human members
            var humans = members.Where(m => !m.IsBot).ToList();

            if (humans.Count == 0)
            {
                await ctx.Channel.SendMessageAsync("No non-bot members found!");
                return;
            }

            // Pick a random member
            Random rand = new Random();
            var chosen = humans[rand.Next(humans.Count)];

            // Mention (ping) them
            await ctx.Channel.SendMessageAsync($"Hey {chosen.Mention}, you’ve been chosen!");
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;
            var Log = $"{user.Username}#{user.Discriminator} Pinged {chosen} at {timestamp}";
            Console.WriteLine(Log);
        }

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
            var Log = $"{user.Username}#{user.Discriminator} used Info {timestamp}";
            Console.WriteLine(Log);
        }

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
            var Log = $"{user.Username}#{user.Discriminator} deleted {amt} message(s) at {timestamp}";
            Console.WriteLine(Log);

            // Optional: auto-delete the confirmation message after a short delay
            await Task.Delay(3000);
            await confirmation.DeleteAsync();
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
            Random Rand = new Random();
            int RNum;
            if (Num1 > Num2)
            {
                RNum = Rand.Next(Num2, Num1 + 1);
                await ctx.Channel.SendMessageAsync($"Generated a random number between {Num2} and {Num1}. is {RNum}");

            }
            else
            {
                RNum = Rand.Next(Num1, Num2 + 1);
                await ctx.Channel.SendMessageAsync($"Generated a random number between {Num1} and {Num2}. is {RNum}");

            }
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            var user = ctx.User;
            var logMessage = $"{user.Username}#{user.Discriminator} generated a random number between {Num1} and {Num2}: {RNum}";
            Console.WriteLine(logMessage + " at " + timestamp);
        }

        /*
         * Command : Whisper
         * Function : Wadebot will DM the user asking for a secret and randomly decide to keep it or snitch
         * Cooldown : 1 use every 5 seconds per user
         * ChatGPT helped me with this one :)
        */
        [Command("Whisper")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task Whisper(CommandContext ctx)
        {
            var user = ctx.Member;
            var timestamp = DateTime.Now.ToString("HH:mm:ss");

            if (user.IsBot)
            {
                await ctx.Channel.SendMessageAsync("❌ Bots cannot use this command.");
                return;
            }

            // Step 1: Acknowledge in server
            await ctx.Channel.SendMessageAsync("📩 Check your DMs…");

            // Step 2: Create DM
            DiscordDmChannel dm;
            try
            {
                dm = await user.CreateDmChannelAsync();
            }
            catch
            {
                await ctx.Channel.SendMessageAsync("❌ I can't DM you. Please enable DMs.");
                return;
            }

            // Step 3: Start DM conversation
            await dm.SendMessageAsync("🤫 What is your secret?");

            // Step 4: Wait for DM reply
            var interactivity = ctx.Client.GetInteractivity();
            var reply = await interactivity.WaitForMessageAsync(
                m => m.Author.Id == user.Id && m.Channel.Id == dm.Id,
                TimeSpan.FromSeconds(30)
            );

            if (reply.TimedOut)
            {
                await dm.SendMessageAsync("⌛ You took too long to reply.");
                return;
            }

            string secret = reply.Result.Content;

            // Step 5: Random response
            Random rand = new Random();
            bool snitch = rand.Next(0, 2) == 1;

            string response;
            string log;

            if (!snitch)
            {
                response = "🤐 Your secret is safe with me.";
                await ctx.Channel.SendMessageAsync("I aint no snitch");
                log = $"{user.Username}#{user.Discriminator} whispered: {secret} (kept) at {timestamp}";
            }
            else
            {
                response = $"📣 Yeah nah I'm snitching — {secret}";
                await ctx.Channel.SendMessageAsync($"Wow I cant believe {user.Username} said '{secret}'");
                log = $"{user.Username}#{user.Discriminator} whispered: {secret} (snitched) at {timestamp}";
            }

            // Step 6: Send DM response
            await dm.SendMessageAsync(response);
            Console.WriteLine(log);
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

            var TimeStamp = DateTime.Now.ToString("HH:mm");
            var log = $"{user.Username}#{user.Discriminator} was muted by {Muter.Username}#{Muter.Discriminator} for {dur} minutes because {reason} at {TimeStamp}";
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

            var Timestamp = DateTime.Now.ToString("HH:mm");
            var log = $"{user.Username}#{user.Discriminator} was unmuted by {unMuter.Username}#{unMuter.Discriminator} at {Timestamp}";
            Console.WriteLine(log);
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

            var Timestamp = DateTime.Now.ToString("HH:mm");
            var log = $"{user.Username}#{user.Discriminator} was unmuted by {Banhammer.Username}#{Banhammer.Discriminator} at {Timestamp}";
            Console.WriteLine(log);
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
            var date = DateTime.Now.ToString("MMMM/d/yyyy");
            await ctx.Channel.SendMessageAsync("The date is " + date);
        }
        //Start of Birthday Commands
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
            logs.Parameters.AddWithValue("output","Birthday Saved: "+month+"/"+day+"/"+year);

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
        }



    }
}
