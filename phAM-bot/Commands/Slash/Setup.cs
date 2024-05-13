using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace phAM.Commands.Slash
{
    public class Setup : ApplicationCommandModule
    {
        [SlashCommand("admin-setup", "initial setup for phAM. requires administrator.")]
        [SlashRequireUserPermissions(Permissions.Administrator, false)]
        public static async Task InitialSetup(InteractionContext ctx, 
            [Option("db_addr", "addr of sql server")] string db_addr,
            [Option("username", "username for database")] string username,
            [Option("password", "password for database")] string password,
            [Option("PasswordHashWorkFactor", "from server's config.js file, default '8'")] long workFactor = 8,
            [Option("db_port", "port of sql server, default '3306'")] long db_port = 3306,
            [Option("auth_db", "name of auth database, default 'ace_auth'")] string auth_db = "ace_auth",
            [Option("max_accts", "max accts per user, default '0' for infinite")] long max_accts = 0)
        {
            DiscordEmbedBuilder message = new()
            {
                Title = $"{ctx.User.Username} used /admin-setup"
            };

            var new_guild = new Guild
            {
                Discord = ctx.Guild.Id,
                Server = db_addr,
                Port = db_port,
                Uid = username,
                Pwd = password,
                Database = auth_db,
                PasswordHashWorkFactor = long.Clamp(workFactor, 4, 31),
                Max = max_accts
            };

            if (new_guild.Validate())
            {
                message.Description = "you successfully connected this bot to a valid sql server. nice";
                message.Color = DiscordColor.DarkGreen;
            }
            else
            {
                message.Description = "wasnt able to validate the provided sql server. get fkd kid";
                message.Color = DiscordColor.DarkRed;
                Helper.DebugPrintFailure(ctx, message.Description);
            }

            Helper.DebugPrintSuccess(ctx, message.Title);
            await ctx.CreateResponseAsync(message, true);
        }
    }
}
