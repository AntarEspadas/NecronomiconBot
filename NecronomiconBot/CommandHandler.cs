using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NecronomiconBot
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message))
                return;

            int argPos = 0;

            if (!(message.HasStringPrefix("navi ", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                if (message.Author.IsBot)
                    return;
                string[] nonPrefixCommands = { 
                    "always has been",
                    "ahb"
                };
                if (!nonPrefixCommands.Contains(message.Content.ToLower()))
                {
                    return;
                }
                else
                {
                    argPos = 0;
                }
            }

            var context = new SocketCommandContext(_client, message);
            
            await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);

        }
    }
}
