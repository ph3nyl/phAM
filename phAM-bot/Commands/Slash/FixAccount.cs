using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LiteDB;
using MySql.Data.MySqlClient;

namespace phAM.Commands.Slash
{
    public class FixAccount : ApplicationCommandModule
    {
        [SlashCommand("fix-acct", "changes the password on an account attached to this discord id")]
        public static async Task SlashFixAcct(InteractionContext ctx,
            [Option("username", "username of account to modify")] string username,
            [Option("password", "desired password for account")] string password)
        {
            DiscordEmbedBuilder message = new()
            {
                Title = $"{ctx.User.Username} used /fix-acct"
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

            using (MySqlConnection c = new(Helper.GetSQLConStr(g)))
            {
                try
                {
                    c.Open();

                    
                    string sql = $"SELECT * FROM `account` WHERE accountName=\"{username}\"";
                    MySqlCommand cmd = new(sql, c);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    //does the specified account exist
                    if (!rdr.HasRows)
                    {
                        message.Description = "specified account doesnt exist";
                        message.Color = DiscordColor.Red;
                        rdr.Close();
                        Helper.DebugPrintFailure(ctx, message.Description);
                        await ctx.CreateResponseAsync(message, true);
                        return;
                    }

                    //does the user own the account
                    rdr.Read();
                    if (DBNull.Value.Equals(rdr.GetValue(rdr.GetOrdinal("email_Address"))))
                    {
                        message.Description = "that account is unbound. see /help";
                        message.Color = DiscordColor.Red;
                        rdr.Close();
                        Helper.DebugPrintFailure(ctx, message.Description);
                        await ctx.CreateResponseAsync(message, true);
                        return;
                    }
                    if ((string)rdr.GetValue(rdr.GetOrdinal("email_Address")) != ctx.User.Id.ToString())
                    {
                        message.Description = "that account is bound to a different user. fk off";
                        message.Color = DiscordColor.Red;
                        rdr.Close();
                        Helper.DebugPrintFailure(ctx, message.Description);
                        await ctx.CreateResponseAsync(message, true);
                        return;
                    }
                    rdr.Close();

                    //update password
                    sql = $"UPDATE `account` SET `passwordHash` = '{BCrypt.Net.BCrypt.HashPassword(password, (int)g.PasswordHashWorkFactor)}' WHERE `account`.`accountName` = \"{username}\"";
                    cmd = new MySqlCommand(sql, c);
                    cmd.ExecuteNonQuery();

                    message.Description = $"you successfully changed the password for the account \"{username}\". good jorb";
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
