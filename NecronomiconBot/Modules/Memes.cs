using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using NecronomiconBot.Logic.Distribution;

namespace NecronomiconBot.Modules
{
    public class Memes : NecroModuleBase<SocketCommandContext>
    {
        private static readonly string assetsFolder = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "assets");
        [Command("step on me")]
        public Task StepOnMe()
        {
            string[] replies = { 
                "Kimoi",
                "Kimochi warui",
                "Disgusting",
                "Pathetic",
                "Shine",
                "Go kill yourself"
            };
            Random rand = new Random();
            int index = rand.Next(0, replies.Length);
            return ReplyAsync(replies[index]);
        }
        
        [Command("always has been")]
        [Alias("ahb")]
        public async Task AlwaysHasBeen()
        {
            var reference = Context.Message.Reference;
            IMessage message = null;

            message = await GetParentMessageAsync(Context.Message);

            _ = GetImageAndSendAsync(message.Author.Username, Context.Message.Author.Username, message.Content);

        }

        private async Task GetImageAndSendAsync(string astronaut1, string astronaut2, string messageContetn)
        {
            await Context.Channel.SendFileAsync(Logic.AlwaysHasBeen.GetImage(astronaut1, astronaut2, messageContetn), "Always has been.png");
        }

        [Command("tell")]
        public async Task Tell(SocketUser user, [Remainder] string sentence)
        {
            Regex regex = new Regex("to go fuck (himself)|(herself)|(themselves)|(itself)");
            if (regex.IsMatch(sentence))
            {
                if (Program.Authors.Contains(user.Id))
                {
                    await ReplyAsync($"I love you {user.Mention}");
                }
                else
                {
                    await ReplyAsync($"{user.Mention}, go fuck yourself");
                }
            }
        }

        [Command("UwU")]
        [Alias("OwO", "UwUtize", "OwOtize")]
        public async Task UwutizeAsync()
        {
            var message = await GetParentMessageAsync(Context.Message);
            var embed = Quote(message.Author, uwutize(message.Content), null);
            await ReplyAsync(embed: embed.Build());
        }

        [Command("UwU")]
        [Alias("OwO", "UwUtize", "OwOtize")]
        public async Task UwutizeAsync([Remainder] string text)
        {
            var embed = Quote(Context.Message.Author, uwutize(text));
            await ReplyAsync(embed:embed.Build());
        }

        private string uwutize(string text)
        {
            string[] faces = { "(・\\`ω´・)", ";;w;;", "owo", "UwU", ">w<", "^w^" };
            var regex = new Regex("(?:r|l)");
            text = regex.Replace(text, "w");
            regex = new Regex("(?:R|L)");
            text = regex.Replace(text, "W");
            regex = new Regex("n(?=[aeiouáéíóú])");
            text = regex.Replace(text, "ny");
            regex = new Regex("N(?=[aeiouáéíóú])");
            text = regex.Replace(text, "ny");
            regex = new Regex("N(?=[AEIOUÁÉÍÓÚ])");
            text = regex.Replace(text, "ny");
            regex = new Regex("(?<=[pt])o");
            text = regex.Replace(text, "wo");
            regex = new Regex("!+");
            var random = new Random();
            text = regex.Replace(text, $" {faces[random.Next(0, faces.Length)]} ");
            return text;
        }
        [Command("FakeQuote")]
        [Alias("Quote", "Fake","Gandhi")]
        public async Task FakeQuote()
        {
            var message = await GetParentMessageAsync(Context.Message);
            await FakeQuote(message.Content);
        }
        [Command("FakeQuote")]
        [Alias("Quote", "Fake", "Gandhi")]
        public async Task FakeQuote([Remainder]string text)
        {
            string[,] people = {
                {"Gandhi","https://upload.wikimedia.org/wikipedia/commons/d/d1/Portrait_Gandhi.jpg" },
                {"Abraham Lincoln","https://upload.wikimedia.org/wikipedia/commons/a/ab/Abraham_Lincoln_O-77_matte_collodion_print.jpg" },
                {"Dalai Lama","https://upload.wikimedia.org/wikipedia/commons/3/3a/HH_The_Dalai_Lama_%288098007806%29.jpg" },
                {"Albert Einstein", "https://upload.wikimedia.org/wikipedia/commons/d/d3/Albert_Einstein_Head.jpg" },
                {"Hayao Miyazaki", "https://upload.wikimedia.org/wikipedia/commons/6/65/Hayao_Miyazaki_cropped_3_Hayao_Miyazaki_201211.jpg" },
                {"God", "https://upload.wikimedia.org/wikipedia/commons/1/13/Michelangelo%2C_Creation_of_Adam_06.jpg" },
                {"Confucius","https://upload.wikimedia.org/wikipedia/commons/9/9a/Confucius_the_scholar.jpg" }
            };
            var index = new Random().Next(0,people.GetLength(0));
            var eb = new EmbedBuilder()
            {
                Title = text,
                Description = $"*-{people[index, 0]}*",
                ThumbnailUrl = people[index, 1],
            };
            await ReplyAsync(embed: eb.Build());
        }
        public async Task Vente()
        {
            string ahegaosFolder = Path.Combine(assetsFolder, "ahegaos");
            if (!Directory.Exists(ahegaosFolder))
                return;
            string[] ahegaos = Directory.EnumerateFiles(ahegaosFolder).ToArray();
            if (ahegaos.Length > 0)
            {
                string ahegao = ahegaos.RandomElement();
                await Context.Channel.SendFileAsync(ahegao);
            }
        }

        public async Task A()
        {
            string aPath = Path.Combine(assetsFolder, "a", "a.gif");
            if (!File.Exists(aPath)) return;
            await Context.Channel.SendFileAsync(aPath);
        }
    }
}
