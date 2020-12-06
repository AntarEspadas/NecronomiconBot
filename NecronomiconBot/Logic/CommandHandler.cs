using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NecronomiconBot.Settings;

namespace NecronomiconBot.Logic
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly NonStandardCommandHandler nonStandard;

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
            string naturalPrefix = GetPrefix("natural", channel.Guild.Id, channel.Id, message.Author.Id);
            string prefix = GetPrefix("synthetic", channel.Guild.Id, channel.Id, message.Author.Id);

            if (!(message.HasStringPrefix(naturalPrefix + " ", ref argPos) || message.HasStringPrefix(naturalPrefix + ", ", ref argPos)
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

        private string GetPrefix(string type, ulong guildId, ulong channelId, ulong userId)
        {
            if (BotSettings.Instance.GetChannelSettingOrDefault<bool>("prefix:use-user-defined", guildId, channelId)[0])
                return BotSettings.Instance.GetUserSettingOrDefault<string>($"prefix:{type}", guildId, userId)[0];
            return BotSettings.Instance.GetChannelSettingOrDefault<string>($"prefix:{type}", guildId, channelId)[0];
        }
    }
}
