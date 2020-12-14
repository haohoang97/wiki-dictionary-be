using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class MultiLanguageWord
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("WordValue")]
        public String WordValue { get; set; }

        [BsonElement("Language")]
        public String Language { get; set; }

        [BsonElement("AssignID")]
        public String AssignID { get; set; }
    }
}
