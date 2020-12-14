using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Project.Models
{
    [BsonIgnoreExtraElements]
    public class Words
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }

        [BsonElement("id")]
        public int Id { get; set; }

        [BsonElement("Word")]
        public String Word { get; set; }

        [BsonElement("description")]
        public String Description { get; set; }

        public Words(int id, string word, string description)
        {
            Id = id;
            Word = word;
            Description = description;
        }
    }
}
