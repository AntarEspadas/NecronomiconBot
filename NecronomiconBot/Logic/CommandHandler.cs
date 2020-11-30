using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NecronomiconBot.Logic
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly NonStandardCommandHandler nonStandard;
        private static readonly string default_natural_prefix = "navi";
        private static readonly string default_prefix = "n.";

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
            nonStandard = new NonStandardCommandHandler(_client, _commands);
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

            //string naturalPrefix = GetNaturalPrefix(messageParam.Source)
            var channel = message.Channel as SocketGuildChannel;
            string natural_prefix = GetNaturalPrefix(channel.Guild.Id);
            string prefix = GetPrefix(channel.Guild.Id);

            if (!(message.HasStringPrefix(natural_prefix + " ", ref argPos) || message.HasStringPrefix(natural_prefix + ", ", ref argPos)
                || message.HasStringPrefix(prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                if (message.Author.IsBot)
                    return;
                string[] nonPrefixCommands = { 
                    "always has been",
                    "ahb"
                };
                if (!nonPrefixCommands.Contains(message.Content.ToLower()))
                {
                    _ = nonStandard.HandleCommandAsync(message);
                    return;
                }
                else
                {
                    argPos = 0;
                }
            }

            var context = new SocketCommandContext(_client, message);
            
            var result = await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
            if (!result.IsSuccess)
            {
                Console.WriteLine($"Error: {result.Error}");
                Console.WriteLine($"Reason: {result.ErrorReason}");
            }

        }

        private string GetNaturalPrefix(ulong guildId)
        {
            var prefix = Program.config.AppSettings.Settings["natural_prefix_" + guildId];
            return prefix == null ? default_natural_prefix : prefix.Value;
        }

        private string GetPrefix(ulong guildId)
        {
            var prefix = Program.config.AppSettings.Settings["prefix_" + guildId];
            return prefix == null ? default_prefix : prefix.Value;
        }
    }
}
