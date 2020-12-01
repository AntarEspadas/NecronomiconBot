using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Timers;

namespace NecronomiconBot.Logic
{
    public class BotSettings
    {
        public static BotSettings Instance { get => GetInstance(); }
        private static BotSettings instance = null;
        public Dictionary<ulong, Dictionary<string, LinkedList<string>>> GuildSettings { get; set; } = new Dictionary<ulong, Dictionary<string, LinkedList<string>>>();
        public Dictionary<ulong, Dictionary<string, LinkedList<string>>> GlobalUserSettings { get; set; } = new Dictionary<ulong, Dictionary<string, LinkedList<string>>>();
        public Dictionary<string, Dictionary<string, LinkedList<string>>> UserSettings { get; set; } = new Dictionary<string, Dictionary<string, LinkedList<string>>>();
        private Timer timer;
        public string Token { get; set; } = null;
        public static readonly string path = Path.Combine(".","BotSettings.json");

        private static BotSettings GetInstance()
        {
            if (instance == null)
                try
                {
                    string json = File.ReadAllText(path);
                    instance = JsonConvert.DeserializeObject<BotSettings>(json);
                }
                catch (Exception)
                {
                }
            else
                return instance;
            return instance ??= new BotSettings();
        }

        private BotSettings()
        {
            timer = new Timer();
            timer.Interval = 10000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        public LinkedList<string> GetGuildSetting(ulong guildId, string setting)
        {
            var settings = GetValueOrCreate(guildId, GuildSettings);
            return GetValueOrCreate(setting, settings);
        }

        public LinkedList<string> GetGuildSetting(ulong guildId, string setting, LinkedList<string> defaultValue)
        {
            var settingList = GetGuildSetting(guildId, setting);
            if (settingList.Count == 0)
                return defaultValue;
            return settingList;
        }

        public LinkedList<string> GetGuildSetting(ulong guildId, string setting, IEnumerable<string> defaultValue)
        {
            return GetGuildSetting(guildId, setting, new LinkedList<string>(defaultValue));
        }

        public LinkedList<string> GetGuildSetting(ulong guildId, string setting, string defaultValue)
        {
            var list = new LinkedList<string>();
            list.AddLast(defaultValue);
            return GetGuildSetting(guildId, setting, list);
        }

        public LinkedList<string> GetGlobalUserSetting(ulong userId, string setting)
        {
            var settings = GetValueOrCreate(userId, GlobalUserSettings);
            return GetValueOrCreate(setting, settings);
        }

        public LinkedList<string> GetGlobalUserSetting(ulong userId, string setting, LinkedList<string> defaultValue)
        {
            var settingList = GetGlobalUserSetting(userId, setting);
            if (settingList.Count == 0)
                return defaultValue;
            return settingList;
        }

        public LinkedList<string> GetGlobalUserSetting(ulong userId, string setting, IEnumerable<string> defaultValue)
        {
            return GetGlobalUserSetting(userId, setting, new LinkedList<string>(defaultValue));
        }

        public LinkedList<string> GetGlobalUserSetting(ulong userId, string setting, string defaultValue)
        {
            var list = new LinkedList<string>();
            list.AddLast(defaultValue);
            return GetGlobalUserSetting(userId, setting, list);
        }

        public LinkedList<string> GetUserSetting(ulong guildId, ulong userId, string setting)
        {
            var settings = GetValueOrCreate($"{guildId}_{userId}", UserSettings);
            var settingList =  GetValueOrCreate(setting, settings);
            if (settingList.Count == 0)
                return GetGlobalUserSetting(userId, setting);
            return settingList;
        }

        public LinkedList<string> GetUserSetting(ulong guildId, ulong userId, string setting, LinkedList<string> defaultValue)
        {
            var settingList = GetUserSetting(guildId, userId, setting);
            if (settingList.Count == 0)
                return defaultValue;
            return settingList;
        }

        public LinkedList<string> GetUserSetting(ulong guildId, ulong userId, string setting, IEnumerable<string> defaultValue)
        {
            return GetUserSetting(guildId, userId, setting, new LinkedList<string>(defaultValue));
        }

        public LinkedList<string> GetUserSetting(ulong guildId, ulong userId, string setting, string defaultValue)
        {
            var list = new LinkedList<string>();
            list.AddLast(defaultValue);
            return GetUserSetting(guildId, userId, setting, list);
        }
        private TValue GetValueOrCreate<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dict) where TValue : new()
        {
            if (dict.TryGetValue(key, out var value))
                return value;
            return dict[key] = new TValue();
        }
    }
}
