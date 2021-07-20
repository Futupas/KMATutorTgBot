﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
        public string DisplayName { get; set; } //todo may be get; init...

        /// <summary>About me information</summary>
        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("telegramName")]
        public string TelegramName { get; init; } // I'm not sure, it is nesessary

        [BsonElement("studentCategories")]
        public int[] StudentCategories { get; set; } = null; // null if user isn't student

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
        public DateTime? LicenseExpired { get; private set; } = null;

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
    }
}
