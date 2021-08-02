using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMATutorBot.Models
{
    public class MatchResult
    {
        [BsonElement("student")]
        public int Student { get; set; }
        [BsonElement("teacher")]
        public int Teacher { get; set; }
        [BsonElement("category")]
        public int Category { get; set; }
        [BsonElement("match")]
        public MatchResultType Match { get; set; }
    }
    public enum MatchResultType
    {
        Next = 0,
        Save
    }
}
