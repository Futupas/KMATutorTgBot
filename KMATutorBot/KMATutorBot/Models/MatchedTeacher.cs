using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Models
{
    [BsonIgnoreExtraElements]
    public class MatchedTeacher : BotUser
    {
        [BsonElement("matches")]
        public IEnumerable<MatchResult> Matches { get; set; }
    }
}
