using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.WebSocket;
using NecronomiconBot.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NecronomiconBot.Logic
{
    class NonStandardCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public NonStandardCommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task HandleCommandAsync(SocketUserMessage message)
        {
            var context = new SocketCommandContext(_client, message);
            int argPos = 0;
            if (IsOnlyMention(message, _client.CurrentUser))
            {
                var utility = new Utility();
                utility.SetContext(context);
                await utility.OnMention();
            }
            else if (new Random().Next(0,500) == 69)
            {
                var memes = new Memes();
                memes.SetContext(context);
                await memes.FakeQuote(message.Content);
            }
        }

        private bool IsOnlyMention(IMessage message, IUser user)
        {
            string content = message.Content;
            if (string.IsNullOrEmpty(content) || content.Length < 4 || content[0] != '<' || content[1] != '@' || content[content.Length -1] != '>')
                return false;
            if (!MentionUtils.TryParseUser(content, out ulong userId))
                return false;
            if (userId == user.Id)
                return true;
            return false;
        }
    }
}
