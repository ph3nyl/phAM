using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace phAM.Commands.Slash
{
    public class Help : ApplicationCommandModule
    {
        [SlashCommand("help", "r u dum dum? it gon be k")]
        public static async Task SlashLogAcct(InteractionContext ctx)
        {
            DiscordEmbedBuilder message = new()
            {
                Title = $"{ctx.User.Username} used /help",
                Color = DiscordColor.Gray,
                Description = $"gotchu fam"
            };

            message.AddField("*i cant change the password on my account*",
                            $"youre either trying to change the password on an account thats bound to someone else, or the account hasnt been bound to a discord id yet");

            message.AddField("*how do i bind an already existing account to my discord so it shows in the list?*",
                            $"log ingame on any character on the account, and use /pham bind <discord id>");

            message.AddField("*/pham doesnt work ingame*", 
                            $"your ace server doesnt have the phAM harmony mod installed");

            message.AddField("*i bound my discord while logged into my character and the account still isnt showing in the list*", 
                            $"make sure you bind your discord id not your discord name. its a long string of numbers, not your username");

            message.AddField("*ok so how do i find my discord id?*", 
                            $"`User Settings->Advanced->Developer Mode = true`\nthen r/l click on your name somewhere and copy id\n<https://www.youtube.com/watch?v=mc3cV57m3mM>");

            message.AddField("*why is <something about this bot/mod> the way it is?*", 
                            $"idk probably some combination of autism and adhd");

            message.AddField("*are you saving my password to steal all of my loot?*", 
                            $"user account information isnt saved persistantly anywhere except on the sql server your discord admin entered in /admin-setup");

            message.AddField("*[admin] do i* **need** *to use the phAM harmony mod on my server?*", 
                            $"no, but only accounts created with this bot will automatically have a bound discord id and be manageable. you can manually enter a discord id into the email_Address field for an account on ace_auth if you want to.");

            message.AddField("*[admin] do i need to give the bot the* **root** *sql user account credentials?*", 
                            $"no, you can make a seperate user account with SELECT/UPDATE/INSERT permissions on ace_auth for normal functionality");//, and SELECT permissions on ace_shard for the character-related functionality");

            Helper.DebugPrintSuccess(ctx, message.Title);
            await ctx.CreateResponseAsync(message, true);
        }
    }
}
