using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NecronomiconBot.Logic
{
    class MessageHistory
    {
        public static MessageHistory Instance { get => GetEditHistory(); }
        private static MessageHistory instance = null;

        public Dictionary<ulong, Dictionary<ulong, Dictionary<ulong, LinkedList<IMessage>>>> Guilds { get; private set; }
        public LinkedList<IMessage> this[ulong guildId, ulong channelId, ulong messageId] { get => GetHistory(guildId, channelId, messageId); }


        private static MessageHistory GetEditHistory()
        {
            if (instance == null)
                instance = new MessageHistory();
            return instance;
        }

        private MessageHistory()
        {
            Guilds = new Dictionary<ulong, Dictionary<ulong, Dictionary<ulong, LinkedList<IMessage>>>>();
        }

        public void Add(IMessage message)
        {
            var channel = message.Channel as SocketTextChannel;
            ulong guildId = channel.Guild.Id;
            ulong channelId = channel.Id;
            ulong messageId = message.Id;

            if (!Guilds.ContainsKey(guildId))
            {
                Guilds[guildId] = new Dictionary<ulong, Dictionary<ulong, LinkedList<IMessage>>>();
            }
            var channels = Guilds[guildId];
            if (!channels.ContainsKey(channelId))
            {
                channels[channelId] = new Dictionary<ulong, LinkedList<IMessage>>();
            }
            var messages = channels[channelId];
            if (!messages.ContainsKey(messageId))
            {
                messages[messageId] = new LinkedList<IMessage>();
            }
            var messageHistory = messages[messageId];
            messageHistory.AddLast(message);
        }

        public async Task AddAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            if (before.HasValue)
            {
                Add(await before.GetOrDownloadAsync());
            }
        }

        public LinkedList<IMessage> GetHistory(IMessage message)
        {
            var channel = message.Channel as SocketTextChannel;
            ulong guildId = channel.Guild.Id;
            ulong channelId = channel.Id;
            ulong messageId = message.Id;

            return this[guildId, channelId, messageId];
        }

        private LinkedList<IMessage> GetHistory(ulong guildId, ulong channelId, ulong messageId)
        {
            Dictionary<ulong, Dictionary<ulong, LinkedList<IMessage>>> channels;
            if (!Guilds.TryGetValue(guildId, out channels))
                return null;
            Dictionary<ulong, LinkedList<IMessage>> messages;
            if (!channels.TryGetValue(channelId, out messages))
                return null;
            LinkedList<IMessage> messageHistory;
            if (!messages.TryGetValue(messageId, out messageHistory))
                return null;
            return messages[messageId];
        }
    }
}
