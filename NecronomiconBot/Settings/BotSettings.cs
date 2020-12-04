using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Timers;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace NecronomiconBot.Settings
{
    [Serializable]
    public class BotSettings
    {
        public static BotSettings Instance { get; private set; }
        public Dictionary<ulong, Dictionary<string, LinkedList<string>>> GuildSettings { get; set; } = new Dictionary<ulong, Dictionary<string, LinkedList<string>>>();
        public Dictionary<string, Dictionary<string, LinkedList<string>>> ChannelSettings { get; set; } = new Dictionary<string, Dictionary<string, LinkedList<string>>>();
        public Dictionary<ulong, Dictionary<string, LinkedList<string>>> GlobalUserSettings { get; set; } = new Dictionary<ulong, Dictionary<string, LinkedList<string>>>();
        public Dictionary<string, Dictionary<string, LinkedList<string>>> UserSettings { get; set; } = new Dictionary<string, Dictionary<string, LinkedList<string>>>();
        private Timer timer;
        public string Token { get; set; } = null;
        private string path;
        private static bool save = true;
        [JsonIgnore]
        public ImmutableDictionary<string, SettingInfo> Schema { get { return schema; } }
        [JsonIgnore]
        private ImmutableDictionary<string, SettingInfo> schema;
        public static void Init(string settingsPath, string schemaPath)
        {
            try
            {
                string json = File.ReadAllText(settingsPath);
                Instance = JsonConvert.DeserializeObject<BotSettings>(json);
            }
            catch (FileNotFoundException) { }

            try { File.Copy(settingsPath + ".back", settingsPath + ".back.back", true); }
            catch (FileNotFoundException) { }
            catch (Exception)
            {
                Console.WriteLine("An error ocurred when trying to created the settings backup file. Saving of user or guild settings will be disabled for this session");
                save = false;
            }

            try { File.Copy(settingsPath, settingsPath + ".back", true); }
            catch (FileNotFoundException) { }
            catch (Exception)
            {
                Console.WriteLine("An error ocurred when trying to created the settings backup file. Saving of user or guild settings will be disabled for this session");
                save = false;
            }

            Instance ??= new BotSettings();
            Instance.path = settingsPath;
            Instance.ReadSchema(schemaPath);
            if (save)
                Instance.timer.Start();
        }

        private BotSettings()
        {
            timer = new Timer();
            timer.Interval = 10000;
            timer.Elapsed += Timer_Elapsed;
        }
        private void ReadSchema(string path)
        {
            schema = JsonConvert.DeserializeObject<ImmutableDictionary<string, SettingInfo>>(File.ReadAllText(path));
        }
        public SettingInfo GetSettingInfo(string setting)
        {
            if (!schema.TryGetValue(setting, out var result))
                throw new SettingNotFoundException($"The setting {setting} is not part of the current schema");
            return result;
        }
        private void ValidateType<T>(string setting)
        {
            var type = GetSettingInfo(setting).Type;
            if (type != typeof(T) && typeof(T) != typeof(string))
                throw new TypeMissmatchException($"Cannot return setting {setting} as {typeof(T)} because it is of the type {type}");
        }
        public ImmutableList<T> GetUserSettingOrDefault<T>(string setting, ulong guildId, ulong userId)
        {
            ValidateType<T>(setting);
            var result = GetSettingOrDefault(UserSettings, GlobalUserSettings, guildId, userId, setting);
            return ToTypedImmutableList<T>(result);
        }
        public ImmutableList<T> GetChannelSettingOrDefault<T>(string setting, ulong guildId, ulong channelId)
        {
            ValidateType<T>(setting);
            var result = GetSettingOrDefault(ChannelSettings, GuildSettings, channelId, guildId, setting);
            return ToTypedImmutableList<T>(result);
        }
        public ImmutableList<T> GetGlobalUserSettingOrDefault<T>(string setting, ulong userId)
        {
            ValidateType<T>(setting);
            var result = GetSettingOrDefault(GlobalUserSettings, userId, setting);
            return ToTypedImmutableList<T>(result);
        }
        public ImmutableList<T> GetGuildSettingOrDefault<T>(string setting, ulong guildId)
        {
            ValidateType<T>(setting);
            var result = GetSettingOrDefault(GuildSettings, guildId, setting);
            return ToTypedImmutableList<T>(result);
        }
        private IEnumerable<string> GetSettingOrDefault(Dictionary<string, Dictionary<string, LinkedList<string>>> dict, Dictionary<ulong, Dictionary<string, LinkedList<string>>> backupDict, ulong key1, ulong key2, string setting)
        {
            if (!dict.TryGetValue($"{key1}_{key2}", out var dict2) || !dict2.TryGetValue(setting, out var settingList) || settingList.Count == 0)
                return GetSettingOrDefault(backupDict, key2, setting);
            return settingList;
        }
        private IEnumerable<string> GetSettingOrDefault(Dictionary<ulong, Dictionary<string, LinkedList<string>>> dict, ulong key1, string setting)
        {
            if (!dict.TryGetValue(key1, out var dict2) || !dict2.TryGetValue(setting, out var settingList) || settingList.Count == 0)
                return schema[setting].DefaultValues.ToImmutableList();
            return settingList;
        }
        public SettingList GetUserSetting(string setting, ulong guildId, ulong userId)
        {
            var settingInfo = GetSettingInfo(setting);
            return new SettingList(GetSettingOrCreate(UserSettings, $"{guildId}_{userId}", setting), settingInfo.Type);
        }
        public SettingList GetChannelSetting(string setting, ulong guildId, ulong channelId)
        {
            var settingInfo = GetSettingInfo(setting);
            return new SettingList(GetSettingOrCreate(ChannelSettings, $"{channelId}_{guildId}", setting), settingInfo.Type);
        }
        public SettingList GetGlobalUserSetting(string setting, ulong userId)
        {
            var settingInfo = GetSettingInfo(setting);
            return new SettingList(GetSettingOrCreate(GlobalUserSettings, userId, setting), settingInfo.Type);
        }
        public SettingList GetGuildSetting(string setting, ulong guildId)
        {
            var settingInfo = GetSettingInfo(setting);
            return new SettingList(GetSettingOrCreate(GuildSettings, guildId, setting),settingInfo.Type);
        }
        private LinkedList<string> GetSettingOrCreate<T>(Dictionary<T, Dictionary<string, LinkedList<string>>> dict, T key, string setting)
        {
            var dict2 = GetValueOrCreate(key, dict);
            return GetValueOrCreate(setting, dict2);
        }
        private TValue GetValueOrCreate<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dict) where TValue : new()
        {
            if (dict.TryGetValue(key, out var value))
                return value;
            return dict[key] = new TValue();
        }
        private ImmutableDictionary<string, ImmutableList<string>> ToImmutable(Dictionary<string, IEnumerable<string>> dict)
        {
            var dictImmutableValues = new Dictionary<string, ImmutableList<string>>(dict.Count);
            foreach (var pair in dict)
            {
                dictImmutableValues[pair.Key] = pair.Value.ToImmutableList();
            }
            return dictImmutableValues.ToImmutableDictionary();
        }
        public void Save()
        {
            if (!save)
                return;
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Save();
        }
        private ImmutableList<T> ToTypedImmutableList<T>(IEnumerable<string> enumerable)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            List<T> list = new List<T>(enumerable.Count());
            foreach (var item in enumerable)
            {
                list.Add((T)converter.ConvertFromString(item));
            }
            return list.ToImmutableList();
        }

    }
}
