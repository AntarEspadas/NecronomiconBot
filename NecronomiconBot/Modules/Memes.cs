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
    public class Memes : ModuleBase<SocketCommandContext>
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
            if (reference == null)
            {
                var asyncMessages = Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 1);
                var messages = await AsyncEnumerableExtensions.FlattenAsync(asyncMessages);
                foreach (var item in messages)
                {
                    message = item;
                }
            }
            else
            {
                var guild = Context.Client.GetGuild(reference.GuildId.Value);
                var channel = guild.GetTextChannel(reference.ChannelId);
                message = await channel.GetMessageAsync(reference.MessageId.Value);
            }
            await Context.Channel.SendFileAsync(Logic.AlwaysHasBeen.GetImage(message.Author.Username, Context.Message.Author.Username, message.Content), "Always has been.png");

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

    }
}
