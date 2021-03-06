﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;

namespace DiscordBot.BlueBot
{
    class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";

        public static BotConfig bot;

        static Config()
        {
            // if ./Resources/ doesn't exist, makes it.
            Utilities.ValidateDirectoryExistance(configFolder);
            //if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);
            // ./Resources/config.json doesn't exist, makes it
            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
            File.WriteAllText(configFolder + "/" + configFile, json);
        }
    }

    public struct BotConfig
    {
        public string token;
        public string cmdPrefix;
        public ulong botChannelId;
        public ulong logChannelId;
        public ulong guildId;
        public ulong ownerId;
    }
}
