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

        //public (
        //    List<(int id, string name)> Students, 
        //    List<(int id, string name)> Teachers
        //    ) Categories = (
        //        new() { (1, "Math"), (2, "Programming"), (3, "Philosophy") }, 
        //        new() { (1, "Math"), (2, "Programming"), (3, "Philosophy") }
        //    );
        //public List<(int id, string name)> Categories =
        //        new() { (1, "Math"), (2, "Programming"), (3, "Philosophy") };


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
        public Models.BotUser UpdateUserDisplayName(Models.BotUser user, string name)
        {
            if (user == null) return user;
            BotUsers.UpdateOne(u => u.Id == user.Id, Builders<Models.BotUser>.Update.Set("displayName", name));
            user.DisplayName = name;
            return user;
        }
        public Models.BotUser UpdateUserDescription(Models.BotUser user, string description)
        {
            if (user == null) return user;
            BotUsers.UpdateOne(u => u.Id == user.Id, Builders<Models.BotUser>.Update.Set("description", description));
            user.Description = description;
            return user;
        }

        /// <param name="categories">Can be null</param>
        public Models.BotUser UpdateUserTeacherCategories(Models.BotUser user, int[] categories)
        {
            if (user == null) return user;
            BotUsers.UpdateOne(u => u.Id == user.Id, Builders<Models.BotUser>.Update.Set("teacherCategories", categories));
            user.TeacherCategories = categories;
            return user;
        }

        //public IEnumerable<Models.BotUser> GetAllFreeStudents(int[] categories, int? limit = 10)
        //{
        //    if (categories == null || !categories.Any()) return Array.Empty<Models.BotUser>();

        //    var filterBuilder = Builders<Models.BotUser>.Filter.Where(user => user.StudentCategories != null);
        //    filterBuilder &= Builders<Models.BotUser>.Filter.AnyIn(user => user.StudentCategories, categories);

        //    var students = BotUsers
        //        .Find(filterBuilder)
        //        .Limit(limit)
        //        .ToEnumerable();

        //    return students;
        //}
        //public IEnumerable<Models.BotUser> GetAllFreeTeachers(int[] categories, int? limit = 10)
        //{
        //    //todo refactor this as students finder
        //    if (categories == null || !categories.Any()) return Array.Empty<Models.BotUser>();

        //    var teachers = BotUsers
        //        .Find(user => user.TeacherCategories != null && user.TeacherCategories.Intersect(categories).Any())
        //        .Limit(limit)
        //        .ToEnumerable();

        //    return teachers;
        //}
        public IEnumerable<Models.BotUser> GetTeachersByCategory(int category, int? limit = 10)
        {
            var teachers = BotUsers
                .Find(user => user.TeacherCategories != null && user.TeacherCategories.Contains(category))
                .Limit(limit)
                .ToEnumerable();

            return teachers;
        }

        #endregion
    }
}
