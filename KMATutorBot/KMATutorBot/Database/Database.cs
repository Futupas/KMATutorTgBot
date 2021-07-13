using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KMATutorBot
{
    internal class Database
    {
        private const string CONNECTION_STRING = @"mongodb+srv://user1:user1user1@cluster0.di7oe.mongodb.net/test";
        public Database()
        {
            MongoClient client = new MongoClient(CONNECTION_STRING);
            //IMongoDatabase database = client.GetDatabase("MainDB");
            var dbs = client.ListDatabases();
            var a = dbs.ToList();
            Console.WriteLine("d");
        }
    }
}
