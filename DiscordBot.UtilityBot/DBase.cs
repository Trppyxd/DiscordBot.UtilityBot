﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.BlueBot;
using DiscordBot.BlueBot.Core;

namespace DiscordBot_BlueBot
{
    /// <summary>
    /// MUST be initialized to work with any data.
    /// </summary>
    public class DBase
    {
        private SQLiteConnection db;
        public static string dbPath;

        public DBase(SocketGuild guild)
        {
            Utilities.ValidateDirectoryExistance($@"{Program.BaseDir}Core\UserAccounts\{guild.Id}");

            dbPath = $@"{Program.BaseDir}Core\UserAccounts\{guild.Id}\UserDB-{guild.Id}.db";

            db = new SQLiteConnection(dbPath);
        }

        public void CreateUserTable()
        {
            db.CreateTable<UserAccount>();
        }

        public void AddUser(UserAccount user, SocketGuildUser gUser = null)
        {
            db.Insert(user);
            var databaseIndex = db.DatabasePath.Split('\\').Length - 1;
            var databaseFolderIndex = databaseIndex - 1;
            if (gUser == null)
            {
                Utilities.LogConsole(Utilities.LogFormat.DATABASE,
                    $"Added DB:{db.DatabasePath.Split('\\')[databaseFolderIndex]} | ID:{user.DiscordId} Name:{user.Username}");
                return;
            }
            Utilities.LogConsole(Utilities.LogFormat.DATABASE,
                $"Added DB:{db.DatabasePath.Split('\\')[databaseFolderIndex]} - \"{gUser.Guild.Name}\" | ID: {user.DiscordId} - Name: {user.Username}");

        }

        public void EditUser(ulong discordId, string dbProperty, string value)
        {
            SQLiteCommand cmd = new SQLiteCommand(db);
            cmd.CommandText = $@"Update UserAccount Set {dbProperty} = {value} Where DiscordId = {discordId}";

            int result = cmd.ExecuteNonQuery();
            // If succeeded
            if (result == 1)
            {
                Utilities.LogConsole(Utilities.LogFormat.DATABASE,
                    $"Edit Successful > User {GetDBUserByDiscordId(discordId).Username} - {discordId}, property {dbProperty}, new value {value}");
            }
            else
            {
                Utilities.LogConsole(Utilities.LogFormat.DATABASE_ERROR,
             $"Couldn't change property > User {GetDBUserByDiscordId(discordId).Username} - {discordId}, property {dbProperty}, target value {value}");
            }
        }

        public List<UserAccount> GetAllUsers()
        {
            var table = db.Table<UserAccount>();

            return table.ToList();
        }

        public void RemoveUser(UserAccount user)
        {
            db.Delete<UserAccount>(user.Id);
        }

        public void RemoveUserByDiscordId(ulong discordId)
        {
            db.Delete<UserAccount>(GetDBUserByDiscordId(discordId).DiscordId);
        }

        public UserAccount GetDBUserByDiscordId(ulong discordId)
        {
            var dId = Convert.ToInt64(discordId);
            var table = db.Table<UserAccount>().ToList(); // IMPORTANT to convert the enumerable to list first filter(lambda) data, was a real pain
            var result = table.First(x => x.DiscordId == dId);
            return result;
        }

        public List<ulong> GetUserIds()
        {
            List<ulong> ids = null;
            foreach (var user in db.Table<UserAccount>())
            {
                ids.Add(Convert.ToUInt64(user.DiscordId));
            }

            return ids;
        }

        public void CreateNewUser(ulong discId, string username, DateTimeOffset joinDate, int isMember)
        {
            if (!db.Table<UserAccount>().Any())
            {
                var newUser = new UserAccount();
                newUser.DiscordId = Convert.ToInt64(discId);
                newUser.Username = username;
                newUser.JoinDate = joinDate;
                newUser.IsMember = isMember;
                db.Insert(newUser);
            }
        }

    }
}
