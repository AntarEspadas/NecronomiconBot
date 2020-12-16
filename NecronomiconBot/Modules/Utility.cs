using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using NecronomiconBot.Logic;
using NecronomiconBot.Settings;

namespace NecronomiconBot.Modules
{
    public class Utility: NecroModuleBase<SocketCommandContext>
    {
        public async Task OnMention()
        {
            string naturalPrefix = CommandHandler.GetPrefix("natural", Context.Guild.Id, Context.Channel.Id, Context.User.Id);
            string syntheticPrefix = CommandHandler.GetPrefix("synthetic", Context.Guild.Id, Context.Channel.Id, Context.User.Id);
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>()
            {
                new EmbedFieldBuilder(){Value = $"{syntheticPrefix}help", Name = "Synthetic prefix:"},
                new EmbedFieldBuilder(){Value = $"{naturalPrefix} help | {naturalPrefix}, help", Name = "Natural prefix:"},
                new EmbedFieldBuilder(){Value = $"{Context.Client.CurrentUser.Mention} help", Name = "Mention:"}
            };
            var eb = new EmbedBuilder()
            {
                Title = "Use the help command to know what I can do",
                Fields = fields
                
            };
            await ReplyAsync(embed:eb.Build());
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
            if (message.EditedTimestamp is null)
                return;
            if (BotSettings.Instance.GetChannelSettingOrDefault<bool>("unedit:opt-out", Context.Guild.Id, Context.Channel.Id)[0])
            {
                await ReplyAsync("Sorry, that command is disabled for this channel or server");
                return;
            }
            if (BotSettings.Instance.GetUserSettingOrDefault<bool>("unedit:opt-out", Context.Guild.Id, message.Author.Id)[0])
            {
                await ReplyAsync("Sorry, the targeted user has opted out of this feature");
                return;
            }
            var messages = Logic.MessageHistory.Instance.GetHistory(message);
            if (messages is null)
                return;
            _ = SendAllHistory(messages);
        }
        [Command("undelete")]
        public async Task UndeleteAsync()
        {
            if (BotSettings.Instance.GetChannelSettingOrDefault<bool>("undelete:opt-out", Context.Guild.Id, Context.Channel.Id)[0])
            {
                await ReplyAsync("Sorry, that command is disabled for this channel or server");
                return;
            }
            var messages = Logic.MessageHistory.Instance.GetLastDeletedHistory(Context.Message.Channel);
            if (messages is null)
                return;
            if (BotSettings.Instance.GetUserSettingOrDefault<bool>("undelete:opt-out", Context.Guild.Id, messages.First.Value.Author.Id)[0])
            {
                await ReplyAsync("Sorry, the latest user to have deleted a message in this channel has opted out of this feature");
                return;
            }
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

        [Command("help")]
        [Alias("h","-h","--help","halp")]
        public async Task Help()
        {
            await ReplyAsync("Sorry, this command has not yet been implemented");
        }

    }
}
