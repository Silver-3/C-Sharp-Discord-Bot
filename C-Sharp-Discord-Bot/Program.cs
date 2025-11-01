using Discord;
using Discord.WebSocket;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class BotConfig
    {
        public string? Token { get; set; }
        public string? Prefix { get; set; }
        public string? ConnectionString { get; set; }
    }

    public interface ICommand
    {
        string Name { get; }
        Task ExecuteAsync(SocketMessage message, Database db);
    }

    class Program
    {
        private static DiscordSocketClient? _client;
        private static Database? db;
        private static BotConfig? Config;
        private static Dictionary<string, ICommand>? Commands;

        private void LoadCommands()
        {
            Commands = typeof(ICommand).Assembly
                .GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (ICommand)Activator.CreateInstance(t)!)
                .ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);
        }

        static async Task Main(string[] args)
        {
            await new Program().MainAsync();
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.MessageContent | GatewayIntents.GuildMessageReactions | GatewayIntents.DirectMessages | GatewayIntents.DirectMessageReactions
            });

            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;

            Config = JsonSerializer.Deserialize<BotConfig>(File.ReadAllText("./config.json"))
                     ?? throw new InvalidOperationException("Failed to load config.json");

            if (string.IsNullOrWhiteSpace(Config.ConnectionString))
                throw new InvalidOperationException("Missing ConnectionString in config.json");

            db = new Database();
            db.Connect(ConnectionString: Config.ConnectionString);

            LoadCommands();

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();

            Console.WriteLine("Bot is running. Press Ctrl+C to exit.");
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            var Prefix = Config?.Prefix ?? string.Empty;
            if (!Prefix.Equals(string.Empty) && !message.Content.StartsWith(Prefix)) return;

            string[] parts = message.Content.Substring(Prefix.Length).Split(' ', 2);
            string command = parts[0].ToLower();

            if (Commands != null && Commands.TryGetValue(command, out var cmd))
            {
                await cmd.ExecuteAsync(message, db!);
            }
        }
    }
}