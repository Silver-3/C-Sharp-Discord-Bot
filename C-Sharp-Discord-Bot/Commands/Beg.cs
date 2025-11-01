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
    public class Beg : ICommand
    {
        public string Name => "beg";

        public async Task ExecuteAsync(SocketMessage message, Database db)
        {
            
        }
    }
}
