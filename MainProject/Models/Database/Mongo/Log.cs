using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BasicDotnetTemplate.MainProject.Models.Database.Mongo
{
    public class Log
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}