using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wadebot;

public class FunCommands : BaseCommandModule
{
    private static readonly Random rand = new Random();

    #region Fun Commands
    [Command("Speak")]
    [Cooldown(1, 5, CooldownBucketType.User)]
    public async Task Speak(CommandContext ctx)
    {
        string[] words =
        {
        "Hello", "Im not alive", "I was created in C#",
        "Not all roads lead to rome", "absolute lamp",
        "Hue Hue Hue", "Hello World", ":D",
        "in the big 26 💔", "oh word?", "Cat"
    };

        string chosenWord = words[rand.Next(words.Length)];

        if (chosenWord == "Cat")
        {
            string filepath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Cloe.png"
            );

            using var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            await new DiscordMessageBuilder()
                .WithContent("Cat")
                .AddFile(fs)
                .SendAsync(ctx.Channel);
        }
        else
        {
            await ctx.Channel.SendMessageAsync(chosenWord);
        }

        LogCommand(ctx, "Speak", chosenWord);
        await HandleXpAsync(ctx);
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
        string[] Faces = { ":)", ":(", ":/" };
        string face = "";
        Random rand = new Random();
        string chosenResponse = responses[rand.Next(responses.Length)];
        if (chosenResponse == responses[0] || chosenResponse == responses[4])
        {
            face = Faces[0];
        }
        else if (chosenResponse == responses[1] || chosenResponse == responses[3] || chosenResponse == responses[5])
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

        LogCommand(ctx, "8ball", $"{user} asked {question} WadeBot responded with :{chosenResponse}");

        await HandleXpAsync(ctx);
    }

    [Command("RPS")]
    [Cooldown(1, 5, CooldownBucketType.User)]
    public async Task Rps(CommandContext ctx, string userChoice)
    {
        userChoice = userChoice.ToLower();
        string[] choices = { "rock", "paper", "scissors" };

        if (!choices.Contains(userChoice))
        {
            await ctx.RespondAsync("Choose **rock**, **paper**, or **scissors**.");
            return;
        }

        string botChoice = choices[rand.Next(choices.Length)];
        string result;

        if (userChoice == botChoice)
            result = "It's a tie! :O";
        else if (
            (userChoice == "rock" && botChoice == "scissors") ||
            (userChoice == "paper" && botChoice == "rock") ||
            (userChoice == "scissors" && botChoice == "paper")
        )
            result = "You win! :D";
        else
            result = "I win! :D";

        await ctx.Channel.SendMessageAsync(
            $"You chose **{userChoice}**. I chose **{botChoice}**. {result}"
        );

        LogCommand(ctx, "RPS", $"{ctx.User} vs bot — {result}");
        await HandleXpAsync(ctx);
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
        bool snitch = rand.Next(0, 2) == 1;

        string response;

        if (!snitch)
        {
            response = "🤐 Your secret is safe with me.";
            await ctx.Channel.SendMessageAsync("I aint no snitch");
            LogCommand(ctx, "Whisper", $"{user}  whispered: {secret} (Kept)");
        }
        else
        {
            response = $"📣 Yeah nah I'm snitching — {secret}";
            await ctx.Channel.SendMessageAsync($"Wow I cant believe {user.Username} said '{secret}'");
            LogCommand(ctx, "Whisper", $"{user}  whispered: {secret} (snitched)");
        }

        // Step 6: Send DM response
        await dm.SendMessageAsync(response);
        await HandleXpAsync(ctx);
    }


    [Command("PokemonGuesser")]
    [Cooldown(1, 5, CooldownBucketType.User)]
    public async Task PokemonGuesser(CommandContext ctx)
    {
        var answerFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pokemon.txt");
        if (!File.Exists(answerFilePath))
        {
            await ctx.RespondAsync($"Technical Error: answers file not found at `{answerFilePath}`");
            return;
        }

        var answers = File.ReadAllLines(answerFilePath)
                          .Select(s => s?.Trim())
                          .Where(s => !string.IsNullOrEmpty(s))
                          .ToArray();

        if (answers.Length == 0)
        {
            await ctx.RespondAsync("Technical Error: answers file is empty.");
            return;
        }

        // reuse a static Random or a shared instance in the class to avoid seed issues
        int dexNumber = rand.Next(1, answers.Length + 1);
        string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pokemon", $"{dexNumber}.png");

        if (!File.Exists(filepath))
        {
            await ctx.RespondAsync($"Technical Error: I looked for the image at `{filepath}` but it wasn't there!");
            return;
        }

        using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        {
            await new DiscordMessageBuilder()
                .WithContent("Who's that Pokémon?")
                .AddFile(fs)
                .SendAsync(ctx.Channel);
        }

        string correctName = answers[dexNumber - 1];

        var interactivity = ctx.Client.GetInteractivity();
        var response = await interactivity.WaitForMessageAsync(
            x => x.Content.Equals(correctName, StringComparison.OrdinalIgnoreCase) && x.Author.Id == ctx.User.Id,
            TimeSpan.FromSeconds(45));

        if (response.TimedOut)
            await ctx.RespondAsync($"Time's up😒! It was {correctName}.");
        else
            await ctx.RespondAsync("🎉You are correct YIPPEE🎉");

        LogCommand(ctx, "Pokemonguesser", $"{correctName} Was the chosen Pokemon");
        await HandleXpAsync(ctx);
    }

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
        var chosen = humans[rand.Next(humans.Count)];

        // Mention (ping) them
        await ctx.Channel.SendMessageAsync($"Hey {chosen.Mention}, you’ve been chosen!");
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        var user = ctx.User;

        LogCommand(ctx, "Roulette", $"{user} Pinged {chosen} with the bot");
        await HandleXpAsync(ctx);

    }
    #endregion



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