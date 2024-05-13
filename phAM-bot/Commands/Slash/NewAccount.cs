using ACE.Database.Models.Auth;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MySql.Data.MySqlClient;


namespace phAM.Commands.Slash
{
    public class NewAccount : ApplicationCommandModule
    {
        [SlashCommand("new-acct", "create new deadgame account. joy.")]
        public static async Task SlashNewAcct(InteractionContext ctx,
            [Option("username", "desired username for new account")] string username,
            [Option("password", "desired password for new account")] string password)
        {
            DiscordEmbedBuilder message = new()
            {
                Title = $"{ctx.User.Username} used /new-acct"
            };

            Guild? g = Helper.GetGuild(ctx);

            if (g == null)
            {
                message.Description = "invalid discord guild. a discord admin needs to run /admin-setup";
                message.Color = DiscordColor.Red;
                Helper.DebugPrintFailure(ctx, message.Description);
                await ctx.CreateResponseAsync(message, true);
                return;
            }

            //Guild g is a valid non-null Guild
            Account a = new()
            {
                AccountName = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, (int)g.PasswordHashWorkFactor),
                PasswordSalt = "use bcrypt",
                AccessLevel = 0, //player
                EmailAddress = ctx.User.Id.ToString(),
                CreateIP = [69, 69, 69, 69]
            };

            using (MySqlConnection c = new(Helper.GetSQLConStr(g)))
            {
                try
                {
                    c.Open();

                    //check for max accounts
                    
                    string sql = $"SELECT * FROM `account` WHERE email_Address=\"{ctx.User.Id}\"";
                    MySqlCommand cmd = new(sql, c);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows && g.Max > 0)
                    {
                        int i = 0;
                        while (rdr.Read())
                        {
                            ++i;
                        }
                        if (i >= g.Max && i > 0)
                        {
                            message.Description = $"you cant make any more accounts. there are already {i} accounts bound to your discord id, and server config says max of {g.Max} accounts per discord id";
                            message.Color = DiscordColor.Red;
                            rdr.Close();
                            Helper.DebugPrintFailure(ctx, message.Description);
                            await ctx.CreateResponseAsync(message, true);
                            return;
                        }
                    }
                    rdr.Close();

                    //check for existing account name
                    sql = $"SELECT * FROM `account` WHERE accountName=\"{a.AccountName}\"";
                    cmd = new(sql, c);
                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        //dupicate account name
                        message.Description = "duplicate account name. did you mean to /fix-acct? idk try something else";
                        message.Color = DiscordColor.Red;
                        rdr.Close();
                        Helper.DebugPrintFailure(ctx, message.Description);
                        await ctx.CreateResponseAsync(message, true);
                        return;
                    }
                    rdr.Close();

                    //insert new account
                    sql = $"INSERT INTO `account` (`accountName`, `passwordHash`, `passwordSalt`, `accessLevel`, `email_Address`, `create_Time`, `create_I_P`) VALUES(\"{a.AccountName}\", \"{a.PasswordHash}\", \"{a.PasswordSalt}\", \"{a.AccessLevel}\", \"{a.EmailAddress}\", CURRENT_TIMESTAMP, \"0x45454545\")";
                    cmd = new MySqlCommand(sql, c);
                    cmd.ExecuteNonQuery();

                    message.Description = $"you have successfully created the account \"{a.AccountName}\". well done";
                    message.Color = DiscordColor.Green;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    c.Close();
                    message.Description = "connection problem with auth server. maybe it's offline? idk";
                    message.Color = DiscordColor.Red;
                    Helper.DebugPrintFailure(ctx, message.Description);
                }
            }

            Helper.DebugPrintSuccess(ctx, message.Title);
            await ctx.CreateResponseAsync(message, true);
        }

    }
}
