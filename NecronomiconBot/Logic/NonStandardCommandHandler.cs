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
            if (new Random().Next(0,1) == 0)
            {
                var memes = new Memes();
                memes.SetContext(context);
                await memes.Gandhi();
            }
        }
    }
}
