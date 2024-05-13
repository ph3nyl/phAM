using DSharpPlus.SlashCommands;
using LiteDB;
using MySql.Data.MySqlClient;

namespace phAM
{
    public static class Helper
    {
        public static Guild GetGuild(InteractionContext ctx)
        {
            Guild? g = null;
            using (LiteDatabase db = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "guilds.db")))
            {
                try
                {
                    var guilds = db.GetCollection<Guild>("guilds");
                    guilds.EnsureIndex(x => x.Discord);
                    g = guilds.FindOne(x => x.Discord == ctx.Guild.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return g;
        }

        public static string GetSQLConStr(Guild guild)
        {
            MySqlConnectionStringBuilder constr = new()
            {
                Server = guild.Server,
                Port = (uint)guild.Port,
                Database = guild.Database,
                UserID = guild.Uid,
                Password = guild.Pwd
            };
            return constr.ToString();
        }

        public static void DebugPrintSuccess(InteractionContext ctx, string message)
        {
            if (Program.Config?.Debug == true) Console.WriteLine($"[{ctx.Guild.Name}] {message}");
        }

        public static void DebugPrintFailure(InteractionContext ctx, string message)
        {
            if (Program.Config?.Debug == true) Console.WriteLine($"[{ctx.Guild.Name}] <{ctx.User.Username}> {message}");
        }
    }
}
