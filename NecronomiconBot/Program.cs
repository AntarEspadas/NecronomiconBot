using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NecronomiconBot.Logic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using NecronomiconBot.Settings;

namespace NecronomiconBot
{
    class Program
    {

        public static readonly IReadOnlyCollection<ulong> Authors = new List<ulong>() { 189514032242360320/*Naratna*/ }.AsReadOnly();
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

            BotSettings.Init(Path.Combine(".", "BotSettings.json"), Path.Combine(".", "BotSettingsSchema.json"));
            var settings = BotSettings.Instance;
            if (settings.Token == null || settings.Token == string.Empty)
            {
                while (true)
                {
                    Console.WriteLine("Please enter your bot's token:");
                    settings.Token = Console.ReadLine();
                    if (settings.Token != string.Empty)
                        break;
                }
                
            }
            else
            {
                Console.WriteLine("Please enter your bot's token (leave empty to use previously used token):");
                string tokenValue = Console.ReadLine();
                if (tokenValue != string.Empty)
                {
                    settings.Token = tokenValue;
                }
            }
            settings.Save();

            var socketConfig = new DiscordSocketConfig();
            socketConfig.MessageCacheSize = 100;
            _client = new DiscordSocketClient(socketConfig);
            //_client.MessageReceived += LogMessage;
            _client.Log += Log;
            _client.MessageUpdated += Logic.MessageHistory.Instance.AddAsync;
            _client.MessageDeleted += Logic.MessageHistory.Instance.AddDeletedAsync;

            _commands = new CommandService();

            _handler = new CommandHandler(_client, _commands);

            await _handler.InstallCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, settings.Token);
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
        }

    }
}
