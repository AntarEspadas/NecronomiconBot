using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.WebSocket;
using NecronomiconBot.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace NecronomiconBot.Logic
{
    class NonStandardCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private static readonly HashSet<string> a;

        static NonStandardCommandHandler()
        {
            string aPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),"assets", "a", "a.txt");
            try
            {
                a = new HashSet<string>(File.ReadAllLines(aPath));
            }
            catch
            {
                a = new HashSet<string>(new[] { "a", "A"});
            }
        }

        public NonStandardCommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task HandleCommandAsync(SocketUserMessage message)
        {
            string messageContent = message.Content;
            var context = new SocketCommandContext(_client, message);
            int argPos = 0;
            if (IsOnlyMention(message, _client.CurrentUser))
            {
                var utility = new Utility();
                utility.SetContext(context);
                await utility.OnMention();
                return;
            }
            Regex regex = new Regex("\\bvente\\b", RegexOptions.IgnoreCase);
            if (messageContent.Length <= 35 && regex.IsMatch(messageContent))
            {
                var memes = new Memes();
                memes.SetContext(context);
                await memes.Vente();
                return;
            }
            if (a.Contains(StripNonSpacingMarks(messageContent)))
            {
                var memes = new Memes();
                memes.SetContext(context);
                await memes.A();
                return;
            }
            if (new Random().Next(0,500) == 69)
            {
                var memes = new Memes();
                memes.SetContext(context);
                await memes.FakeQuote(messageContent);
                return;
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

        private string StripNonSpacingMarks(string str)
        {
            var stringBuilder = new StringBuilder(str.Length);
            foreach (char chr in str)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(chr);
                if (category != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(chr);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
