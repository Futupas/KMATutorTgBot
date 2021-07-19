using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using KMATutorBot.Menu;

namespace KMATutorBot
{
    internal class Database: IDatabase
    {
        //private const string CONNECTION_STRING = @"mongodb+srv://user1:user1user1@cluster0.di7oe.mongodb.net/test";
        private MongoClient Client { get; set; }
        private IMongoDatabase DB { get; set; }

        #region Collections
        private IMongoCollection<Models.BotUser> BotUsers;
        #endregion

        public Database()
        {
            Client = new MongoClient(IDatabase.CONNECTION_STRING);
            DB = Client.GetDatabase("MainDB");

            BotUsers = DB.GetCollection<Models.BotUser>("BotUsers");
        }


        #region DAL

        public (Models.BotUser user, bool isNew) GetOrCreateUser(Models.BotUser source)
        {
            var user = BotUsers.Find(user => user.Id == source.Id).FirstOrDefault();
            if (user == null)
            {
                BotUsers.InsertOne(source);
                return (source, true);
            }
            else
            {
                return (user, false);
            }
        }
        public Models.BotUser UpdateUserMenuSection(Models.BotUser user, MenuSection newMenuSection)
        {
            if (user == null || newMenuSection == null) return user;
            BotUsers.UpdateOne(u => u.Id == user.Id, Builders<Models.BotUser>.Update.Set("menuSection", newMenuSection.Id));
            user.MenuSection = newMenuSection.Id;
            return user;
        }

        #endregion
    }
}
