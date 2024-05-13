using phAM.Config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using phAM.Commands.Slash;

namespace phAM
{
    public sealed class Program
    {
        public static DiscordClient? Client { get; set; }
        public static CommandsNextExtension? Commands { get; set; }
        public static BotConfig? Config { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        static async Task Main(string[] args)
        {
            Config = new BotConfig();
            await Config.ReadJSON();

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = Config.DiscordBotToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(config);

            Client.Ready += Client_Ready;

            var slashCommandsConfig = Client.UseSlashCommands();
            slashCommandsConfig.RegisterCommands<Help>();
            slashCommandsConfig.RegisterCommands<Setup>();
            slashCommandsConfig.RegisterCommands<Stats>();
            slashCommandsConfig.RegisterCommands<NewAccount>();
            slashCommandsConfig.RegisterCommands<FixAccount>();
            slashCommandsConfig.RegisterCommands<LogAccount>();

            Console.WriteLine("========================================================================================================================\n" +
                              "                                                      phAM - 0.0.1                                                      \n" +
                              "                                       i dont know what goes here im not good wi\n" +
                              "========================================================================================================================");

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
