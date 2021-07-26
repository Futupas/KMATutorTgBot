using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;
using System.Collections.Generic;

namespace KMATutorBot.Models
{
    public class BotUser
    {
        [BsonId]
        [BsonElement("telegramId")]
        public long Id { get; init; }

        [BsonElement("menuSection")]
        public int MenuSection { get; set; } = 0;

        [BsonElement("displayName")]
        public string DisplayName { get; set; } = null;

        [BsonElement("telegramUsername")]
        public string TelegramUsername { get; set; } = null;

        /// <summary>About me information</summary>
        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("telegramName")]
        public string TelegramName { get; init; } // I'm not sure, it is nesessary

        [BsonElement("teacherCategories")]
        public int[] TeacherCategories { get; set; } = null; // null if user isn't teacher

        [BsonElement("isAdmin")]
        public bool IsAdmin { get; init; } = false;

        [BsonElement("registrationDate")]
        public long RegistrationDate { get; init; } = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();

        [BsonElement("registrationMessage")]
        public string RegistrationMessage { get; init; }

        [BsonElement("licenseExpired")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? LicenseExpired { get; set; } = null;

        /// <summary>
        /// True, if license is valid right now
        /// </summary>
        [BsonIgnore]
        public bool LicenseOk
        {
            get
            {
                return DateTime.Now <= LicenseExpired;
            }
        }

        [BsonElement("data")]
        public Dictionary<string, object> Data { get; set; } = new();
    }
}
