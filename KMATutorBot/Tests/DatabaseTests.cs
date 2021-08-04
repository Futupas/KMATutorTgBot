using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using KMATutorBot.Models;

namespace Tests
{
    public class DatabaseTests
    {
        private const string CONNECTION_STRING = @"mongodb+srv://user1:user1user1@cluster0.di7oe.mongodb.net/test";

        private MongoClient Client { get; set; }
        private IMongoDatabase DB { get; set; }

        #region Collections
        private IMongoCollection<BotUser> BotUsers;
        private IMongoCollection<MatchResult> Matches;
        #endregion

        public DatabaseTests()
        {
            Client = new MongoClient(CONNECTION_STRING);
            DB = Client.GetDatabase("MainDB");

            BotUsers = DB.GetCollection<BotUser>("BotUsers");
            Matches = DB.GetCollection<MatchResult>("Matches");
        }

        public void Test1()
        {
            var studentId = 387489833;

            var teachers = BotUsers
                .Aggregate()
                .Lookup("Matches", "_id", "teacher", "matches")
                ;


            //foreach (var t in teachers)
            //{
            //    Console.WriteLine(t.ToJson());
            //    //Console.WriteLine(t.GetElement("ma").ToJson());
            //    Console.WriteLine();
            //}
        }
    }
}
