using Discord;
using Discord.WebSocket;
using Microsoft.Data.Sqlite;
using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class Balance : ICommand
    {
        public string Name => "balance";

        public async Task ExecuteAsync(SocketMessage message, Database db)
        {
            long userId = (long)message.Author.Id;
            int balance = db.GetMoney(userId);

            var embed = new EmbedBuilder()
                .WithTitle($"{message.Author.Username}'s balance")
                .WithDescription($"Purse: {balance}")
                .WithColor(Discord.Color.Blue)
                .Build();

            await message.Channel.SendMessageAsync(embed: embed);
        }
    }
}
