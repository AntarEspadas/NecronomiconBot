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

        [Command("userinfo")]
        [Summary ("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync(
        [Summary("The (optional) user to get info from")] [Remainder]SocketUser user = null)
        {
            var userInfo = user ?? Context.User;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }
    }
}
