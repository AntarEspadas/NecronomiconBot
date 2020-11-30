using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;

namespace NecronomiconBot.Modules
{
    public class Utility: NecroModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Alias("echo")]
        [Summary("Send a message in chat.")]
        public Task Say([Remainder][Summary("The texto to say")] string message)
            => ReplyAsync(message);

        [Command("avatar")]
        [Alias("profile", "pfp")]
        public Task Avatar([Remainder]SocketUser user)
            => ReplyAsync(user.GetAvatarUrl(size: 256));

        [Command("unedit")]
        public async Task UneditAsync()
        {
            IMessage message = await GetParentMessageAsync(Context.Message);
            if (message.EditedTimestamp != null)
            {
                var messages = Logic.MessageHistory.Instance.GetHistory(message);
                _ = SendAllHistory(messages);
            }
        }
        [Command("undelete")]
        public async Task UndeleteAsync()
        {
            var messages = Logic.MessageHistory.Instance.GetLastDeletedHistory(Context.Message.Channel);
            _ = SendAllHistory(messages);
        }
        private async Task SendAllHistory(ICollection<IMessage> messages)
        {
            foreach (var item in messages)
            {
                var eb = Quote(item.Author, item.Content, item.Timestamp);
                await ReplyAsync(embed: eb.Build());
            }
        }

    }
}
