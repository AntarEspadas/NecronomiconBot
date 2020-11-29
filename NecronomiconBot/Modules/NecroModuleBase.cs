using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NecronomiconBot.Modules
{
    public class NecroModuleBase<T> : ModuleBase<T> where T: class, ICommandContext
    {
        protected async Task<IMessage> GetPreviousMessageAsync(IMessage message)
        {
            IMessage previousMessage = null;
            var asyncMessages = Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 1);
            var messages = await AsyncEnumerableExtensions.FlattenAsync(asyncMessages);
            foreach (var item in messages)
            {
                previousMessage = item;
            }
            return previousMessage;
        }

        protected async Task<IMessage> GetReferencedMessageAsync(IMessage message)
        {
            var reference = message.Reference;
            if (reference == null)
                return null;

            var guild = await Context.Client.GetGuildAsync(reference.GuildId.Value);
            var channel = await guild.GetTextChannelAsync(reference.ChannelId);
            return await channel.GetMessageAsync(reference.MessageId.Value);
        }

        protected async Task<IMessage> GetParentMessageAsync(IMessage message)
        {
            if (message.Reference == null)
                return await GetPreviousMessageAsync(message);
            else
                return await GetReferencedMessageAsync(message);
        }
    }
}
