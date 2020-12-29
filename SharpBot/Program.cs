using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace SharpBot
{
    class Program
    {
        public struct ConfigJSON
        {
            [JsonProperty("token")]
            public string Token { get; private set; }
            [JsonProperty("prefix")]
            public string Prefix { get; private set; }
        }

        private DiscordSocketClient _client;
        private static ConfigJSON config;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task MainAsync()
        {
            using var fs = File.OpenRead("config.json");
            using var sr = new StreamReader(fs, new UTF8Encoding(false));
            config = JsonConvert.DeserializeObject<ConfigJSON>(await sr.ReadToEndAsync().ConfigureAwait(false));

            _client = new DiscordSocketClient();
            _client.MessageReceived += CommandHandler;
            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public Task CommandHandler(SocketMessage message)
        {
            if (!message.Content.StartsWith(config.Prefix))
                return Task.CompletedTask;
            if (message.Author.IsBot)
                return Task.CompletedTask;

            string[] msgArr = message.Content.Split(" ");
            string command = msgArr[0].ToLower().Substring(config.Prefix.Length);
            string[] args = msgArr.Skip(1).ToArray();

            Console.WriteLine(command);

            if (command == "ping")
            {
                Console.WriteLine("5");
                Embed embed = new EmbedBuilder()
                    .WithDescription("Pong!!\nResponse time: `"+ _client.Latency.ToString() +"ms`")
                    .Build();
                message.Channel.SendMessageAsync("", false, embed);
                Console.WriteLine("6");
            }

            return Task.CompletedTask;
        }
    }
}
