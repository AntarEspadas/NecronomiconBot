using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NecronomiconBot.Modules
{
    [Group("configure")]
    [Alias("config","settings")]
    public class Configuration : NecroModuleBase<SocketCommandContext>
    {

        static internal readonly Dictionary<string,List<string>> settingsGroups;

        static Configuration()
        {
            settingsGroups = new Dictionary<string, List<string>>();
        }

        [Command("")]
        public async Task Configure()
        {
            var eb = new EmbedBuilder() {
                Title = "Usage",
                Description =
                "`configure` `user|guild` `set` `<settings-group>` `<setting>` `<value>`" +
                "or" +
                "`configure` `user|guild` `reset` `<settings-group>` `<setting>`"
            };
            await ReplyAsync(embed: eb.Build());
        }
        [Group("user")]
        public class User
        {

        }
        [Group("guild")]
        public class Guild
        {

        }
    }
}
