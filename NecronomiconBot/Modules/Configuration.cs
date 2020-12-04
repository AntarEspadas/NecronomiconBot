using Discord;
using Discord.Commands;
using NecronomiconBot.Settings;
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
        public async Task Configure(string scope, string setting, string action, IChannel channel = null)
            => await GenericConfigure(scope, setting, action, channel);
        [Command("")]
        public async Task Configure(string scope, string setting, string action, string value, IChannel channel = null)
            => await GenericConfigure(scope, setting, action, value, channel);
        [Command("")]
        public async Task Configure(string scope, string setting, string action, string value, string level = null)
            => await GenericConfigure(scope, setting, action, value, level);
        private async Task GenericConfigure(string scope, string setting, string action, object arg1, object arg2 = default)
        {
            if (!(scope == "user" || scope == "server"))
            {
                await ReplyAsync($"Unknown value `{scope}`, must be one of `user` or `server`");
                return;
            }
            if (!BotSettings.Instance.Schema.TryGetValue(setting, out var settingInfo) || !settingInfo.Visible)
            {
                await ReplyAsync($"Unknown setting `{setting}`");
                return;
            }
            if (scope == "user" && settingInfo.Scope == Scope.Guild)
            {
                await ReplyAsync($"Setting `{setting}` is a server-only setting");
                return;
            }
            else if (scope == "server" && settingInfo.Scope == Scope.User)
            {
                await ReplyAsync($"Setting `{setting}` is a user-only setting");
                return;
            }
            if (scope == "server" && ! await VerifyPermissions(settingInfo))
            {
                return;
            }
            string value = null;
            object level;
            if (action == "set")
            {
                value = arg1 as string;
                level = arg2;
            }
            else if (action == "get" || action == "reset")
            {
                level = arg1;
            }
            else
            {
                await ReplyAsync($"Unknown value `{action}`, must be one of either `set`, `reset`, or `get`");
                return;
            }
            if (scope == "user" && !(level is null || (level is string levelString && levelString.ToLower() == "global") ))
            {
                await ReplyAsync($"Unsupported value `{level}`, field must be either `global` or empty");
                return;
            }
            if (scope == "server" && !(level is null || level is IChannel))
            {
                await ReplyAsync($"Unsupported value `{level}`, field must be either a channel or empty");
                return;
            }
            if (action == "set" && !settingInfo.IsValid(value))
            {
                await ReplyAsync(settingInfo.GetErrorMessage(value));
                return;
            }

            BotSettings settings = BotSettings.Instance;

            GetUserSetting getUserSetting;
            GetGlobalUserSetting getGlobalUserSetting;
            GetGuildSetting getGuildSetting;
            GetChannelSetting getChannelSetting;
            if (action == "get")
            {
                getUserSetting = settings.GetUserSettingOrDefault<string>;
                getGlobalUserSetting = settings.GetGlobalUserSettingOrDefault<string>;
                getGuildSetting = settings.GetGuildSettingOrDefault<string>;
                getChannelSetting = settings.GetChannelSettingOrDefault<string>;
            }
            else
            {
                getUserSetting = settings.GetUserSetting;
                getGlobalUserSetting = settings.GetGlobalUserSetting;
                getGuildSetting = settings.GetGuildSetting;
                getChannelSetting = settings.GetChannelSetting;
            }
            IList<string> settingList;
            if (scope == "user")
            {
                if (level is null)
                {
                    settingList = getUserSetting(setting, Context.Guild.Id, Context.User.Id);
                }
                else
                {
                    settingList = getGlobalUserSetting(setting, Context.User.Id);
                }
            }
            else
            {
                if (level is null)
                {
                    settingList = getGuildSetting(setting, Context.Guild.Id);
                }
                else
                {
                    settingList = getChannelSetting(setting, Context.Guild.Id, ((IChannel)level).Id);
                }
            }
            switch (action)
            {
                case "set":
                    if (settingList.Count == 0)
                        settingList.Add(value);
                    else
                        settingList[0] = value;
                    await ReplyAsync($"Changed value of setting `{setting}` to `{value}`");
                    break;
                case "reset":
                    settingList.Clear();
                    await ReplyAsync($"Value for setting `{setting}` has been set back to its default");
                    break;
                case "get":
                    await ReplyAsync($"The value for setting `{setting}` is currently `{settingList[0]}`");
                    break;
            }
        }
        [Command("")]
        public async Task Configure()
        {
            var eb = new EmbedBuilder() {
                Title = "Usage",
                Description =
                "`configure` `user` `<setting>` `set` `<value>` `[global]`\n" +
                "or\n" +
                "`configure` `user` `<setting>` `reset|get` `[global]`\n" +
                "or" +
                "`configure` `server` `<setting>` `set` `<value>` `[<channel>]`\n" +
                "or\n" +
                "`configure` `server` `<setting>` `reset|get` `[<channel>]`"
            };
            await ReplyAsync(embed: eb.Build());
        }

        private delegate IList<string> GetUserSetting(string setting, ulong key1, ulong key2);
        private delegate IList<string> GetGlobalUserSetting(string setting, ulong key);
        private delegate IList<string> GetGuildSetting(string setting, ulong key);
        private delegate IList<string> GetChannelSetting(string setting, ulong key1, ulong key2);

        private async Task<bool> VerifyPermissions(SettingInfo settingInfo)
        {
            var settings = BotSettings.Instance;
            var user = Context.User as IGuildUser;
            if (user.GuildPermissions.Has(GuildPermission.Administrator))
                return true;
            string roleName = settings.GetGuildSettingOrDefault<string>("admin:bot-role-name", Context.Guild.Id)[0];
            if (settingInfo.PermissionRole && GetUserRoleNames(user).Contains(roleName))
                return true;
            foreach (var permission in settingInfo.GuildPermissions)
                if (user.GuildPermissions.Has(permission))
                    return true;
            foreach (var permission in settingInfo.ChannelPermissions)
                if (user.GetPermissions(Context.Channel as IGuildChannel).Has(permission))
                    return true;

            string message = "";
            if (settingInfo.PermissionRole)
            {
                message += $"A role named `{roleName}`\n\n";
            }
            if (settingInfo.GuildPermissions.Count > 0)
            {
                message += $"Server permissions```";
                foreach (var permission in settingInfo.GuildPermissions)
                {
                    message += "\n" +  permission.ToString();
                }
                message += "```\n\n";
            }
            if (settingInfo.ChannelPermissions.Count > 0)
            {
                message += "Channel permissions```";
                foreach (var permission in settingInfo.ChannelPermissions)
                {
                    message += "\n" + permission.ToString();
                }
                message += "```";
            }
            var eb = new EmbedBuilder()
            {
                Title = "Insufficient permissions, must have either of the following to alter this command:",
                Description = message
            };

            await ReplyAsync(embed:eb.Build());

            return false;
        }
        private List<string> GetUserRoleNames(IGuildUser user)
        {
            var result = new List<string>(user.RoleIds.Count);
            foreach (var roleId in user.RoleIds)
            {
                result.Add(Context.Guild.GetRole(roleId).Name);
            }
            return result;
        }
    }
}
