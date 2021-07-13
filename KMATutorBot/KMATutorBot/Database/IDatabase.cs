using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMATutorBot.Menu;
using KMATutorBot.Models;
using MongoDB.Driver;

namespace KMATutorBot
{
    interface IDatabase
    {
        const string CONNECTION_STRING = @"mongodb+srv://user1:user1user1@cluster0.di7oe.mongodb.net/test";

        #region DAL

        (BotUser user, bool isNew) GetOrCreateUser(Models.BotUser source);
        BotUser UpdateUserMenuSection(Models.BotUser user, MenuSection newMenuSection);

        #endregion
    }
}
