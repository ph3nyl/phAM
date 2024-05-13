using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MySql.Data.MySqlClient;
using phAM.Config;

namespace phAM.Commands.Slash
{
    public class LogAccount : ApplicationCommandModule
    {
        [SlashCommand("log-acct", "list all acounts bound to user's discord id.")]
        public static async Task SlashLogAcct(InteractionContext ctx)
        {
            DiscordEmbedBuilder message = new()
            {
                Title = $"{ctx.User.Username} used /log-acct"
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

                    //check for existing account name
                    string sql = $"SELECT * FROM `account` WHERE email_Address=\"{ctx.User.Id}\"";
                    MySqlCommand cmd = new(sql, c);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                       
                        //message.Description += "```[#]\tACCOUNT\t\tLAST LOGGED IN\n";
                        int i = 0;
                        string a_number = "", a_name = "", a_logged = "";
                        while (rdr.Read())
                        {
                            //message.Description += $"[{++i}]\t{rdr.GetValue(rdr.GetOrdinal("accountName"))}\t\t{rdr.GetValue(rdr.GetOrdinal("last_Login_Time"))}\n";
                            a_number += $"[{++i}]\n";
                            a_name += $"{rdr.GetValue(rdr.GetOrdinal("accountName"))}\n";
                            a_logged += $"{rdr.GetValue(rdr.GetOrdinal("last_Login_Time"))}\n";
                        }
                        message.AddField("[#]", a_number, true);
                        message.AddField("account name", a_name, true);
                        message.AddField("last login time", a_logged, true);
                        message.Description = $"you have {i} accounts bound to your discord id";

                        if (g.Max > 0)
                        {
                            message.Description += $", out of a maximum {g.Max} accounts.";
                        }


                        message.Color = DiscordColor.Green;
                    }
                    else
                    {
                        message.Description = "you have no accounts bound to your discord id.";
                        message.Color = DiscordColor.Red;
                        Helper.DebugPrintFailure(ctx, message.Description);
                    }
                    rdr.Close();

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
