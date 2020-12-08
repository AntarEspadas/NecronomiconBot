using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using NecronomiconBot.Logic;

namespace NecronomiconBot.Modules
{
    public class Utility: NecroModuleBase<SocketCommandContext>
    {
        public async Task OnMention()
        {
            string naturalPrefix = CommandHandler.GetPrefix("natural", Context.Guild.Id, Context.Channel.Id, Context.User.Id);
            string syntheticPrefix = CommandHandler.GetPrefix("synthetic", Context.Guild.Id, Context.Channel.Id, Context.User.Id);
            string message = $"You can call me using synthetic prefix `{syntheticPrefix}`, natural prefix `{naturalPrefix}` or by mentioning me ({Context.Client.CurrentUser.Mention})\n" +
                $"Examples:\n" +
                $"`{syntheticPrefix}say hi`\n" +
                $"`{naturalPrefix} say hi`\n" +
                $"`{Context.Client.CurrentUser.Mention}` say hi`";
            await ReplyAsync(message);
        }

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
