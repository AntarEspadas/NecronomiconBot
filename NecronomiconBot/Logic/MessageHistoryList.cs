using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace NecronomiconBot.Logic
{
    class MessageHistoryList : LinkedList<IMessage>
    {
        public bool IsDeleted { get { return DeletedTimestamp != null; } }
        public DateTimeOffset? DeletedTimestamp { get; set; } = null;
    }
}
