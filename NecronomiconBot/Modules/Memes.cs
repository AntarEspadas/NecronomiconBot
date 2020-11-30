using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.Linq;

namespace NecronomiconBot.Modules
{
    public class Memes : NecroModuleBase<SocketCommandContext>
    {
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
            await ReplyAsync(uwutize(message.Content));
        }

        [Command("UwU")]
        [Alias("OwO", "UwUtize", "OwOtize")]
        public async Task UwutizeAsync([Remainder] string text)
        {
            await ReplyAsync(uwutize(text));
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
    }
}
