using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace NecronomiconBot
{
    class Program
    {

        public static readonly IReadOnlyCollection<ulong> Authors = new List<ulong>() { 189514032242360320/*Naratna*/ }.AsReadOnly();
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private DiscordSocketClient _client;
        private CommandService _commands;
        private CommandHandler _handler;

        static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            if (false)
            {
                Testing();
                return;
            }
            if (args.Length == 1)
            {
                config.AppSettings.Settings["Token"].Value = args[0];
                config.Save();
            }
            string token = config.AppSettings.Settings["Token"].Value;

            _client = new DiscordSocketClient();
            //_client.MessageReceived += LogMessage;
            _client.Log += Log;

            _commands = new CommandService();

            _handler = new CommandHandler(_client, _commands);

            await _handler.InstallCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task LogMessage(SocketMessage message)
        {
            Console.WriteLine(
                $"\nActivity: {message.Activity}"+
                $"\n{message.Application}" +
                $"\n{message.Attachments}" +
                $"\n{message.Author}" +
                $"\n{message.Channel}" +
                $"\n{message.Content}" +
                $"\n{message.CreatedAt}" +
                $"\n{message.EditedTimestamp}" +
                $"\n{message.Embeds}" +
                $"\n{message.Id}" +
                $"\n{message.IsPinned}" +
                $"\n{message.IsSuppressed}" +
                $"\n{message.IsTTS}" +
                $"\n{message.MentionedChannels}" +
                $"\n{message.MentionedRoles}" +
                $"\n{message.MentionedUsers}" +
                $"\n{message.Reactions}" +
                $"\n{message.Reference}" +
                $"\n{message.Source}" +
                $"\n{message.Tags}" +
                $"\n{message.Timestamp}"
                );
            
            return Task.CompletedTask;
        }

        static void Testing()
        {
            Console.WriteLine(config.AppSettings.Settings["unknown"]);
        }

    }
}
