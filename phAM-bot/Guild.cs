using MySql.Data.MySqlClient;
using LiteDB;
using System;

namespace phAM
{
    public class Guild()
    {
        public Int64 Id { get; set; }
        public ulong Discord { get; set; }
        public string? Server { get; set; }
        public long Port { get; set; }
        public string? Database { get; set; }
        public string? Uid { get; set; }
        public string? Pwd { get; set; }
        public long PasswordHashWorkFactor { get; set; }
        public long Max { get; set; }

        public bool Validate()
        {
            MySqlConnectionStringBuilder connectionString = new()
            {
                Server = Server,
                Port = (uint)Port,
                Database = Database,
                UserID = Uid,
                Password = Pwd
            };

            using (MySqlConnection connection = new(connectionString.ToString()))
            {
                try
                {
                    connection.Open();
                }
                catch //(Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    connection.Close();
                    return false;
                }
            }

            using (LiteDatabase db = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "guilds.db")))
            {
                try
                {
                    var guilds = db.GetCollection<Guild>("guilds");
                    foreach (var guild in guilds.FindAll())
                    {
                        if (guild.Discord == Discord)
                        {
                            guilds.Delete(guild.Id);
                        }
                    }
                    guilds.Insert(this);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            return true;
        }


    }
}
