using Newtonsoft.Json;

namespace phAM.Config
{
    public class BotConfig
    {
        public string? DiscordBotToken { get; set; }
        public string? DiscordBotPrefix { get; set; }
        public bool? Debug { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new($"{AppDomain.CurrentDomain.BaseDirectory}/config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStruct? obj = JsonConvert.DeserializeObject<JSONStruct>(json);

                if(obj?.Token!=null)this.DiscordBotToken = obj.Token;
                if(obj?.Prefix != null)this.DiscordBotPrefix = obj.Prefix;
                if(obj?.Debug != null)this.Debug = obj.Debug;
            }
        }
    }
    internal sealed class JSONStruct
    {
        public string? Token { get; set; }
        public string? Prefix { get; set; }
        public bool? Debug { get; set;}
    }
}
