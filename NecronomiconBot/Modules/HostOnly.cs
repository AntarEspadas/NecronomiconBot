using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using System.Threading;

namespace NecronomiconBot.Modules
{
    public class HostOnly : NecroModuleBase<SocketCommandContext>
    {
        [Group("live share")]
        [Alias("live", "share","live-share")]
        [RequireOwner]
        public class LiveShare : NecroModuleBase<SocketCommandContext>
        {
            private static string path;
            private static FileSystemWatcher watcher = null;
            private static string messageContent;
            private static string code;
            private static IUserMessage message;
            private static readonly Mutex mutex = new Mutex();
            private static bool hasQueue = false;
            private static bool canSendMessages = true;
            private static System.Timers.Timer timer = new System.Timers.Timer(5000);

            static LiveShare()
            {
                timer.Elapsed += TimerElapsed;
                timer.AutoReset = false;
            }

            [Priority(300)]
            [Command("stop")]
            [Alias("end", "exit", "quit", "kill")]
            public async Task Stop()
            {
                if (watcher == null)
                {
                    await ReplyAsync("No ongoing live session that can be stopped, please use `live share [path to file]` to start a live share session");
                    return;
                }
                await message.ModifyAsync(message => { message.Content = messageContent.Replace("%status%", "OFFLINE").Replace("%code%", code); });
                messageContent = null;
                watcher = null;
                message = null;
                code = null;
                hasQueue = false;
                canSendMessages = true;
                await ReplyAsync("Live share session stopped");
            }

            [Priority(100)]
            [Command("")]
            private async Task Start(string path, string language = null)
            {
                _ = Share(path, language);
            }
            private async Task Share(string path, string language)
            {
                if (watcher != null)
                {
                    await ReplyAsync("A live share session is already ongoing, please use the command `live share stop` to sto the current session");
                    return;
                }
                if (!File.Exists(path))
                {
                    await ReplyAsync($"The file `{path}` could not be found");
                    return;
                }
                LiveShare.path = path;
                language ??= Path.GetExtension(path).Substring(1);
                messageContent = $"Sharing file **{Path.GetFileName(path)}**\n" +
                    $"Status: **[%status%]**\n" +
                    $"```{language}\n%code%```";
                mutex.WaitOne();
                try
                {
                    code = ReadAll(path);
                    message = await ReplyAsync(messageContent.Replace("%status%", "ONLINE").Replace("%code%", code));
                    mutex.ReleaseMutex();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }

                watcher = new FileSystemWatcher(Path.GetDirectoryName(path))
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };
                watcher.Changed += FileChanged;
            }

            private static void FileChanged(object sender, FileSystemEventArgs e)
            {
                mutex.WaitOne();
                try
                {
                    string code = ReadAll(path);
                    if (code == string.Empty)
                        return;
                    if (!canSendMessages)
                    {
                        hasQueue = true;
                        return;
                    }
                    LiveShare.code = code;
                    message.ModifyAsync(message => { message.Content = messageContent.Replace("%status%", "ONLINE").Replace("%code%", code); });
                    canSendMessages = false;
                    hasQueue = false;
                    timer.Start();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            private static string ReadAll(string path)
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }

            private static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                canSendMessages = true;
                if (hasQueue)
                    FileChanged(null, null);
            }
        }
    }
}
