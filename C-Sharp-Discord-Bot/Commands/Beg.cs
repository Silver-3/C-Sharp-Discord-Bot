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
            Random random = new Random();
            int randomAmount = random.Next(1, 2501);

            string[] textArray = {
                "No money for you :P",
                $"Aww, take ${randomAmount} :)",
                $"Fine little begger, take ${randomAmount} and get a life",
                "Go beg to someone else",
                $"I'm going broke these days, I only have {randomAmount * 2}. You can have half of it :)",
                $"Take ${randomAmount} and frick off.",
                $"Shooo little pest, take this ${randomAmount} with you",
                $"Look at my purse, it has ${randomAmount}. Want it? Take it!",
                $"Just get a damn job! Oh wait, you have no money! Take ${randomAmount} and get a job.",
                "Never gonna give you coins.",
                $"💰 Money, Money, Money. Oh no, I dropped ${randomAmount}!",
                $"Never not gonna give you money, ${randomAmount}.",
                $"You begged on the streets and found ${randomAmount * 2}. Too bad you lost half of it.",
                "I only give money to friends.",
                $"Poor begger! Take ${randomAmount} :)",
                "*Bang*, *Bang*. Take this non-existing money",
                $"Fineee you dumb begger. I'll give you ${randomAmount}",
                $"Look at this ${randomAmount * 2}, too bad you only get half of it",
                $"You found ${randomAmount * 4} in the old lady's purse. She whacked you and you ran off with a quarter of it.",
                $"You begged for 24 hours. And made ${randomAmount} — you somehow duplicated it. I wonder how?"
            };

            string[] peopleArray = {
                "Liam Carter",
                "Olivia Bennett",
                "Noah Parker",
                "Emma Hayes",
                "Oliver Brooks",
                "Ava Mitchell",
                "Elijah Reed",
                "Sophia Collins",
                "James Turner",
                "Isabella Hughes",
                "William Foster",
                "Mia Richardson",
                "Lucas Gray",
                "Charlotte Evans",
                "Henry Price",
                "Amelia Morris",
                "Ethan Cooper",
                "Harper Ward",
                "Jack Phillips",
                "Evelyn Scott"
            };

            string randomText = textArray[random.Next(textArray.Length)];
            string randomPerson = peopleArray[random.Next(peopleArray.Length)];

            var embed = new EmbedBuilder()
                .WithTitle(randomPerson)
                .WithDescription(randomText)
                .WithColor(Discord.Color.Blue)
                .Build();

            await message.Channel.SendMessageAsync(embed: embed);

            if (randomText == textArray[0]) return;
            if (randomText == textArray[3]) return;
            if (randomText == textArray[9]) return;
            if (randomText == textArray[13]) return;
            if (randomText == textArray[15]) return;
            if (randomText == textArray[19]) randomAmount *= 2;

            db.AddMoney((long)message.Author.Id, randomAmount);
        }
    }
}
