using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Discord;

namespace NecronomiconBot.Settings
{
    public class SettingInfo
    {
        public Type Type { get; private set; } = typeof(string);
        public ImmutableList<string> DefaultValues { get; private set; } = new string[0].ToImmutableList();
        public ImmutableList<GuildPermission> GuildPermissions { get; private set; } = new GuildPermission[0].ToImmutableList();
        public ImmutableList<ChannelPermission> ChannelPermissions { get; private set; } = new ChannelPermission[0].ToImmutableList();
        public bool PermissionRole { get; private set; } = true;
        public string ValidationRegex { get; private set; } = ".+";
        public string RawErrorMessage { get; private set; } = "The value `%s` is invalid for this setting";
        public Scope Scope { get; private set; } = Scope.Both;
        public bool Visible { get; private set; } = true;

        public SettingInfo(Type type, IEnumerable<string> defaultValues, IEnumerable<GuildPermission> guildPermissions, IEnumerable<ChannelPermission> channelPermissions, bool permissionRole, string validationRegex, string rawErrorMessage, Scope scope, bool visible)
        {
            Type = type;
            DefaultValues = defaultValues.ToImmutableList();
            GuildPermissions = guildPermissions.ToImmutableList();
            ChannelPermissions = channelPermissions.ToImmutableList();
            PermissionRole = permissionRole;
            ValidationRegex = validationRegex;
            RawErrorMessage = rawErrorMessage;
            Scope = scope;
            Visible = visible;
        }

        public bool IsValid(string value)
        {
            return new Regex(ValidationRegex, RegexOptions.None, TimeSpan.FromSeconds(5)).IsMatch(value);
        }

        public string GetErrorMessage(string value)
        {
            return RawErrorMessage.Replace("%s", value);
        }
    }
}
